using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.SpecialOperations;
using Randevoo.AdminPanel.Services.Permissions;
using Randevoo.Application.Interfaces.Currencies;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseSpecialOperationsApiClient : ISpecialOperationsApiClient
{
    private const string PermissionEntity = "specialOperations";
    private const string OperationReferenceType = nameof(SpecialOperationLog);

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RandevooDbContext _db;
    private readonly IOperationPermissionService _permissions;
    private readonly ICurrencyExchangeRateProvider _exchangeRates;

    public DatabaseSpecialOperationsApiClient(
        RandevooDbContext db,
        IOperationPermissionService permissions,
        ICurrencyExchangeRateProvider exchangeRates)
    {
        _db = db;
        _permissions = permissions;
        _exchangeRates = exchangeRates;
    }

    public async Task<SpecialOperationPreview> PreviewCancelTicketRefundAsync(MockUser currentUser, CancelTicketRefundInput input, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "cancelTicketRefundToWallet", cancellationToken);
        var ticket = await LoadRefundableTicketAsync(input.TicketId, cancellationToken);
        if (ticket.TicketOrder.BuyerUserId != input.BuyerUserId)
            throw new InvalidOperationException("شناسه خریدار با بلیت انتخاب‌شده تطابق ندارد.");

        return new SpecialOperationPreview(
            "CancelTicketRefundToWallet",
            "کنسل بلیت و برگشت مبلغ به کیف پول",
            new[]
            {
                Row("بلیت", $"#{ticket.Id}"),
                Row("سفارش", $"#{ticket.TicketOrderId}"),
                Row("رویداد", ticket.DatingEvent.Title),
                Row("خریدار", FormatUser(ticket.TicketOrder.BuyerUser)),
                Row("شرکت‌کننده", FormatUser(ticket.User)),
                Row("مبلغ برگشت", $"{ticket.ReportingPriceIrr:N0} ریال ایران", isDanger: true),
                Row("وضعیت بعد از اجرا", "بلیت Refund می‌شود و مبلغ به کیف پول خریدار اضافه می‌شود", isDanger: true)
            },
            Array.Empty<string>());
    }

    public Task<SpecialOperationExecuteResult> ExecuteCancelTicketRefundAsync(MockUser currentUser, CancelTicketRefundInput input, CancellationToken cancellationToken = default)
        => ExecuteWithLogAsync(
            currentUser,
            "cancelTicketRefundToWallet",
            "CancelTicketRefundToWallet",
            input.IdempotencyKey,
            input.Reason,
            input.SupportTicketNumber,
            input,
            async ct => await PreviewCancelTicketRefundAsync(currentUser, input, ct),
            async (operation, ct) =>
            {
                var ticket = await LoadRefundableTicketAsync(input.TicketId, ct);
                if (ticket.TicketOrder.BuyerUserId != input.BuyerUserId)
                    throw new InvalidOperationException("شناسه خریدار با بلیت انتخاب‌شده تطابق ندارد.");

                var buyerAccount = await GetOrCreateBalanceAccountAsync(ticket.TicketOrder.BuyerUser, ct);
                ticket.MarkRefunded();
                if (ticket.TicketOrder.Tickets.All(item => item.IsRefunded || item.IsRemoved))
                    ticket.TicketOrder.MarkRefunded();

                buyerAccount.Credit(
                    ticket.Price,
                    BalanceTransactionType.TicketRefund,
                    $"بازگشت ویژه بلیت رویداد {ticket.DatingEvent.Title}: {input.Reason}",
                    ticket.DatingEventId,
                    OperationReferenceType,
                    operation.Id,
                    currentUser.Id,
                    ticket.CurrencyCode,
                    ticket.ReportingPriceIrr,
                    ticket.ExchangeRateToIrr,
                    ticket.ExchangeRateCapturedAtUtc,
                    ticket.ExchangeRateId,
                    ticket.TicketOrder);

                await _db.SaveChangesAsync(ct);
                var walletTransaction = LatestTransaction(buyerAccount);
                return new OperationMutationResult(
                    $"بلیت #{ticket.Id} کنسل شد و مبلغ به کیف پول خریدار برگشت.",
                    input.BuyerUserId,
                    ticket.Id,
                    ticket.TicketOrderId,
                    ticket.DatingEventId,
                    walletTransaction.Id == 0 ? null : walletTransaction.Id,
                    ticket.ReportingPriceIrr,
                    ticket.CurrencyCode,
                    new
                    {
                        ticketId = ticket.Id,
                        orderId = ticket.TicketOrderId,
                        eventId = ticket.DatingEventId,
                        buyerUserId = input.BuyerUserId,
                        walletTransactionId = walletTransaction.Id,
                        refundAmount = ticket.Price,
                        reportingRefundAmountIrr = ticket.ReportingPriceIrr
                    });
            },
            cancellationToken);

    public async Task<SpecialOperationPreview> PreviewManualIssueTicketAsync(MockUser currentUser, ManualIssueTicketInput input, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "manualIssueTicketWithWalletDebit", cancellationToken);
        var context = await LoadManualIssueContextAsync(input.UserId, input.EventId, cancellationToken);
        var price = context.Event.GetTicketPriceForGender(context.Profile.Gender);
        var currencyCode = context.Event.GetTicketCurrencyForGender(context.Profile.Gender);
        var exchangeRate = await _exchangeRates.GetActiveRateToIrrAsync(currencyCode, DateTime.UtcNow, cancellationToken);
        var reportingPrice = ConvertToIrr(price, exchangeRate.Rate);
        var remainingCapacity = RemainingCapacity(context.Event, context.Profile.Gender);

        if (context.Account is null)
            throw new InvalidOperationException("کاربر کیف پول ندارد و امکان کسر مبلغ برای صدور دستی بلیت وجود ندارد.");

        return new SpecialOperationPreview(
            "ManualIssueTicketWithWalletDebit",
            "صدور دستی بلیت با کسر از کیف پول",
            new[]
            {
                Row("کاربر", FormatUser(context.User)),
                Row("رویداد", context.Event.Title),
                Row("جنسیت/ظرفیت", $"{GenderLabel(context.Profile.Gender)}، ظرفیت باقی‌مانده {remainingCapacity:N0}"),
                Row("مبلغ بلیت", $"{reportingPrice:N0} ریال ایران", isDanger: true),
                Row("موجودی قبل", $"{context.Account.Balance:N0} ریال ایران"),
                Row("موجودی بعد", $"{context.Account.Balance - reportingPrice:N0} ریال ایران", isDanger: true),
                Row("وضعیت بعد از اجرا", "یک بلیت صادر می‌شود و مبلغ از کیف پول همین کاربر کم می‌شود", isDanger: true)
            },
            context.Account.Balance < reportingPrice
                ? new[] { "موجودی کیف پول برای اجرای این عملیات کافی نیست." }
                : Array.Empty<string>());
    }

    public Task<SpecialOperationExecuteResult> ExecuteManualIssueTicketAsync(MockUser currentUser, ManualIssueTicketInput input, CancellationToken cancellationToken = default)
        => ExecuteWithLogAsync(
            currentUser,
            "manualIssueTicketWithWalletDebit",
            "ManualIssueTicketWithWalletDebit",
            input.IdempotencyKey,
            input.Reason,
            input.SupportTicketNumber,
            input,
            async ct => await PreviewManualIssueTicketAsync(currentUser, input, ct),
            async (operation, ct) =>
            {
                var context = await LoadManualIssueContextAsync(input.UserId, input.EventId, ct);
                var account = context.Account ?? throw new InvalidOperationException("کاربر کیف پول ندارد و امکان کسر مبلغ برای صدور دستی بلیت وجود ندارد.");
                var price = context.Event.GetTicketPriceForGender(context.Profile.Gender);
                var currencyCode = context.Event.GetTicketCurrencyForGender(context.Profile.Gender);
                var exchangeRate = await _exchangeRates.GetActiveRateToIrrAsync(currencyCode, DateTime.UtcNow, ct);
                var platformCommission = price * context.Event.EventPlannerCommissionPercent / 100m;
                var order = new TicketOrder(
                    context.Event,
                    context.User,
                    price,
                    0,
                    price,
                    platformCommission,
                    context.Event.PaymentCollectionMethod,
                    currencyCode,
                    exchangeRate.Rate,
                    exchangeRate.CapturedAtUtc,
                    exchangeRate.ExchangeRateId,
                    null,
                    TicketOrderPaymentStatus.Paid,
                    TicketOrderStatus.Confirmed,
                    $"Special operation #{operation.Id}");
                order.MarkPaid(currentUser.Id);

                var ticket = context.Event.SellTicket(order, context.User, context.Profile, price);
                ticket.CaptureExchangeRate(exchangeRate.Rate, exchangeRate.CapturedAtUtc, exchangeRate.ExchangeRateId);
                account.Debit(
                    ticket.Price,
                    BalanceTransactionType.ManualTicketPurchaseDebit,
                    $"کسر کیف پول بابت صدور دستی بلیت رویداد {context.Event.Title}: {input.Reason}",
                    context.Event.Id,
                    OperationReferenceType,
                    operation.Id,
                    currentUser.Id,
                    ticket.CurrencyCode,
                    ticket.ReportingPriceIrr,
                    exchangeRate.Rate,
                    exchangeRate.CapturedAtUtc,
                    exchangeRate.ExchangeRateId,
                    order);

                _db.TicketOrders.Add(order);
                await _db.SaveChangesAsync(ct);
                var walletTransaction = LatestTransaction(account);
                return new OperationMutationResult(
                    $"بلیت #{ticket.Id} برای کاربر صادر شد و مبلغ از کیف پول کم شد.",
                    input.UserId,
                    ticket.Id,
                    order.Id,
                    context.Event.Id,
                    walletTransaction.Id == 0 ? null : walletTransaction.Id,
                    ticket.ReportingPriceIrr,
                    ticket.CurrencyCode,
                    new
                    {
                        ticketId = ticket.Id,
                        orderId = order.Id,
                        eventId = context.Event.Id,
                        userId = input.UserId,
                        walletTransactionId = walletTransaction.Id,
                        amount = ticket.Price,
                        reportingAmountIrr = ticket.ReportingPriceIrr
                    });
            },
            cancellationToken);

    public async Task<SpecialOperationPreview> PreviewManualWalletCreditAsync(MockUser currentUser, ManualWalletAdjustmentInput input, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "manualWalletCredit", cancellationToken);
        return await PreviewWalletAdjustmentAsync(input, true, cancellationToken);
    }

    public Task<SpecialOperationExecuteResult> ExecuteManualWalletCreditAsync(MockUser currentUser, ManualWalletAdjustmentInput input, CancellationToken cancellationToken = default)
        => ExecuteWalletAdjustmentAsync(currentUser, input, true, cancellationToken);

    public async Task<SpecialOperationPreview> PreviewManualWalletDebitAsync(MockUser currentUser, ManualWalletAdjustmentInput input, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "manualWalletDebit", cancellationToken);
        return await PreviewWalletAdjustmentAsync(input, false, cancellationToken);
    }

    public Task<SpecialOperationExecuteResult> ExecuteManualWalletDebitAsync(MockUser currentUser, ManualWalletAdjustmentInput input, CancellationToken cancellationToken = default)
        => ExecuteWalletAdjustmentAsync(currentUser, input, false, cancellationToken);

    public async Task<ReportedUserListResult> ListReportedUsersAsync(MockUser currentUser, UserReportListFilter filter, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "userReportsView", cancellationToken);

        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 5, 100);
        var reportsQuery = _db.ModerationReports.AsNoTracking();

        if (filter.Status is not null)
            reportsQuery = reportsQuery.Where(report => report.Status == filter.Status);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            var matchingUserIds = _db.Users
                .AsNoTracking()
                .Where(user =>
                    user.MobileNumber.Contains(term)
                    || user.Id.ToString() == term
                    || (user.Profile != null && user.Profile.DisplayName.Contains(term)))
                .Select(user => user.Id);
            reportsQuery = reportsQuery.Where(report => matchingUserIds.Contains(report.ReportedUserId));
        }

        var groupedQuery = reportsQuery
            .GroupBy(report => report.ReportedUserId)
            .Select(group => new
            {
                UserId = group.Key,
                TotalReports = group.Count(),
                OpenReports = group.Count(report => report.Status == ModerationReportStatus.Pending),
                LastReportedAtUtc = group.Max(report => report.CreatedAt)
            });

        if (filter.MinimumOpenReports is > 0)
            groupedQuery = groupedQuery.Where(item => item.OpenReports >= filter.MinimumOpenReports.Value);

        var totalCount = await groupedQuery.CountAsync(cancellationToken);
        var pageRows = await groupedQuery
            .OrderByDescending(item => item.OpenReports)
            .ThenByDescending(item => item.TotalReports)
            .ThenByDescending(item => item.LastReportedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var userIds = pageRows.Select(item => item.UserId).ToArray();
        var users = await _db.Users
            .AsNoTracking()
            .Include(user => user.Profile)
            .ThenInclude(profile => profile!.Images)
            .Where(user => userIds.Contains(user.Id))
            .ToDictionaryAsync(user => user.Id, cancellationToken);
        var latestReports = await _db.ModerationReports
            .AsNoTracking()
            .Where(report => userIds.Contains(report.ReportedUserId))
            .GroupBy(report => report.ReportedUserId)
            .Select(group => group.OrderByDescending(report => report.CreatedAt).First())
            .ToDictionaryAsync(report => report.ReportedUserId, cancellationToken);
        var activeRestrictions = await _db.UserRestrictions
            .AsNoTracking()
            .Where(restriction =>
                userIds.Contains(restriction.UserId)
                && restriction.RestrictionType == UserRestrictionType.TicketPurchase
                && restriction.IsActive
                && (restriction.ExpiresAtUtc == null || restriction.ExpiresAtUtc > DateTime.UtcNow))
            .ToDictionaryAsync(restriction => restriction.UserId, cancellationToken);

        var items = pageRows.Select(row =>
        {
            users.TryGetValue(row.UserId, out var user);
            latestReports.TryGetValue(row.UserId, out var latestReport);
            activeRestrictions.TryGetValue(row.UserId, out var restriction);
            return new ReportedUserSummaryItem
            {
                UserId = row.UserId,
                DisplayName = user is null ? $"کاربر #{row.UserId}" : DisplayName(user),
                MobileNumber = user?.MobileNumber ?? string.Empty,
                ProfileImageUrl = PrimaryImageUrl(user),
                TotalReports = row.TotalReports,
                OpenReports = row.OpenReports,
                LastReportedAtUtc = row.LastReportedAtUtc,
                LatestReason = latestReport?.Reason ?? ModerationReportReason.Other,
                LatestDescription = latestReport?.Description ?? string.Empty,
                HasActiveTicketPurchaseRestriction = restriction is not null,
                ActiveRestrictionReason = restriction?.Reason,
                ActiveRestrictionExpiresAtUtc = restriction?.ExpiresAtUtc
            };
        }).ToList();

        return new ReportedUserListResult
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ReportedUserDetails?> GetReportedUserDetailsAsync(MockUser currentUser, long userId, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "userReportsView", cancellationToken);

        var user = await _db.Users
            .AsNoTracking()
            .Include(item => item.Profile)
            .ThenInclude(profile => profile!.Images)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken);
        if (user is null)
            return null;

        var activeRestriction = await _db.UserRestrictions
            .AsNoTracking()
            .FirstOrDefaultAsync(
                restriction =>
                    restriction.UserId == userId
                    && restriction.RestrictionType == UserRestrictionType.TicketPurchase
                    && restriction.IsActive
                    && (restriction.ExpiresAtUtc == null || restriction.ExpiresAtUtc > DateTime.UtcNow),
                cancellationToken);

        var reports = await _db.ModerationReports
            .AsNoTracking()
            .Include(report => report.ReporterUser)
            .ThenInclude(reporter => reporter.Profile)
            .Include(report => report.DatingEvent)
            .Include(report => report.ReviewedByAdminUser)
            .ThenInclude(reviewer => reviewer!.Profile)
            .Where(report => report.ReportedUserId == userId)
            .OrderByDescending(report => report.Status == ModerationReportStatus.Pending)
            .ThenByDescending(report => report.CreatedAt)
            .Take(80)
            .Select(report => new UserReportDetailItem
            {
                Id = report.Id,
                ReporterUserId = report.ReporterUserId,
                ReporterName = report.ReporterUser.Profile == null ? report.ReporterUser.MobileNumber : report.ReporterUser.Profile.DisplayName,
                EventId = report.DatingEventId,
                EventTitle = report.DatingEvent == null ? null : report.DatingEvent.Title,
                Reason = report.Reason,
                Description = report.Description,
                Status = report.Status,
                AdminReviewNote = report.AdminReviewNote,
                ReviewedByName = report.ReviewedByAdminUser == null
                    ? null
                    : report.ReviewedByAdminUser.Profile == null
                        ? report.ReviewedByAdminUser.MobileNumber
                        : report.ReviewedByAdminUser.Profile.DisplayName,
                ReviewedAtUtc = report.ReviewedAt,
                CreatedAtUtc = report.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new ReportedUserDetails
        {
            UserId = user.Id,
            DisplayName = DisplayName(user),
            MobileNumber = user.MobileNumber,
            ProfileImageUrl = PrimaryImageUrl(user),
            IsUserActive = user.IsActive,
            HasActiveTicketPurchaseRestriction = activeRestriction is not null,
            ActiveRestrictionReason = activeRestriction?.Reason,
            ActiveRestrictionExpiresAtUtc = activeRestriction?.ExpiresAtUtc,
            Reports = reports
        };
    }

    public async Task<SpecialOperationExecuteResult> ReviewUserReportAsync(MockUser currentUser, ReviewUserReportInput input, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "userReportsReview", cancellationToken);
        var idempotencyKey = EnsureIdempotencyKey(input.IdempotencyKey);
        var existing = await _db.SpecialOperationLogs.AsNoTracking().FirstOrDefaultAsync(item => item.IdempotencyKey == idempotencyKey, cancellationToken);
        if (existing is not null)
        {
            if (existing.Status == "Succeeded")
                return new SpecialOperationExecuteResult(existing.Id, existing.OperationType, "این بررسی قبلا با همین کلید ثبت شده است.", true);

            throw new InvalidOperationException("برای این بررسی قبلا یک تلاش ثبت شده است. لطفا صفحه را تازه‌سازی کنید.");
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        var report = await _db.ModerationReports
            .Include(item => item.ReportedUser)
            .FirstOrDefaultAsync(item => item.Id == input.ReportId, cancellationToken)
            ?? throw new InvalidOperationException("گزارش پیدا نشد.");
        var oldStatus = report.Status;
        report.Review(input.Status, currentUser.Id, input.Note);

        var operation = new SpecialOperationLog(
            "UserReportReviewed",
            currentUser.Id,
            report.ReportedUserId,
            string.IsNullOrWhiteSpace(input.Note) ? "بررسی گزارش کاربر" : input.Note,
            idempotencyKey,
            Serialize(input),
            null,
            relatedEventId: report.DatingEventId);
        operation.MarkSucceeded(Serialize(new { reportId = report.Id, oldStatus, newStatus = report.Status }));
        _db.SpecialOperationLogs.Add(operation);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new SpecialOperationExecuteResult(operation.Id, operation.OperationType, "وضعیت گزارش به‌روزرسانی شد.", false);
    }

    public async Task<SpecialOperationPreview> PreviewRestrictTicketPurchaseAsync(MockUser currentUser, RestrictTicketPurchaseInput input, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "userReportsRestrictTicketPurchase", cancellationToken);
        var details = await GetReportedUserDetailsAsync(currentUser, input.UserId, cancellationToken)
            ?? throw new InvalidOperationException("کاربر پیدا نشد.");

        return new SpecialOperationPreview(
            "UserTicketPurchaseRestricted",
            "محدود کردن امکان خرید بلیت",
            new[]
            {
                Row("کاربر", $"{details.DisplayName} - {details.MobileNumber} (#{details.UserId})"),
                Row("تعداد گزارش‌ها", $"{details.Reports.Count:N0} گزارش"),
                Row("گزارش‌های باز", $"{details.Reports.Count(report => report.Status == ModerationReportStatus.Pending):N0} گزارش باز", isDanger: true),
                Row("اثر عملیات", "کاربر همچنان می‌تواند وارد حساب شود، اما امکان خرید بلیت برای او بسته می‌شود", isDanger: true),
                Row("پیام کاربر", "یک اعلان داخلی درباره محدودیت و تماس با پشتیبانی برای کاربر ثبت می‌شود", isDanger: true)
            },
            details.HasActiveTicketPurchaseRestriction
                ? new[] { "این کاربر همین حالا محدودیت فعال خرید بلیت دارد." }
                : Array.Empty<string>());
    }

    public Task<SpecialOperationExecuteResult> ExecuteRestrictTicketPurchaseAsync(MockUser currentUser, RestrictTicketPurchaseInput input, CancellationToken cancellationToken = default)
        => ExecuteWithLogAsync(
            currentUser,
            "userReportsRestrictTicketPurchase",
            "UserTicketPurchaseRestricted",
            input.IdempotencyKey,
            input.Reason,
            input.SupportTicketNumber,
            input,
            async ct => await PreviewRestrictTicketPurchaseAsync(currentUser, input, ct),
            async (operation, ct) =>
            {
                var actor = await LoadUserAsync(currentUser.Id, ct);
                var target = await LoadUserAsync(input.UserId, ct);
                var activeRestriction = await _db.UserRestrictions
                    .FirstOrDefaultAsync(
                        restriction =>
                            restriction.UserId == input.UserId
                            && restriction.RestrictionType == UserRestrictionType.TicketPurchase
                            && restriction.IsActive
                            && (restriction.ExpiresAtUtc == null || restriction.ExpiresAtUtc > DateTime.UtcNow),
                        ct);
                if (activeRestriction is not null)
                    throw new InvalidOperationException("این کاربر همین حالا محدودیت فعال خرید بلیت دارد.");

                var restriction = new UserRestriction(target, UserRestrictionType.TicketPurchase, input.Reason, actor, input.ExpiresAtUtc);
                _db.UserRestrictions.Add(restriction);
                var notification = CreateUserNotification(
                    actor,
                    target,
                    "محدودیت خرید بلیت",
                    "حساب شما به دلیل گزارش‌های دریافت‌شده در حال بررسی قرار گرفت و امکان خرید بلیت برای شما موقتاً محدود شد. برای پیگیری با پشتیبانی تماس بگیرید.",
                    operation.Id);
                _db.Notifications.Add(notification);
                await _db.SaveChangesAsync(ct);

                return new OperationMutationResult(
                    "امکان خرید بلیت برای کاربر محدود شد و اعلان داخلی ثبت شد.",
                    input.UserId,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new
                    {
                        userId = input.UserId,
                        restrictionId = restriction.Id,
                        notificationId = notification.Id,
                        expiresAtUtc = restriction.ExpiresAtUtc
                    });
            },
            cancellationToken);

    public async Task<SpecialOperationExecuteResult> RemoveTicketPurchaseRestrictionAsync(MockUser currentUser, RemoveTicketPurchaseRestrictionInput input, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "userReportsRemoveRestriction", cancellationToken);
        var idempotencyKey = EnsureIdempotencyKey(input.IdempotencyKey);
        var existing = await _db.SpecialOperationLogs.AsNoTracking().FirstOrDefaultAsync(item => item.IdempotencyKey == idempotencyKey, cancellationToken);
        if (existing is not null)
        {
            if (existing.Status == "Succeeded")
                return new SpecialOperationExecuteResult(existing.Id, existing.OperationType, "این عملیات قبلا با همین کلید اجرا شده است.", true);

            throw new InvalidOperationException("برای این عملیات قبلا یک تلاش ثبت شده است. لطفا صفحه را تازه‌سازی کنید.");
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        var actor = await LoadUserAsync(currentUser.Id, cancellationToken);
        var target = await LoadUserAsync(input.UserId, cancellationToken);
        var restriction = await _db.UserRestrictions.FirstOrDefaultAsync(
            item =>
                item.UserId == input.UserId
                && item.RestrictionType == UserRestrictionType.TicketPurchase
                && item.IsActive
                && (item.ExpiresAtUtc == null || item.ExpiresAtUtc > DateTime.UtcNow),
            cancellationToken)
            ?? throw new InvalidOperationException("این کاربر محدودیت فعال خرید بلیت ندارد.");

        restriction.Remove(actor, input.Reason);
        var notification = CreateUserNotification(
            actor,
            target,
            "رفع محدودیت خرید بلیت",
            "محدودیت خرید بلیت حساب شما برداشته شد. می‌توانید طبق قوانین راندوو دوباره برای رویدادها بلیت تهیه کنید.",
            null);
        _db.Notifications.Add(notification);
        var operation = new SpecialOperationLog(
            "UserTicketPurchaseRestrictionRemoved",
            currentUser.Id,
            input.UserId,
            input.Reason,
            idempotencyKey,
            Serialize(input),
            null,
            input.SupportTicketNumber);
        _db.SpecialOperationLogs.Add(operation);
        await _db.SaveChangesAsync(cancellationToken);
        operation.MarkSucceeded(Serialize(new { userId = input.UserId, restrictionId = restriction.Id, notificationId = notification.Id }));
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return new SpecialOperationExecuteResult(operation.Id, operation.OperationType, "محدودیت خرید بلیت برداشته شد و اعلان داخلی ثبت شد.", false);
    }

    public Task<SpecialOperationExecuteResult> SendUserReportWarningAsync(MockUser currentUser, SendUserReportWarningInput input, CancellationToken cancellationToken = default)
        => ExecuteWithLogAsync(
            currentUser,
            "userReportsSendWarning",
            "UserWarningNotificationSent",
            input.IdempotencyKey,
            input.Message,
            input.SupportTicketNumber,
            input,
            async ct =>
            {
                var target = await LoadUserAsync(input.UserId, ct);
                return new SpecialOperationPreview(
                    "UserWarningNotificationSent",
                    "ارسال هشدار به کاربر ریپورت‌شده",
                    new[]
                    {
                        Row("کاربر", FormatUser(target)),
                        Row("متن پیام", input.Message, isDanger: true),
                        Row("اثر عملیات", "فقط یک اعلان داخلی برای کاربر ثبت می‌شود و حساب یا خریدهای او تغییر نمی‌کند")
                    },
                    Array.Empty<string>());
            },
            async (operation, ct) =>
            {
                var actor = await LoadUserAsync(currentUser.Id, ct);
                var target = await LoadUserAsync(input.UserId, ct);
                var notification = CreateUserNotification(actor, target, "هشدار درباره گزارش‌های دریافتی", input.Message, operation.Id);
                _db.Notifications.Add(notification);
                await _db.SaveChangesAsync(ct);

                return new OperationMutationResult(
                    "هشدار برای کاربر ثبت شد.",
                    input.UserId,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new { userId = input.UserId, notificationId = notification.Id });
            },
            cancellationToken);

    public Task<SpecialOperationExecuteResult> SendUserReportNotificationAsync(MockUser currentUser, SendUserReportNotificationInput input, CancellationToken cancellationToken = default)
        => ExecuteWithLogAsync(
            currentUser,
            "userReportsSendNotification",
            "UserReportNotificationSent",
            input.IdempotencyKey,
            input.Title,
            input.SupportTicketNumber,
            input,
            async ct =>
            {
                var target = await LoadUserAsync(input.UserId, ct);
                return new SpecialOperationPreview(
                    "UserReportNotificationSent",
                    "ارسال نوتیفیکیشن به کاربر ریپورت‌شده",
                    new[]
                    {
                        Row("کاربر", FormatUser(target)),
                        Row("عنوان", input.Title),
                        Row("متن پیام", input.Message, isDanger: true),
                        Row("اثر عملیات", "فقط یک اعلان داخلی برای کاربر ثبت می‌شود و وضعیت حساب یا خرید او تغییر نمی‌کند")
                    },
                    Array.Empty<string>());
            },
            async (operation, ct) =>
            {
                var actor = await LoadUserAsync(currentUser.Id, ct);
                var target = await LoadUserAsync(input.UserId, ct);
                var notification = CreateUserNotification(actor, target, input.Title, input.Message, operation.Id);
                _db.Notifications.Add(notification);
                await _db.SaveChangesAsync(ct);

                return new OperationMutationResult(
                    "نوتیفیکیشن برای کاربر ثبت شد.",
                    input.UserId,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new { userId = input.UserId, notificationId = notification.Id });
            },
            cancellationToken);

    public Task<SpecialOperationExecuteResult> DeactivateReportedUserAsync(MockUser currentUser, DeactivateReportedUserInput input, CancellationToken cancellationToken = default)
        => ExecuteWithLogAsync(
            currentUser,
            "userReportsDeactivateUser",
            "ReportedUserDeactivated",
            input.IdempotencyKey,
            input.Reason,
            input.SupportTicketNumber,
            input,
            async ct =>
            {
                var target = await LoadUserAsync(input.UserId, ct);
                return new SpecialOperationPreview(
                    "ReportedUserDeactivated",
                    "غیرفعال کردن حساب کاربر ریپورت‌شده",
                    new[]
                    {
                        Row("کاربر", FormatUser(target)),
                        Row("وضعیت فعلی", target.IsActive ? "فعال" : "غیرفعال"),
                        Row("دلیل عملیات", input.Reason, isDanger: true),
                        Row("پیام کاربر", input.NotificationMessage, isDanger: true),
                        Row("اثر عملیات", "حساب کاربر غیرفعال می‌شود و یک اعلان داخلی با دلیل کلی برای او ثبت می‌شود", isDanger: true)
                    },
                    target.IsActive
                        ? Array.Empty<string>()
                        : new[] { "این حساب همین حالا غیرفعال است." });
            },
            async (operation, ct) =>
            {
                if (input.UserId == currentUser.Id)
                    throw new InvalidOperationException("امکان غیرفعال کردن حساب جاری از این صفحه وجود ندارد.");

                var actor = await LoadUserAsync(currentUser.Id, ct);
                var target = await LoadUserAsync(input.UserId, ct);
                if (!target.IsActive)
                    throw new InvalidOperationException("این حساب همین حالا غیرفعال است.");

                if (target.Role is UserRole.Admin or UserRole.PlatformSupportTeam)
                    throw new InvalidOperationException("غیرفعال کردن حساب مدیر یا پشتیبان از صفحه کاربران ریپورت‌شده مجاز نیست.");

                target.Deactivate();
                var notification = CreateUserNotification(
                    actor,
                    target,
                    "غیرفعال شدن حساب",
                    input.NotificationMessage,
                    operation.Id);
                _db.Notifications.Add(notification);
                await _db.SaveChangesAsync(ct);

                return new OperationMutationResult(
                    "حساب کاربر غیرفعال شد و اعلان داخلی ثبت شد.",
                    input.UserId,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    new { userId = input.UserId, notificationId = notification.Id, isActive = target.IsActive });
            },
            cancellationToken);

    public async Task<IReadOnlyList<SpecialOperationHistoryItem>> ListHistoryAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        await EnsureAllowedAsync(currentUser, "viewAuditLog", cancellationToken);

        var users = _db.Users.AsNoTracking().Include(user => user.Profile);
        return await (
                from operation in _db.SpecialOperationLogs.AsNoTracking()
                join performer in users on operation.PerformedByUserId equals performer.Id
                join targetUser in users on operation.TargetUserId equals targetUser.Id into targetUsers
                from target in targetUsers.DefaultIfEmpty()
                orderby operation.CreatedAt descending
                select new SpecialOperationHistoryItem
                {
                    Id = operation.Id,
                    OperationType = operation.OperationType,
                    Status = operation.Status,
                    PerformedByName = performer.Profile == null ? performer.MobileNumber : performer.Profile.DisplayName,
                    TargetUserName = target == null ? null : target.Profile == null ? target.MobileNumber : target.Profile.DisplayName,
                    TargetUserId = operation.TargetUserId,
                    RelatedTicketId = operation.RelatedTicketId,
                    RelatedOrderId = operation.RelatedOrderId,
                    RelatedEventId = operation.RelatedEventId,
                    RelatedWalletTransactionId = operation.RelatedWalletTransactionId,
                    Amount = operation.Amount,
                    CurrencyCode = operation.CurrencyCode,
                    Reason = operation.Reason,
                    SupportTicketNumber = operation.SupportTicketNumber,
                    FailureMessage = operation.FailureMessage,
                    CreatedAtUtc = operation.CreatedAt,
                    CompletedAtUtc = operation.CompletedAtUtc
                })
            .Take(80)
            .ToListAsync(cancellationToken);
    }

    private Task<SpecialOperationExecuteResult> ExecuteWalletAdjustmentAsync(MockUser currentUser, ManualWalletAdjustmentInput input, bool isCredit, CancellationToken cancellationToken)
    {
        var permission = isCredit ? "manualWalletCredit" : "manualWalletDebit";
        var operationType = isCredit ? "ManualWalletCredit" : "ManualWalletDebit";
        var transactionType = isCredit ? BalanceTransactionType.ManualWalletCredit : BalanceTransactionType.ManualWalletDebit;

        return ExecuteWithLogAsync(
            currentUser,
            permission,
            operationType,
            input.IdempotencyKey,
            input.Reason,
            input.SupportTicketNumber,
            input,
            async ct => isCredit
                ? await PreviewManualWalletCreditAsync(currentUser, input, ct)
                : await PreviewManualWalletDebitAsync(currentUser, input, ct),
            async (operation, ct) =>
            {
                var user = await _db.Users
                    .Include(item => item.Profile)
                    .FirstOrDefaultAsync(item => item.Id == input.UserId, ct)
                    ?? throw new InvalidOperationException("کاربر پیدا نشد.");
                var account = await GetOrCreateBalanceAccountAsync(user, ct);

                if (isCredit)
                {
                    account.Credit(
                        input.Amount,
                        transactionType,
                        $"شارژ دستی کیف پول: {input.Reason}",
                        null,
                        OperationReferenceType,
                        operation.Id,
                        currentUser.Id);
                }
                else
                {
                    account.Debit(
                        input.Amount,
                        transactionType,
                        $"کسر دستی کیف پول: {input.Reason}",
                        null,
                        OperationReferenceType,
                        operation.Id,
                        currentUser.Id);
                }

                await _db.SaveChangesAsync(ct);
                var walletTransaction = LatestTransaction(account);
                return new OperationMutationResult(
                    isCredit ? "کیف پول کاربر شارژ شد." : "مبلغ از کیف پول کاربر کسر شد.",
                    input.UserId,
                    null,
                    null,
                    null,
                    walletTransaction.Id == 0 ? null : walletTransaction.Id,
                    input.Amount,
                    "IRR",
                    new
                    {
                        userId = input.UserId,
                        amount = input.Amount,
                        walletTransactionId = walletTransaction.Id,
                        direction = isCredit ? "credit" : "debit",
                        balance = account.Balance
                    });
            },
            cancellationToken);
    }

    private async Task<SpecialOperationPreview> PreviewWalletAdjustmentAsync(ManualWalletAdjustmentInput input, bool isCredit, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .AsNoTracking()
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == input.UserId, cancellationToken)
            ?? throw new InvalidOperationException("کاربر پیدا نشد.");
        var account = await _db.BalanceAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == input.UserId, cancellationToken);
        var before = account?.Balance ?? 0m;
        var after = isCredit ? before + input.Amount : before - input.Amount;

        return new SpecialOperationPreview(
            isCredit ? "ManualWalletCredit" : "ManualWalletDebit",
            isCredit ? "شارژ دستی کیف پول" : "کسر دستی کیف پول",
            new[]
            {
                Row("کاربر", FormatUser(user)),
                Row("مبلغ عملیات", $"{input.Amount:N0} ریال ایران", isDanger: true),
                Row("موجودی قبل", $"{before:N0} ریال ایران"),
                Row("موجودی بعد", $"{after:N0} ریال ایران", isDanger: true),
                Row("وضعیت بعد از اجرا", isCredit ? "یک تراکنش شارژ دستی ثبت می‌شود" : "یک تراکنش کسر دستی ثبت می‌شود", isDanger: true)
            },
            !isCredit && before < input.Amount
                ? new[] { "موجودی کیف پول برای کسر دستی کافی نیست." }
                : Array.Empty<string>());
    }

    private async Task<SpecialOperationExecuteResult> ExecuteWithLogAsync<TInput>(
        MockUser currentUser,
        string permission,
        string operationType,
        string idempotencyKey,
        string reason,
        string? supportTicketNumber,
        TInput input,
        Func<CancellationToken, Task<SpecialOperationPreview>> previewFactory,
        Func<SpecialOperationLog, CancellationToken, Task<OperationMutationResult>> mutation,
        CancellationToken cancellationToken)
    {
        await EnsureAllowedAsync(currentUser, permission, cancellationToken);
        idempotencyKey = EnsureIdempotencyKey(idempotencyKey);
        var existing = await _db.SpecialOperationLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.IdempotencyKey == idempotencyKey, cancellationToken);
        if (existing is not null)
        {
            if (existing.Status == "Succeeded")
                return new SpecialOperationExecuteResult(existing.Id, existing.OperationType, "این عملیات قبلا با همین کلید اجرا شده است و دوباره اعمال نشد.", true);

            throw new InvalidOperationException("برای این عملیات قبلا یک تلاش ثبت شده است. لطفا فرم را تازه‌سازی کنید و دوباره تلاش کنید.");
        }

        SpecialOperationPreview? preview = null;
        try
        {
            preview = await previewFactory(cancellationToken);
            await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            var operation = new SpecialOperationLog(
                operationType,
                currentUser.Id,
                null,
                reason,
                idempotencyKey,
                Serialize(input),
                Serialize(preview),
                supportTicketNumber);

            _db.SpecialOperationLogs.Add(operation);
            await _db.SaveChangesAsync(cancellationToken);

            var result = await mutation(operation, cancellationToken);
            operation = await _db.SpecialOperationLogs.FirstAsync(item => item.Id == operation.Id, cancellationToken);
            SetOperationRelations(operation, result);
            operation.MarkSucceeded(Serialize(result.ResultPayload), result.WalletTransactionId);
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return new SpecialOperationExecuteResult(operation.Id, operation.OperationType, result.Message, false);
        }
        catch (Exception ex)
        {
            await TryWriteFailureLogAsync(currentUser, operationType, idempotencyKey, reason, supportTicketNumber, input, preview, ex, cancellationToken);
            throw;
        }
    }

    private async Task TryWriteFailureLogAsync<TInput>(
        MockUser currentUser,
        string operationType,
        string idempotencyKey,
        string reason,
        string? supportTicketNumber,
        TInput input,
        SpecialOperationPreview? preview,
        Exception exception,
        CancellationToken cancellationToken)
    {
        try
        {
            _db.ChangeTracker.Clear();
            if (await _db.SpecialOperationLogs.AnyAsync(item => item.IdempotencyKey == idempotencyKey, cancellationToken))
                return;

            var failureLog = new SpecialOperationLog(
                operationType,
                currentUser.Id,
                null,
                SafeReason(reason),
                idempotencyKey,
                Serialize(input),
                preview is null ? null : Serialize(preview),
                supportTicketNumber);
            failureLog.MarkFailed(exception.Message);
            _db.SpecialOperationLogs.Add(failureLog);
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            _db.ChangeTracker.Clear();
        }
    }

    private static void SetOperationRelations(SpecialOperationLog operation, OperationMutationResult result)
    {
        operation.AttachResultReferences(
            result.TargetUserId,
            result.TicketId,
            result.OrderId,
            result.EventId,
            result.Amount,
            result.CurrencyCode);
    }

    private async Task<EventTicket> LoadRefundableTicketAsync(long ticketId, CancellationToken cancellationToken)
    {
        var ticket = await _db.EventTickets
            .Include(item => item.DatingEvent)
            .Include(item => item.User)
            .ThenInclude(user => user.Profile)
            .Include(item => item.TicketOrder)
            .ThenInclude(order => order.BuyerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.TicketOrder)
            .ThenInclude(order => order.Tickets)
            .FirstOrDefaultAsync(item => item.Id == ticketId, cancellationToken)
            ?? throw new InvalidOperationException("بلیت پیدا نشد.");

        if (ticket.IsRefunded || ticket.IsRemoved)
            throw new InvalidOperationException("این بلیت قبلا کنسل، حذف یا Refund شده است.");

        return ticket;
    }

    private async Task<ManualIssueContext> LoadManualIssueContextAsync(long userId, long eventId, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("کاربر پیدا نشد.");
        var profile = user.Profile ?? throw new InvalidOperationException("کاربر پروفایل کامل ندارد.");
        var datingEvent = await _db.DatingEvents
            .Include(item => item.Tickets)
            .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد پیدا نشد.");
        var account = await _db.BalanceAccounts
            .Include(item => item.Transactions)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        return new ManualIssueContext(user, profile, datingEvent, account);
    }

    private async Task<BalanceAccount> GetOrCreateBalanceAccountAsync(User user, CancellationToken cancellationToken)
    {
        var account = await _db.BalanceAccounts
            .Include(item => item.Transactions)
            .FirstOrDefaultAsync(item => item.UserId == user.Id, cancellationToken);
        if (account is not null)
            return account;

        account = new BalanceAccount(user);
        _db.BalanceAccounts.Add(account);
        return account;
    }

    private async Task<User> LoadUserAsync(long userId, CancellationToken cancellationToken)
    {
        return await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("کاربر پیدا نشد.");
    }

    private async Task EnsureAllowedAsync(MockUser user, string action, CancellationToken cancellationToken)
    {
        if (user.Role is not (AdminRole.Admin or AdminRole.SupportTeam))
            throw new InvalidOperationException("فقط مدیر یا پشتیبان می‌تواند از عملیات ویژه استفاده کند.");

        if (!await _permissions.IsAllowedAsync(user, PermissionEntity, action, cancellationToken))
            throw new InvalidOperationException("شما به این عملیات ویژه دسترسی ندارید.");
    }

    private static BalanceTransaction LatestTransaction(BalanceAccount account)
        => account.Transactions.OrderByDescending(item => item.Id).ThenByDescending(item => item.CreatedAt).First();

    private static int RemainingCapacity(DatingEvent datingEvent, Gender gender)
    {
        var capacity = gender == Gender.Male ? datingEvent.MaleCapacity : datingEvent.FemaleCapacity;
        var sold = datingEvent.Tickets.Count(item => !item.IsRefunded && !item.IsRemoved && item.Gender == gender);
        return capacity - sold;
    }

    private static string FormatUser(User user)
        => user.Profile is null ? $"{user.MobileNumber} (#{user.Id})" : $"{user.Profile.DisplayName} - {user.MobileNumber} (#{user.Id})";

    private static string DisplayName(User user)
        => user.Profile is null ? user.MobileNumber : user.Profile.DisplayName;

    private static string? PrimaryImageUrl(User? user)
        => user?.Profile?.Images
            .OrderByDescending(image => image.IsPrimary)
            .ThenBy(image => image.DisplayOrder)
            .Select(image => image.ImageUrl)
            .FirstOrDefault();

    private static Notification CreateUserNotification(User actor, User target, string title, string body, long? referenceId)
    {
        var notification = new Notification(
            actor,
            NotificationType.AdminToUser,
            title,
            body,
            NotificationPriority.Important,
            requiresApproval: false,
            referenceType: OperationReferenceType,
            referenceId: referenceId);
        notification.AddRecipient(target, NotificationDeliveryChannel.InApp);
        return notification;
    }

    private static string GenderLabel(Gender gender) => gender switch
    {
        Gender.Male => "آقا",
        Gender.Female => "خانم",
        _ => "نامشخص"
    };

    private static decimal ConvertToIrr(decimal amount, decimal rate)
        => Math.Round(amount * rate, 0, MidpointRounding.AwayFromZero);

    private static SpecialOperationPreviewRow Row(string label, string value, bool isDanger = false)
        => new(label, value, isDanger);

    private static string EnsureIdempotencyKey(string? value)
        => string.IsNullOrWhiteSpace(value) ? Guid.NewGuid().ToString("N") : value.Trim();

    private static string SafeReason(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Length < 5
            ? "Operation failed before reason validation."
            : value.Trim();

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, JsonOptions);

    private sealed record ManualIssueContext(User User, UserProfile Profile, DatingEvent Event, BalanceAccount? Account);

    private sealed record OperationMutationResult(
        string Message,
        long? TargetUserId,
        long? TicketId,
        long? OrderId,
        long? EventId,
        long? WalletTransactionId,
        decimal? Amount,
        string? CurrencyCode,
        object ResultPayload);
}
