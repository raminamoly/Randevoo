using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseFinanceApiClient : IFinanceApiClient
{
    private readonly RandevooDbContext _db;

    public DatabaseFinanceApiClient(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<PlannerFinanceDashboard> GetPlannerFinanceAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        EnsurePlanner(currentUser);

        var profile = await _db.EventPlannerProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == currentUser.Id, cancellationToken);
        var settlementCurrencyCode = profile?.SettlementCurrencyCode ?? "IRR";

        var balanceRecord = await _db.BalanceAccounts
            .Where(account => account.UserId == currentUser.Id)
            .Select(account => new { Balance = (decimal?)account.Balance, account.ReportingCurrencyCode })
            .FirstOrDefaultAsync(cancellationToken);
        var balance = balanceRecord?.Balance ?? 0m;

        var eventLookup = await _db.DatingEvents
            .Where(item => item.EventPlannerUserId == currentUser.Id)
            .Select(item => new
            {
                item.Id,
                item.Title,
                item.DateTimeStart,
                item.EventPlannerCommissionPercent
            })
            .ToDictionaryAsync(item => item.Id, cancellationToken);

        var transactions = await _db.BalanceTransactions
            .Where(transaction => transaction.UserId == currentUser.Id)
            .Where(transaction =>
                transaction.Type == BalanceTransactionType.EventPlannerIncome
                || transaction.Type == BalanceTransactionType.EventSettlementCredit
                || transaction.Type == BalanceTransactionType.PlatformCommission
                || transaction.Type == BalanceTransactionType.PlannerWithdrawalPayout)
            .OrderByDescending(transaction => transaction.CreatedAt)
            .Select(transaction => new PlannerCommissionTransactionItem
            {
                Id = transaction.Id,
                EventId = transaction.DatingEventId,
                Amount = transaction.Amount,
                CurrencyCode = transaction.CurrencyCode,
                ReportingAmountIrr = transaction.ReportingAmountIrr,
                ExchangeRateToIrr = transaction.ExchangeRateToIrr,
                Type = transaction.Type,
                Description = transaction.Description,
                CreatedAtUtc = transaction.CreatedAt
            })
            .ToListAsync(cancellationToken);

        foreach (var transaction in transactions)
        {
            if (transaction.EventId is long eventId && eventLookup.TryGetValue(eventId, out var datingEvent))
                transaction.EventTitle = datingEvent.Title;
        }

        var ticketSummaries = await _db.EventTickets
            .Where(ticket => !ticket.IsRefunded && !ticket.IsRemoved && ticket.DatingEvent.EventPlannerUserId == currentUser.Id)
            .GroupBy(ticket => new
            {
                ticket.DatingEventId,
                ticket.DatingEvent.Title,
                ticket.DatingEvent.DateTimeStart,
                ticket.DatingEvent.EventPlannerCommissionPercent
            })
            .Select(group => new PlannerCommissionEventSummary
            {
                EventId = group.Key.DatingEventId,
                EventTitle = group.Key.Title,
                StartAtUtc = group.Key.DateTimeStart,
                PlannerCommissionPercent = 100m - group.Key.EventPlannerCommissionPercent,
                TicketsSold = group.Count(),
                GrossTicketSales = group.Sum(ticket => ticket.Price),
                CurrencyCode = group.Select(ticket => ticket.CurrencyCode).FirstOrDefault() ?? "IRR",
                GrossTicketSalesIrr = group.Sum(ticket => ticket.ReportingPriceIrr),
                PlannerIncome = group.Sum(ticket => ticket.Price * (100m - group.Key.EventPlannerCommissionPercent) / 100m),
                PlannerIncomeIrr = group.Sum(ticket => ticket.ReportingPriceIrr * (100m - group.Key.EventPlannerCommissionPercent) / 100m)
            })
            .OrderByDescending(item => item.StartAtUtc)
            .ToListAsync(cancellationToken);

        var withdrawals = await GetPlannerWithdrawalsAsync(currentUser.Id, cancellationToken);
        var totalCommissionIncome = transactions
            .Where(transaction => transaction.Type == BalanceTransactionType.EventPlannerIncome)
            .Concat(transactions.Where(transaction => transaction.Type == BalanceTransactionType.EventSettlementCredit))
            .Sum(transaction => transaction.ReportingAmountIrr);
        var paidWithdrawals = transactions
            .Where(transaction => transaction.Type == BalanceTransactionType.PlannerWithdrawalPayout)
            .Sum(transaction => Math.Abs(transaction.ReportingAmountIrr));
        var pendingWithdrawals = withdrawals
            .Where(item => item.Status == PlannerWithdrawalRequestStatus.Pending)
            .Sum(item => item.ReportingAmountIrr);

        return new PlannerFinanceDashboard
        {
            CurrentBalance = balance,
            ReportingCurrencyCode = balanceRecord?.ReportingCurrencyCode ?? "IRR",
            SettlementCurrencyCode = settlementCurrencyCode,
            TotalCommissionIncome = totalCommissionIncome,
            TotalCommissionIncomeIrr = totalCommissionIncome,
            PendingWithdrawalAmount = pendingWithdrawals,
            PaidWithdrawalAmount = paidWithdrawals,
            AvailableWithdrawalAmount = Math.Max(0m, balance - pendingWithdrawals),
            Events = ticketSummaries,
            Transactions = transactions,
            Withdrawals = withdrawals
        };
    }

    public async Task RequestWithdrawalAsync(MockUser currentUser, decimal amount, CancellationToken cancellationToken = default)
    {
        EnsurePlanner(currentUser);

        if (amount <= 0)
            throw new InvalidOperationException("مبلغ برداشت باید بیشتر از صفر باشد.");

        var planner = await _db.Users.FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("برگزارکننده پیدا نشد.");
        var profile = await _db.EventPlannerProfiles.FirstOrDefaultAsync(item => item.UserId == currentUser.Id, cancellationToken);
        var settlementCurrencyCode = profile?.SettlementCurrencyCode ?? "IRR";
        var rate = await GetActiveRateToIrrAsync(settlementCurrencyCode, DateTime.UtcNow, cancellationToken);
        var reportingAmountIrr = ConvertToIrr(amount, rate.Rate);
        var dashboard = await GetPlannerFinanceAsync(currentUser, cancellationToken);

        if (reportingAmountIrr > dashboard.AvailableWithdrawalAmount)
            throw new InvalidOperationException("مبلغ درخواست بیشتر از موجودی قابل برداشت است.");

        _db.PlannerWithdrawalRequests.Add(new PlannerWithdrawalRequest(
            planner,
            amount,
            settlementCurrencyCode,
            reportingAmountIrr,
            rate.Rate,
            rate.CapturedAtUtc,
            rate.ExchangeRateId));
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<PlannerWithdrawalRequestItem>> GetWithdrawalRequestsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        return await MapWithdrawalRequests(_db.PlannerWithdrawalRequests)
            .OrderBy(item => item.Status)
            .ThenByDescending(item => item.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task ConfirmWithdrawalAsync(MockUser currentUser, long requestId, string? reviewNote, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var request = await _db.PlannerWithdrawalRequests
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.Id == requestId, cancellationToken)
            ?? throw new InvalidOperationException("درخواست تسویه پیدا نشد.");

        if (request.Status != PlannerWithdrawalRequestStatus.Pending)
            throw new InvalidOperationException("این درخواست قبلا بررسی شده است.");

        var admin = await _db.Users.FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("مدیر جاری پیدا نشد.");

        var account = await _db.BalanceAccounts
            .Include(item => item.Transactions)
            .FirstOrDefaultAsync(item => item.UserId == request.UserId, cancellationToken)
            ?? throw new InvalidOperationException("حساب مالی برگزارکننده پیدا نشد.");

        account.Debit(
            request.Amount,
            BalanceTransactionType.PlannerWithdrawalPayout,
            "تسویه درآمد برگزارکننده",
            null,
            nameof(PlannerWithdrawalRequest),
            request.Id,
            currentUser.Id,
            request.CurrencyCode,
            request.ReportingAmountIrr,
            request.ExchangeRateToIrr,
            request.ExchangeRateCapturedAtUtc,
            request.ExchangeRateId);
        request.Confirm(admin, reviewNote);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectWithdrawalAsync(MockUser currentUser, long requestId, string? reviewNote, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var request = await _db.PlannerWithdrawalRequests
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.Id == requestId, cancellationToken)
            ?? throw new InvalidOperationException("درخواست تسویه پیدا نشد.");

        if (request.Status != PlannerWithdrawalRequestStatus.Pending)
            throw new InvalidOperationException("این درخواست قبلا بررسی شده است.");

        var admin = await _db.Users.FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("مدیر جاری پیدا نشد.");

        request.Reject(admin, reviewNote);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RequestEventSettlementAsync(MockUser currentUser, long eventId, string? note = null, CancellationToken cancellationToken = default)
    {
        EnsurePlanner(currentUser);

        var datingEvent = await _db.DatingEvents
            .Include(item => item.EventPlannerUser)
            .Include(item => item.Tickets)
            .ThenInclude(ticket => ticket.TicketOrder)
            .FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
            ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

        if (datingEvent.EventPlannerUserId != currentUser.Id)
            throw new InvalidOperationException("فقط برگزارکننده رویداد می‌تواند درخواست تسویه ثبت کند.");

        if (datingEvent.IsCancelled || datingEvent.LifecycleStatus == EventLifecycleStatus.Cancelled)
            throw new InvalidOperationException("رویداد لغو شده قابل تسویه عادی نیست.");

        if (datingEvent.DateTimeEnd > DateTime.UtcNow)
            throw new InvalidOperationException("تسویه فقط بعد از پایان رویداد امکان‌پذیر است.");

        if (await _db.EventSettlementRequests.AnyAsync(item => item.DatingEventId == eventId && item.Status != EventSettlementRequestStatus.Rejected, cancellationToken))
            throw new InvalidOperationException("برای این رویداد قبلا درخواست تسویه فعال یا تایید شده ثبت شده است.");

        datingEvent.MarkCompleted(DateTime.UtcNow);
        var summary = CalculateSettlement(datingEvent);
        _db.EventSettlementRequests.Add(new EventSettlementRequest(
            datingEvent,
            datingEvent.EventPlannerUser,
            summary.ValidTicketCount,
            summary.GrossAmount,
            summary.PlatformCommissionAmount,
            summary.OrganizerIncomeAmount,
            summary.ReportingOrganizerIncomeIrr,
            note));
        _db.EventWorkflowLogs.Add(new EventWorkflowLog(
            datingEvent,
            EventWorkflowActionType.SettlementRequested,
            currentUser.Id,
            toApprovalStatus: datingEvent.ApprovalStatus,
            toSaleStatus: datingEvent.SaleStatus,
            toLifecycleStatus: datingEvent.LifecycleStatus,
            reason: note,
            metadataJson: System.Text.Json.JsonSerializer.Serialize(summary)));

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ConfirmEventSettlementAsync(MockUser currentUser, long requestId, string? reviewNote = null, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var admin = await _db.Users.FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("مدیر جاری پیدا نشد.");
        var request = await _db.EventSettlementRequests
            .Include(item => item.DatingEvent)
            .ThenInclude(item => item.EventPlannerUser)
            .FirstOrDefaultAsync(item => item.Id == requestId, cancellationToken)
            ?? throw new InvalidOperationException("درخواست تسویه رویداد پیدا نشد.");

        if (request.Status != EventSettlementRequestStatus.Pending)
            throw new InvalidOperationException("این درخواست قبلا بررسی شده است.");

        var account = await _db.BalanceAccounts
            .Include(item => item.Transactions)
            .FirstOrDefaultAsync(item => item.UserId == request.DatingEvent.EventPlannerUserId, cancellationToken);
        if (account is null)
        {
            account = new BalanceAccount(request.DatingEvent.EventPlannerUser);
            _db.BalanceAccounts.Add(account);
        }

        account.Credit(
            request.OrganizerIncomeAmount,
            BalanceTransactionType.EventSettlementCredit,
            $"بستانکاری تسویه رویداد {request.DatingEvent.Title}",
            request.DatingEventId,
            nameof(EventSettlementRequest),
            request.Id,
            currentUser.Id,
            request.DatingEvent.CurrencyCode,
            request.ReportingOrganizerIncomeIrr,
            1m,
            DateTime.UtcNow,
            null);
        var creditTransaction = account.Transactions.OrderByDescending(item => item.CreatedAt).First();
        request.Approve(admin, creditTransaction, reviewNote);
        _db.EventWorkflowLogs.Add(new EventWorkflowLog(
            request.DatingEvent,
            EventWorkflowActionType.OrganizerCredited,
            currentUser.Id,
            toApprovalStatus: request.DatingEvent.ApprovalStatus,
            toSaleStatus: request.DatingEvent.SaleStatus,
            toLifecycleStatus: request.DatingEvent.LifecycleStatus,
            reason: reviewNote,
            metadataJson: System.Text.Json.JsonSerializer.Serialize(new
            {
                request.ValidTicketCount,
                request.GrossAmount,
                request.PlatformCommissionAmount,
                request.OrganizerIncomeAmount,
                request.ReportingOrganizerIncomeIrr
            })));

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectEventSettlementAsync(MockUser currentUser, long requestId, string? reviewNote = null, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var admin = await _db.Users.FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("مدیر جاری پیدا نشد.");
        var request = await _db.EventSettlementRequests
            .Include(item => item.DatingEvent)
            .FirstOrDefaultAsync(item => item.Id == requestId, cancellationToken)
            ?? throw new InvalidOperationException("درخواست تسویه رویداد پیدا نشد.");

        request.Reject(admin, reviewNote);
        _db.EventWorkflowLogs.Add(new EventWorkflowLog(
            request.DatingEvent,
            EventWorkflowActionType.SettlementRejected,
            currentUser.Id,
            toApprovalStatus: request.DatingEvent.ApprovalStatus,
            toSaleStatus: request.DatingEvent.SaleStatus,
            toLifecycleStatus: request.DatingEvent.LifecycleStatus,
            reason: reviewNote));

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AdminEventTicketTransactionGroup>> GetTicketPurchaseTransactionsByEventAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var records = await (
                from order in _db.TicketOrders
                join datingEvent in _db.DatingEvents on order.DatingEventId equals datingEvent.Id
                join buyer in _db.Users on order.BuyerUserId equals buyer.Id
                join planner in _db.Users on datingEvent.EventPlannerUserId equals planner.Id
                where order.OrderStatus != TicketOrderStatus.Cancelled
                orderby datingEvent.DateTimeStart descending, order.CreatedAt descending
                select new
                {
                    EventId = datingEvent.Id,
                    EventTitle = datingEvent.Title,
                    StartAtUtc = datingEvent.DateTimeStart,
                    PlannerUserId = planner.Id,
                    PlannerMobile = planner.MobileNumber,
                    OrderId = order.Id,
                    TransactionId = order.Id,
                    BuyerUserId = buyer.Id,
                    BuyerMobile = buyer.MobileNumber,
                    TicketCount = order.Tickets.Count(ticket => !ticket.IsRefunded && !ticket.IsRemoved),
                    DiscountCode = order.DiscountCode,
                    OriginalPrice = order.GrossAmount,
                    DiscountAmount = order.DiscountAmount,
                    Amount = order.NetAmount,
                    CurrencyCode = order.CurrencyCode,
                    ReportingAmountIrr = order.ReportingNetAmountIrr,
                    Description = "سفارش خرید بلیت رویداد",
                    PurchasedAtUtc = order.CreatedAt
                })
            .ToListAsync(cancellationToken);

        var relatedUserIds = records
            .SelectMany(item => new[] { item.BuyerUserId, item.PlannerUserId })
            .Distinct()
            .ToList();

        var profiles = await _db.UserProfiles
            .Where(profile => relatedUserIds.Contains(profile.UserId))
            .Select(profile => new { profile.UserId, profile.DisplayName })
            .ToDictionaryAsync(profile => profile.UserId, profile => profile.DisplayName, cancellationToken);

        var mappedRecords = records
            .Select(item => new
            {
                item.EventId,
                item.EventTitle,
                item.StartAtUtc,
                PlannerName = profiles.GetValueOrDefault(item.PlannerUserId, item.PlannerMobile),
                Transaction = new AdminTicketTransactionItem
                {
                    OrderId = item.OrderId,
                    TransactionId = item.TransactionId,
                    BuyerUserId = item.BuyerUserId,
                    BuyerName = profiles.GetValueOrDefault(item.BuyerUserId, item.BuyerMobile),
                    BuyerMobile = item.BuyerMobile,
                    TicketCount = item.TicketCount,
                    DiscountCode = item.DiscountCode,
                    OriginalPrice = item.OriginalPrice,
                    DiscountAmount = item.DiscountAmount,
                    FinalPaidAmount = item.Amount,
                    Amount = Math.Abs(item.Amount),
                    CurrencyCode = item.CurrencyCode,
                    ReportingAmountIrr = item.ReportingAmountIrr,
                    Description = item.Description,
                    PurchasedAtUtc = item.PurchasedAtUtc
                }
            });

        return mappedRecords
            .GroupBy(item => new { item.EventId, item.EventTitle, item.StartAtUtc, item.PlannerName, item.Transaction.CurrencyCode })
            .Select(group => new AdminEventTicketTransactionGroup
            {
                EventId = group.Key.EventId,
                EventTitle = group.Key.EventTitle,
                StartAtUtc = group.Key.StartAtUtc,
                PlannerName = group.Key.PlannerName,
                CurrencyCode = group.Key.CurrencyCode,
                TicketCount = group.Sum(item => item.Transaction.TicketCount),
                TotalTicketAmount = group.Sum(item => item.Transaction.Amount),
                TotalTicketAmountIrr = group.Sum(item => item.Transaction.ReportingAmountIrr),
                Transactions = group.Select(item => item.Transaction).ToList()
            })
            .ToList();
    }

    public async Task<IReadOnlyList<ManualPaymentReceiptItem>> GetManualPaymentReceiptsAsync(MockUser currentUser, ManualPaymentDestinationType destinationType, CancellationToken cancellationToken = default)
    {
        var query = _db.ManualPaymentReceipts
            .AsNoTracking()
            .Include(item => item.DatingEvent)
            .Include(item => item.ParticipantUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.TicketOrder)
            .ThenInclude(order => order!.BuyerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.PlannerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ReviewedByUser)
            .ThenInclude(user => user!.Profile)
            .Where(item => item.DestinationType == destinationType);

        if (destinationType == ManualPaymentDestinationType.Platform)
        {
            EnsurePlatformReceiptReviewer(currentUser);
        }
        else
        {
            EnsurePlanner(currentUser);
            query = query.Where(item => item.PlannerUserId == currentUser.Id);
        }

        return await query
            .OrderBy(item => item.Status)
            .ThenByDescending(item => item.SubmittedAtUtc)
            .Select(item => new ManualPaymentReceiptItem
            {
                Id = item.Id,
                TicketOrderId = item.TicketOrderId,
                WalletCreditTransactionId = item.WalletCreditTransactionId,
                EventId = item.DatingEventId,
                EventTitle = item.DatingEvent.Title,
                PlannerUserId = item.PlannerUserId,
                PlannerName = item.PlannerUser.Profile == null ? item.PlannerUser.MobileNumber : item.PlannerUser.Profile.DisplayName,
                ParticipantUserId = item.ParticipantUserId,
                ParticipantName = item.ParticipantUser.Profile == null ? item.ParticipantUser.MobileNumber : item.ParticipantUser.Profile.DisplayName,
                ParticipantMobile = item.ParticipantUser.MobileNumber,
                BuyerUserId = item.TicketOrder == null ? item.ParticipantUserId : item.TicketOrder.BuyerUserId,
                BuyerName = item.TicketOrder == null
                    ? item.ParticipantUser.Profile == null ? item.ParticipantUser.MobileNumber : item.ParticipantUser.Profile.DisplayName
                    : item.TicketOrder.BuyerUser.Profile == null ? item.TicketOrder.BuyerUser.MobileNumber : item.TicketOrder.BuyerUser.Profile.DisplayName,
                BuyerMobile = item.TicketOrder == null ? item.ParticipantUser.MobileNumber : item.TicketOrder.BuyerUser.MobileNumber,
                PaymentCollectionMethod = item.PaymentCollectionMethod,
                DestinationType = item.DestinationType,
                Status = item.Status,
                OriginalAmount = item.OriginalAmount,
                DiscountAmount = item.DiscountAmount,
                Amount = item.Amount,
                CurrencyCode = item.CurrencyCode,
                ReportingAmountIrr = item.ReportingAmountIrr,
                ExchangeRateToIrr = item.ExchangeRateToIrr,
                DiscountCode = item.DiscountCode,
                UploadedFilePath = item.UploadedFilePath,
                TrackingNumber = item.TrackingNumber,
                PayerNote = item.PayerNote,
                SubmittedAtUtc = item.SubmittedAtUtc,
                ReviewedByName = item.ReviewedByUser == null
                    ? null
                    : item.ReviewedByUser.Profile == null
                        ? item.ReviewedByUser.MobileNumber
                        : item.ReviewedByUser.Profile.DisplayName,
                ReviewedAtUtc = item.ReviewedAtUtc,
                RejectReason = item.RejectReason,
                EventTicketId = item.EventTicketId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task ApproveManualPaymentReceiptAsync(MockUser currentUser, long receiptId, CancellationToken cancellationToken = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var reviewer = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("حساب بررسی‌کننده پیدا نشد.");

        var receipt = await _db.ManualPaymentReceipts
            .Include(item => item.DatingEvent)
            .ThenInclude(datingEvent => datingEvent.EventPlannerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.DatingEvent)
            .ThenInclude(datingEvent => datingEvent.Tickets)
            .Include(item => item.ParticipantUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.EventDiscountCode)
            .Include(item => item.TicketOrder)
            .ThenInclude(order => order!.BuyerUser)
            .FirstOrDefaultAsync(item => item.Id == receiptId, cancellationToken)
            ?? throw new InvalidOperationException("رسید پرداخت پیدا نشد.");

        EnsureReceiptReviewAccess(currentUser, receipt);

        if (receipt.Status != ManualPaymentReceiptStatus.Submitted)
            throw new InvalidOperationException("این رسید قبلا بررسی شده است.");

        if (receipt.DatingEvent.IsCancelled || receipt.DatingEvent.LifecycleStatus == EventLifecycleStatus.Cancelled)
        {
            await ApproveCancelledEventManualReceiptAsWalletCreditAsync(receipt, reviewer, currentUser.Id, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return;
        }

        var profile = receipt.ParticipantUser.Profile
            ?? throw new InvalidOperationException("پروفایل شرکت‌کننده برای صدور بلیت پیدا نشد.");

        var order = receipt.TicketOrder;
        if (order is null)
        {
            var platformCommission = receipt.Amount * receipt.DatingEvent.EventPlannerCommissionPercent / 100m;
            order = new TicketOrder(
                receipt.DatingEvent,
                receipt.ParticipantUser,
                receipt.OriginalAmount,
                receipt.DiscountAmount,
                receipt.Amount,
                platformCommission,
                receipt.PaymentCollectionMethod,
                receipt.CurrencyCode,
                receipt.ExchangeRateToIrr,
                receipt.ExchangeRateCapturedAtUtc,
                receipt.ExchangeRateId,
                receipt.EventDiscountCode,
                TicketOrderPaymentStatus.Pending,
                TicketOrderStatus.PendingPayment,
                $"Manual receipt #{receipt.Id}");
            _db.TicketOrders.Add(order);
        }

        order.MarkPaid(reviewer.Id);

        var ticket = receipt.DatingEvent.SellTicket(order, receipt.ParticipantUser, profile, receipt.Amount, receipt.EventDiscountCode);
        ticket.CaptureExchangeRate(receipt.ExchangeRateToIrr, receipt.ExchangeRateCapturedAtUtc, receipt.ExchangeRateId);
        receipt.EventDiscountCode?.RegisterUsage(DateTime.UtcNow);

        if (receipt.DestinationType != ManualPaymentDestinationType.Organizer)
        {
            _db.OnlinePayments.Add(new OnlinePayment(
                order.BuyerUser,
                receipt.Amount,
                "PlatformManualTransfer",
                $"manual-receipt-{receipt.Id}",
                OnlinePaymentStatus.Succeeded,
                receipt.DatingEvent,
                ticket,
                null,
                receipt.CurrencyCode,
                receipt.ReportingAmountIrr,
                receipt.ExchangeRateToIrr,
                receipt.ExchangeRateCapturedAtUtc,
                receipt.ExchangeRateId,
                order));
        }

        receipt.Approve(reviewer, order, ticket);
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task ApproveCancelledEventManualReceiptAsWalletCreditAsync(ManualPaymentReceipt receipt, User reviewer, long reviewerUserId, CancellationToken cancellationToken)
    {
        var participantAccount = await GetOrCreateBalanceAccountAsync(receipt.ParticipantUser, cancellationToken);
        participantAccount.Credit(
            receipt.Amount,
            BalanceTransactionType.ManualReceiptWalletCredit,
            $"اعتبار کیف پول بابت رسید دستی رویداد لغوشده {receipt.DatingEvent.Title}",
            receipt.DatingEventId,
            nameof(ManualPaymentReceipt),
            receipt.Id,
            reviewerUserId,
            receipt.CurrencyCode,
            receipt.ReportingAmountIrr,
            receipt.ExchangeRateToIrr,
            receipt.ExchangeRateCapturedAtUtc,
            receipt.ExchangeRateId);

        var creditTransaction = participantAccount.Transactions.OrderByDescending(item => item.CreatedAt).First();
        receipt.ApproveAsWalletCredit(reviewer, creditTransaction);
        AddInAppNotification(
            reviewer,
            receipt.ParticipantUser,
            NotificationType.Finance,
            NotificationPriority.Important,
            "رسید پرداخت به کیف پول منتقل شد",
            $"رسید پرداخت شما برای رویداد لغوشده {receipt.DatingEvent.Title} تایید شد و مبلغ به کیف پول اضافه شد.",
            receipt.DatingEvent,
            nameof(ManualPaymentReceipt),
            receipt.Id);

        if (receipt.DestinationType == ManualPaymentDestinationType.Organizer)
        {
            var plannerAccount = await GetOrCreateBalanceAccountAsync(receipt.DatingEvent.EventPlannerUser, cancellationToken);
            plannerAccount.DebitAllowNegative(
                receipt.Amount,
                BalanceTransactionType.OrganizerManualReceiptLiability,
                $"بدهی برگزارکننده بابت اعتبار کیف پول رسید رویداد لغوشده {receipt.DatingEvent.Title}",
                receipt.DatingEventId,
                nameof(ManualPaymentReceipt),
                receipt.Id,
                reviewerUserId,
                receipt.CurrencyCode,
                receipt.ReportingAmountIrr,
                receipt.ExchangeRateToIrr,
                receipt.ExchangeRateCapturedAtUtc,
                receipt.ExchangeRateId);
        }
    }

    public async Task RejectManualPaymentReceiptAsync(MockUser currentUser, long receiptId, string reason, CancellationToken cancellationToken = default)
    {
        var reviewer = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("حساب بررسی‌کننده پیدا نشد.");

        var receipt = await _db.ManualPaymentReceipts
            .Include(item => item.DatingEvent)
            .Include(item => item.TicketOrder)
            .FirstOrDefaultAsync(item => item.Id == receiptId, cancellationToken)
            ?? throw new InvalidOperationException("رسید پرداخت پیدا نشد.");

        EnsureReceiptReviewAccess(currentUser, receipt);
        receipt.Reject(reviewer, reason);
        receipt.TicketOrder?.MarkRejected(reviewer.Id, reason);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TicketRefundRequestItem>> GetTicketRefundRequestsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        var query = _db.TicketRefundRequests
            .AsNoTracking()
            .Include(item => item.DatingEvent)
            .Include(item => item.BuyerUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ParticipantUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ReviewedByUser)
            .ThenInclude(user => user!.Profile)
            .AsQueryable();

        if (currentUser.Role == AdminRole.EventPlanner)
            query = query.Where(item => item.DatingEvent.EventPlannerUserId == currentUser.Id);
        else if (currentUser.Role is not (AdminRole.Admin or AdminRole.SupportTeam))
            throw new InvalidOperationException("شما به فهرست درخواست‌های بازگشت وجه دسترسی ندارید.");

        return await query
            .OrderBy(item => item.Status)
            .ThenByDescending(item => item.RequestedAtUtc)
            .Select(item => new TicketRefundRequestItem
            {
                Id = item.Id,
                EventTicketId = item.EventTicketId,
                TicketOrderId = item.TicketOrderId,
                EventId = item.DatingEventId,
                EventTitle = item.DatingEvent.Title,
                BuyerUserId = item.BuyerUserId,
                BuyerName = item.BuyerUser.Profile == null ? item.BuyerUser.MobileNumber : item.BuyerUser.Profile.DisplayName,
                BuyerMobile = item.BuyerUser.MobileNumber,
                ParticipantUserId = item.ParticipantUserId,
                ParticipantName = item.ParticipantUser.Profile == null ? item.ParticipantUser.MobileNumber : item.ParticipantUser.Profile.DisplayName,
                ParticipantMobile = item.ParticipantUser.MobileNumber,
                Status = item.Status,
                RequestedAmount = item.RequestedAmount,
                ApprovedAmount = item.ApprovedAmount,
                CurrencyCode = item.CurrencyCode,
                ReportingRequestedAmountIrr = item.ReportingRequestedAmountIrr,
                ReportingApprovedAmountIrr = item.ReportingApprovedAmountIrr,
                RequestReason = item.RequestReason,
                RequestedAtUtc = item.RequestedAtUtc,
                ReviewedByName = item.ReviewedByUser == null
                    ? null
                    : item.ReviewedByUser.Profile == null
                        ? item.ReviewedByUser.MobileNumber
                        : item.ReviewedByUser.Profile.DisplayName,
                ReviewedAtUtc = item.ReviewedAtUtc,
                ReviewNote = item.ReviewNote,
                WalletCreditTransactionId = item.WalletCreditTransactionId
            })
            .ToListAsync(cancellationToken);
    }

    public async Task RequestTicketRefundAsync(MockUser currentUser, long ticketId, string reason, CancellationToken cancellationToken = default)
    {
        var ticket = await _db.EventTickets
            .Include(item => item.DatingEvent)
            .Include(item => item.TicketOrder)
            .ThenInclude(order => order.BuyerUser)
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.Id == ticketId, cancellationToken)
            ?? throw new InvalidOperationException("بلیت پیدا نشد.");

        if (ticket.IsRefunded || ticket.IsRemoved)
            throw new InvalidOperationException("این بلیت قبلا بازگشت خورده یا حذف شده است.");

        if (await _db.TicketRefundRequests.AnyAsync(item => item.EventTicketId == ticketId && item.Status == TicketRefundRequestStatus.Pending, cancellationToken))
            throw new InvalidOperationException("برای این بلیت یک درخواست بازگشت وجه فعال وجود دارد.");

        if (currentUser.Role == AdminRole.EventPlanner && ticket.DatingEvent.EventPlannerUserId != currentUser.Id)
            throw new InvalidOperationException("برگزارکننده فقط برای رویدادهای خودش می‌تواند درخواست ثبت کند.");

        if (currentUser.Role is not (AdminRole.Admin or AdminRole.SupportTeam or AdminRole.EventPlanner))
            throw new InvalidOperationException("شما به ثبت درخواست بازگشت وجه دسترسی ندارید.");

        var requester = await _db.Users.FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("کاربر جاری پیدا نشد.");

        _db.TicketRefundRequests.Add(new TicketRefundRequest(ticket, requester, ticket.Price, reason));
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveTicketRefundRequestAsync(MockUser currentUser, long requestId, TicketRefundReviewInput input, CancellationToken cancellationToken = default)
    {
        EnsurePlatformReceiptReviewer(currentUser);
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var reviewer = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("حساب بررسی‌کننده پیدا نشد.");

        var request = await _db.TicketRefundRequests
            .Include(item => item.EventTicket)
            .Include(item => item.TicketOrder)
            .ThenInclude(order => order.Tickets)
            .Include(item => item.DatingEvent)
            .Include(item => item.BuyerUser)
            .FirstOrDefaultAsync(item => item.Id == requestId, cancellationToken)
            ?? throw new InvalidOperationException("درخواست بازگشت وجه پیدا نشد.");

        if (request.Status != TicketRefundRequestStatus.Pending)
            throw new InvalidOperationException("این درخواست قبلا بررسی شده است.");

        var approvedAmount = input.ApprovedAmount <= 0 ? request.RequestedAmount : input.ApprovedAmount;
        if (approvedAmount > request.RequestedAmount)
            throw new InvalidOperationException("مبلغ تایید شده نمی‌تواند بیشتر از مبلغ درخواست باشد.");

        var buyerAccount = await GetOrCreateBalanceAccountAsync(request.BuyerUser, cancellationToken);
        buyerAccount.Credit(
            approvedAmount,
            BalanceTransactionType.TicketRefund,
            $"بازگشت وجه بلیت رویداد {request.DatingEvent.Title}",
            request.DatingEventId,
            nameof(TicketRefundRequest),
            request.Id,
            currentUser.Id,
            request.CurrencyCode,
            ConvertToIrr(approvedAmount, request.ExchangeRateToIrr),
            request.ExchangeRateToIrr,
            request.ExchangeRateCapturedAtUtc,
            request.ExchangeRateId,
            request.TicketOrder);

        var creditTransaction = buyerAccount.Transactions.OrderByDescending(item => item.CreatedAt).First();
        await _db.SaveChangesAsync(cancellationToken);

        request.Approve(reviewer, approvedAmount, creditTransaction, input.ReviewNote);
        if (request.TicketOrder.Tickets.All(ticket => ticket.IsRefunded || ticket.IsRemoved))
            request.TicketOrder.MarkRefunded();

        AddInAppNotification(
            reviewer,
            request.BuyerUser,
            NotificationType.Refund,
            NotificationPriority.Important,
            "بازگشت وجه تایید شد",
            $"درخواست بازگشت وجه بلیت رویداد {request.DatingEvent.Title} تایید شد و مبلغ به کیف پول شما اضافه شد.",
            request.DatingEvent,
            nameof(TicketRefundRequest),
            request.Id);

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task RejectTicketRefundRequestAsync(MockUser currentUser, long requestId, string reviewNote, CancellationToken cancellationToken = default)
    {
        EnsurePlatformReceiptReviewer(currentUser);

        var reviewer = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("حساب بررسی‌کننده پیدا نشد.");

        var request = await _db.TicketRefundRequests
            .Include(item => item.DatingEvent)
            .Include(item => item.BuyerUser)
            .FirstOrDefaultAsync(item => item.Id == requestId, cancellationToken)
            ?? throw new InvalidOperationException("درخواست بازگشت وجه پیدا نشد.");

        if (request.Status != TicketRefundRequestStatus.Pending)
            throw new InvalidOperationException("این درخواست قبلا بررسی شده است.");

        request.Reject(reviewer, reviewNote);
        AddInAppNotification(
            reviewer,
            request.BuyerUser,
            NotificationType.Refund,
            NotificationPriority.Important,
            "درخواست بازگشت وجه رد شد",
            $"درخواست بازگشت وجه بلیت رویداد {request.DatingEvent.Title} رد شد. توضیح بررسی: {reviewNote}",
            request.DatingEvent,
            nameof(TicketRefundRequest),
            request.Id);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserFinanceOverview> GetUserFinanceAsync(MockUser currentUser, long userId, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var user = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("شرکت‌کننده پیدا نشد.");

        var account = await _db.BalanceAccounts
            .Include(item => item.Transactions)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        var eventIds = (account?.Transactions.Select(item => item.DatingEventId) ?? Array.Empty<long?>())
            .Where(item => item.HasValue)
            .Select(item => item!.Value)
            .Distinct()
            .ToList();

        var payments = await _db.OnlinePayments
            .Where(payment => payment.UserId == userId)
            .OrderByDescending(payment => payment.CreatedAt)
            .Select(payment => new UserOnlinePaymentItem
            {
                Id = payment.Id,
                Amount = payment.Amount,
                CurrencyCode = payment.CurrencyCode,
                ReportingAmountIrr = payment.ReportingAmountIrr,
                GatewayName = payment.GatewayName,
                TrackingCode = payment.TrackingCode,
                Status = payment.Status,
                EventId = payment.DatingEventId,
                EventTitle = payment.DatingEvent == null ? "بدون رویداد" : payment.DatingEvent.Title,
                TicketId = payment.EventTicketId,
                BalanceTransactionId = payment.BalanceTransactionId,
                CreatedAtUtc = payment.CreatedAt,
                PaidAtUtc = payment.PaidAtUtc
            })
            .ToListAsync(cancellationToken);

        eventIds.AddRange(payments.Where(item => item.EventId.HasValue).Select(item => item.EventId!.Value));
        var eventTitles = await _db.DatingEvents
            .Where(item => eventIds.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, item => item.Title, cancellationToken);

        var transactions = (account?.Transactions ?? Array.Empty<BalanceTransaction>())
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new UserFinanceTransactionItem
            {
                Id = item.Id,
                Amount = item.Amount,
                CurrencyCode = item.CurrencyCode,
                ReportingAmountIrr = item.ReportingAmountIrr,
                ExchangeRateToIrr = item.ExchangeRateToIrr,
                Type = item.Type,
                Description = item.Description,
                EventId = item.DatingEventId,
                EventTitle = item.DatingEventId is long eventId && eventTitles.TryGetValue(eventId, out var eventTitle) ? eventTitle : "بدون رویداد",
                CreatedAtUtc = item.CreatedAt
            })
            .ToList();

        foreach (var payment in payments)
        {
            if (payment.EventId is long eventId && eventTitles.TryGetValue(eventId, out var eventTitle))
                payment.EventTitle = eventTitle;
        }

        return new UserFinanceOverview
        {
            UserId = user.Id,
            DisplayName = DatabaseModelMapper.ResolveUserDisplayName(user),
            MobileNumber = user.MobileNumber,
            IsActive = user.IsActive,
            Balance = account?.Balance ?? 0m,
            ReportingCurrencyCode = account?.ReportingCurrencyCode ?? "IRR",
            Transactions = transactions,
            OnlinePayments = payments
        };
    }

    public async Task<IReadOnlyList<PlannerBankAccountItem>> GetPlannerBankAccountsAsync(MockUser currentUser, long plannerUserId, CancellationToken cancellationToken = default)
    {
        EnsureAdminOrOwner(currentUser, plannerUserId);

        return await _db.PlannerBankAccounts
            .Where(item => item.UserId == plannerUserId)
            .OrderByDescending(item => item.IsActive)
            .ThenByDescending(item => item.CreatedAt)
            .Select(item => new PlannerBankAccountItem
            {
                Id = item.Id,
                UserId = item.UserId,
                CurrencyCode = item.CurrencyCode,
                PayoutMethod = item.PayoutMethod.ToString(),
                AccountHolderName = item.AccountHolderName,
                Country = item.Country,
                CardNumber = item.CardNumber,
                Iban = item.Iban,
                BankName = item.BankName,
                AccountNumber = item.AccountNumber,
                SwiftCode = item.SwiftCode,
                AccountIdentifier = item.AccountIdentifier,
                PublicPaymentInstructions = item.PublicPaymentInstructions,
                IsActive = item.IsActive,
                CreatedAtUtc = item.CreatedAt
            })
            .ToListAsync(cancellationToken);
    }

    public async Task SavePlannerBankAccountAsync(MockUser currentUser, long plannerUserId, PlannerBankAccountInput input, CancellationToken cancellationToken = default)
    {
        EnsureAdminOrOwner(currentUser, plannerUserId);

        var planner = await _db.Users.FirstOrDefaultAsync(item => item.Id == plannerUserId, cancellationToken)
            ?? throw new InvalidOperationException("برگزارکننده پیدا نشد.");
        input.CurrencyCode = await ResolveCurrencyCodeAsync(input.CurrencyCode, cancellationToken);
        if (string.IsNullOrWhiteSpace(input.AccountHolderName))
            input.AccountHolderName = planner.Profile?.DisplayName ?? planner.MobileNumber;

        if (input.Id is long id)
        {
            var bankAccount = await _db.PlannerBankAccounts
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == plannerUserId, cancellationToken)
                ?? throw new InvalidOperationException("حساب بانکی پیدا نشد.");

            bankAccount.Update(
                input.CurrencyCode,
                input.PayoutMethod,
                input.AccountHolderName,
                input.Country,
                input.CardNumber,
                input.Iban,
                input.BankName,
                input.AccountNumber,
                input.SwiftCode,
                input.AccountIdentifier,
                input.PublicPaymentInstructions,
                input.IsActive);
        }
        else
        {
            _db.PlannerBankAccounts.Add(new PlannerBankAccount(
                planner,
                input.CurrencyCode,
                input.PayoutMethod,
                input.AccountHolderName,
                input.Country,
                input.CardNumber,
                input.Iban,
                input.BankName,
                input.AccountNumber,
                input.SwiftCode,
                input.AccountIdentifier,
                input.PublicPaymentInstructions,
                input.IsActive));
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task TogglePlannerBankAccountAsync(MockUser currentUser, long plannerUserId, long bankAccountId, bool isActive, CancellationToken cancellationToken = default)
    {
        EnsureAdminOrOwner(currentUser, plannerUserId);

        var bankAccount = await _db.PlannerBankAccounts
            .FirstOrDefaultAsync(item => item.Id == bankAccountId && item.UserId == plannerUserId, cancellationToken)
            ?? throw new InvalidOperationException("حساب بانکی پیدا نشد.");

        if (isActive)
            bankAccount.Activate();
        else
            bankAccount.Deactivate();

        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<PlannerWithdrawalRequestItem>> GetPlannerWithdrawalsAsync(long plannerUserId, CancellationToken cancellationToken)
    {
        return await MapWithdrawalRequests(_db.PlannerWithdrawalRequests.Where(item => item.UserId == plannerUserId))
            .OrderByDescending(item => item.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<PlannerWithdrawalRequestItem> MapWithdrawalRequests(IQueryable<PlannerWithdrawalRequest> query)
    {
        return query.Select(item => new PlannerWithdrawalRequestItem
        {
            Id = item.Id,
            PlannerUserId = item.UserId,
            PlannerName = item.User.Profile == null ? item.User.MobileNumber : item.User.Profile.DisplayName,
            PlannerMobile = item.User.MobileNumber,
            Amount = item.Amount,
            CurrencyCode = item.CurrencyCode,
            ReportingAmountIrr = item.ReportingAmountIrr,
            Status = item.Status,
            RequestedAtUtc = item.RequestedAtUtc,
            ReviewedAtUtc = item.ReviewedAtUtc,
            ReviewNote = item.ReviewNote
        });
    }

    private async Task<CurrencyRateSnapshot> GetActiveRateToIrrAsync(string currencyCode, DateTime atUtc, CancellationToken cancellationToken)
    {
        var normalizedCurrency = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        var normalizedAt = atUtc.Kind == DateTimeKind.Utc
            ? atUtc
            : DateTime.SpecifyKind(atUtc, DateTimeKind.Utc);

        var rate = await _db.CurrencyExchangeRates
            .Where(item => item.FromCurrencyCode == normalizedCurrency
                && item.ToCurrencyCode == "IRR"
                && item.EffectiveFromUtc <= normalizedAt
                && (item.EffectiveToUtc == null || item.EffectiveToUtc > normalizedAt))
            .OrderByDescending(item => item.EffectiveFromUtc)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException($"نرخ فعال برای ارز {normalizedCurrency} ثبت نشده است.");

        return new CurrencyRateSnapshot(rate.Id, rate.Rate, normalizedAt);
    }

    private async Task<string> ResolveCurrencyCodeAsync(string? currencyCode, CancellationToken cancellationToken)
    {
        var normalized = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        var exists = await _db.Currencies.AnyAsync(item => item.Code == normalized && item.IsActive, cancellationToken);
        if (!exists)
            throw new InvalidOperationException("واحد پول حساب بانکی معتبر نیست.");

        return normalized;
    }

    private static EventSettlementSummary CalculateSettlement(DatingEvent datingEvent)
    {
        var validTickets = datingEvent.Tickets
            .Where(ticket => !ticket.IsRefunded
                && !ticket.IsRemoved
                && ticket.TicketOrder.PaymentStatus == TicketOrderPaymentStatus.Paid
                && ticket.TicketOrder.OrderStatus == TicketOrderStatus.Confirmed)
            .ToList();

        return new EventSettlementSummary(
            validTickets.Count,
            validTickets.Sum(ticket => ticket.Price),
            validTickets.Sum(ticket => ticket.TicketOrder.PlatformCommissionAmount),
            validTickets.Sum(ticket => ticket.TicketOrder.OrganizerIncomeAmount),
            validTickets.Sum(ticket => ticket.TicketOrder.ReportingOrganizerIncomeIrr));
    }

    private static decimal ConvertToIrr(decimal amount, decimal rate)
        => Math.Round(amount * rate, 0, MidpointRounding.AwayFromZero);

    private sealed record CurrencyRateSnapshot(long ExchangeRateId, decimal Rate, DateTime CapturedAtUtc);

    private sealed record EventSettlementSummary(
        int ValidTicketCount,
        decimal GrossAmount,
        decimal PlatformCommissionAmount,
        decimal OrganizerIncomeAmount,
        decimal ReportingOrganizerIncomeIrr);

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

    private void AddInAppNotification(
        User createdByUser,
        User recipient,
        NotificationType type,
        NotificationPriority priority,
        string title,
        string body,
        DatingEvent? datingEvent,
        string referenceType,
        long referenceId)
    {
        var notification = new Notification(
            createdByUser,
            type,
            title,
            body,
            priority,
            requiresApproval: false,
            datingEvent,
            referenceType,
            referenceId);
        notification.AddRecipient(recipient, NotificationDeliveryChannel.InApp);
        _db.Notifications.Add(notification);
    }

    private static void EnsurePlanner(MockUser currentUser)
    {
        if (currentUser.Role != AdminRole.EventPlanner)
            throw new InvalidOperationException("این بخش فقط برای برگزارکننده ها فعال است.");
    }

    private static void EnsureAdmin(MockUser currentUser)
    {
        if (currentUser.Role != AdminRole.Admin)
            throw new InvalidOperationException("این بخش فقط برای مدیر سیستم فعال است.");
    }

    private static void EnsurePlatformReceiptReviewer(MockUser currentUser)
    {
        if (currentUser.Role is AdminRole.Admin or AdminRole.SupportTeam)
            return;

        throw new InvalidOperationException("بررسی رسیدهای پلتفرم فقط برای مدیر یا پشتیبان سایت فعال است.");
    }

    private static void EnsureReceiptReviewAccess(MockUser currentUser, ManualPaymentReceipt receipt)
    {
        if (receipt.DestinationType == ManualPaymentDestinationType.Platform)
        {
            EnsurePlatformReceiptReviewer(currentUser);
            return;
        }

        if (currentUser.Role == AdminRole.Admin)
            return;

        if (currentUser.Role == AdminRole.EventPlanner && receipt.PlannerUserId == currentUser.Id)
            return;

        throw new InvalidOperationException("شما به بررسی این رسید دسترسی ندارید.");
    }

    private static void EnsureAdminOrOwner(MockUser currentUser, long ownerUserId)
    {
        if (currentUser.Role == AdminRole.Admin)
            return;

        if (currentUser.Role == AdminRole.EventPlanner && currentUser.Id == ownerUserId)
            return;

        throw new InvalidOperationException("دسترسی به اطلاعات بانکی این برگزارکننده مجاز نیست.");
    }
}
