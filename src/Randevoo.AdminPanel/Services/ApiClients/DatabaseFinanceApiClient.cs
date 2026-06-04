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

        var balance = await _db.BalanceAccounts
            .Where(account => account.UserId == currentUser.Id)
            .Select(account => (decimal?)account.Balance)
            .FirstOrDefaultAsync(cancellationToken) ?? 0m;

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
            .Where(transaction => transaction.Type == BalanceTransactionType.EventPlannerIncome || transaction.Type == BalanceTransactionType.PlannerWithdrawalPayout)
            .OrderByDescending(transaction => transaction.CreatedAt)
            .Select(transaction => new PlannerCommissionTransactionItem
            {
                Id = transaction.Id,
                EventId = transaction.DatingEventId,
                Amount = transaction.Amount,
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
                PlannerIncome = group.Sum(ticket => ticket.Price * (100m - group.Key.EventPlannerCommissionPercent) / 100m)
            })
            .OrderByDescending(item => item.StartAtUtc)
            .ToListAsync(cancellationToken);

        var withdrawals = await GetPlannerWithdrawalsAsync(currentUser.Id, cancellationToken);
        var totalCommissionIncome = transactions
            .Where(transaction => transaction.Type == BalanceTransactionType.EventPlannerIncome)
            .Sum(transaction => transaction.Amount);
        var paidWithdrawals = transactions
            .Where(transaction => transaction.Type == BalanceTransactionType.PlannerWithdrawalPayout)
            .Sum(transaction => Math.Abs(transaction.Amount));
        var pendingWithdrawals = withdrawals
            .Where(item => item.Status == PlannerWithdrawalRequestStatus.Pending)
            .Sum(item => item.Amount);

        return new PlannerFinanceDashboard
        {
            CurrentBalance = balance,
            TotalCommissionIncome = totalCommissionIncome,
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

        var dashboard = await GetPlannerFinanceAsync(currentUser, cancellationToken);
        if (amount <= 0)
            throw new InvalidOperationException("مبلغ برداشت باید بیشتر از صفر باشد.");

        if (amount > dashboard.AvailableWithdrawalAmount)
            throw new InvalidOperationException("مبلغ درخواست بیشتر از موجودی قابل برداشت است.");

        var planner = await _db.Users.FirstOrDefaultAsync(user => user.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("برگزارکننده پیدا نشد.");

        _db.PlannerWithdrawalRequests.Add(new PlannerWithdrawalRequest(planner, amount));
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
            currentUser.Id);
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

    public async Task<IReadOnlyList<AdminEventTicketTransactionGroup>> GetTicketPurchaseTransactionsByEventAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var records = await (
                from transaction in _db.BalanceTransactions
                join datingEvent in _db.DatingEvents on transaction.DatingEventId!.Value equals datingEvent.Id
                join buyer in _db.Users on transaction.UserId equals buyer.Id
                join planner in _db.Users on datingEvent.EventPlannerUserId equals planner.Id
                where transaction.Type == BalanceTransactionType.TicketPurchase && transaction.DatingEventId != null
                orderby datingEvent.DateTimeStart descending, transaction.CreatedAt descending
                select new
                {
                    EventId = datingEvent.Id,
                    EventTitle = datingEvent.Title,
                    StartAtUtc = datingEvent.DateTimeStart,
                    PlannerUserId = planner.Id,
                    PlannerMobile = planner.MobileNumber,
                    TransactionId = transaction.Id,
                    BuyerUserId = buyer.Id,
                    BuyerMobile = buyer.MobileNumber,
                    Amount = transaction.Amount,
                    Description = transaction.Description,
                    PurchasedAtUtc = transaction.CreatedAt
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
                    TransactionId = item.TransactionId,
                    BuyerUserId = item.BuyerUserId,
                    BuyerName = profiles.GetValueOrDefault(item.BuyerUserId, item.BuyerMobile),
                    BuyerMobile = item.BuyerMobile,
                    Amount = Math.Abs(item.Amount),
                    Description = item.Description,
                    PurchasedAtUtc = item.PurchasedAtUtc
                }
            });

        return mappedRecords
            .GroupBy(item => new { item.EventId, item.EventTitle, item.StartAtUtc, item.PlannerName })
            .Select(group => new AdminEventTicketTransactionGroup
            {
                EventId = group.Key.EventId,
                EventTitle = group.Key.EventTitle,
                StartAtUtc = group.Key.StartAtUtc,
                PlannerName = group.Key.PlannerName,
                TicketCount = group.Count(),
                TotalTicketAmount = group.Sum(item => item.Transaction.Amount),
                Transactions = group.Select(item => item.Transaction).ToList()
            })
            .ToList();
    }

    public async Task<UserFinanceOverview> GetUserFinanceAsync(MockUser currentUser, long userId, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var user = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("کاربر پیدا نشد.");

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
                CardNumber = item.CardNumber,
                Iban = item.Iban,
                BankName = item.BankName,
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

        if (input.Id is long id)
        {
            var bankAccount = await _db.PlannerBankAccounts
                .FirstOrDefaultAsync(item => item.Id == id && item.UserId == plannerUserId, cancellationToken)
                ?? throw new InvalidOperationException("حساب بانکی پیدا نشد.");

            bankAccount.Update(input.CardNumber, input.Iban, input.BankName, input.IsActive);
        }
        else
        {
            _db.PlannerBankAccounts.Add(new PlannerBankAccount(planner, input.CardNumber, input.Iban, input.BankName, input.IsActive));
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
            Status = item.Status,
            RequestedAtUtc = item.RequestedAtUtc,
            ReviewedAtUtc = item.ReviewedAtUtc,
            ReviewNote = item.ReviewNote
        });
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

    private static void EnsureAdminOrOwner(MockUser currentUser, long ownerUserId)
    {
        if (currentUser.Role == AdminRole.Admin)
            return;

        if (currentUser.Role == AdminRole.EventPlanner && currentUser.Id == ownerUserId)
            return;

        throw new InvalidOperationException("دسترسی به اطلاعات بانکی این برگزارکننده مجاز نیست.");
    }
}
