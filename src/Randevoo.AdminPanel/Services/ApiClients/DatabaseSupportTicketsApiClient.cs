using MediatR;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Support;
using Randevoo.Application.Features.SupportTickets.Commands.ChangeSupportTicketStatus;
using Randevoo.Application.Features.SupportTickets.Commands.CreateSupportTicket;
using Randevoo.Application.Features.SupportTickets.Commands.ReplyToSupportTicket;
using Randevoo.Application.Features.SupportTickets.Commands.ReassignSupportTicket;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Application.Features.SupportTickets.Queries.GetSupportTicket;
using Randevoo.Application.Features.SupportTickets.Queries.ListSupportTickets;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseSupportTicketsApiClient : ISupportTicketsApiClient
{
    private readonly ISender _sender;
    private readonly IUserRepository _users;
    private readonly RandevooDbContext _db;

    public DatabaseSupportTicketsApiClient(ISender sender, IUserRepository users, RandevooDbContext db)
    {
        _sender = sender;
        _users = users;
        _db = db;
    }

    public async Task<IReadOnlyList<SupportTicketListItemDto>> GetTicketsAsync(MockUser currentUser, SupportTicketStatus? status, SupportTicketCategory? category, UserRole? submitterRole, long? assigneeUserId, DateTime? createdFromUtc = null, DateTime? createdToUtc = null, CancellationToken cancellationToken = default)
    {
        var effectiveAssigneeUserId = ResolveAssigneeFilter(currentUser, assigneeUserId);
        return await _sender.Send(new ListSupportTicketsQuery(currentUser.Id, status, category, submitterRole, effectiveAssigneeUserId, createdFromUtc, createdToUtc, 200), cancellationToken);
    }

    public async Task<SupportTicketDashboardViewModel> GetDashboardAsync(MockUser currentUser, SupportTicketDashboardFilters filters, CancellationToken cancellationToken = default)
    {
        EnsureSupportOrAdmin(currentUser);

        var query = _db.SupportTickets
            .AsNoTracking()
            .Include(ticket => ticket.SubmitterUser).ThenInclude(user => user.Profile)
            .Include(ticket => ticket.AssignedSupportUser).ThenInclude(user => user!.Profile)
            .AsQueryable();

        if (filters.Status is not null)
            query = query.Where(ticket => ticket.Status == filters.Status);
        if (filters.Category is not null)
            query = query.Where(ticket => ticket.Category == filters.Category);
        if (filters.SubmitterRole is not null)
            query = query.Where(ticket => ticket.SubmitterRole == filters.SubmitterRole);
        var effectiveAssigneeUserId = ResolveAssigneeFilter(currentUser, filters.AssigneeUserId);
        if (effectiveAssigneeUserId is not null)
            query = query.Where(ticket => ticket.AssignedSupportUserId == effectiveAssigneeUserId);
        if (filters.CreatedFromUtc is not null)
            query = query.Where(ticket => ticket.CreatedAt >= filters.CreatedFromUtc.Value.Date);
        if (filters.CreatedToUtc is not null)
            query = query.Where(ticket => ticket.CreatedAt < filters.CreatedToUtc.Value.Date.AddDays(1));

        var tickets = await query.ToListAsync(cancellationToken);
        var statusPoints = Enum.GetValues<SupportTicketStatus>()
            .Select(status => new SupportTicketChartPoint(ToPersianStatus(status), tickets.Count(ticket => ticket.Status == status)))
            .ToList();
        var categoryPoints = Enum.GetValues<SupportTicketCategory>()
            .Select(category => new SupportTicketChartPoint(ToPersianCategory(category), tickets.Count(ticket => ticket.Category == category)))
            .ToList();
        var fromDate = filters.CreatedFromUtc?.Date ?? DateTime.UtcNow.Date.AddDays(-13);
        var toDate = filters.CreatedToUtc?.Date ?? DateTime.UtcNow.Date;
        if (toDate < fromDate)
            (fromDate, toDate) = (toDate, fromDate);

        var dailyPoints = Enumerable.Range(0, (toDate - fromDate).Days + 1)
            .Select(offset => fromDate.AddDays(offset))
            .Select(date => new SupportTicketChartPoint(date.ToString("MM/dd"), tickets.Count(ticket => ticket.CreatedAt.Date == date)))
            .ToList();

        return new SupportTicketDashboardViewModel
        {
            TotalTickets = tickets.Count,
            OpenTickets = tickets.Count(ticket => ticket.Status == SupportTicketStatus.Open),
            InProgressTickets = tickets.Count(ticket => ticket.Status == SupportTicketStatus.InProgress),
            WaitingForUserTickets = tickets.Count(ticket => ticket.Status == SupportTicketStatus.WaitingForUser),
            ClosedTickets = tickets.Count(ticket => ticket.Status == SupportTicketStatus.Closed),
            ReopenedTickets = tickets.Count(ticket => ticket.Status == SupportTicketStatus.Reopened),
            FinancialTickets = tickets.Count(ticket => ticket.Category == SupportTicketCategory.FinancialProblem),
            EventTickets = tickets.Count(ticket => ticket.Category == SupportTicketCategory.EventProblem),
            QuestionTickets = tickets.Count(ticket => ticket.Category == SupportTicketCategory.GeneralQuestion),
            UnassignedTickets = tickets.Count(ticket => ticket.AssignedSupportUserId is null),
            StatusChart = statusPoints,
            CategoryChart = categoryPoints,
            DailyCreatedChart = dailyPoints
        };
    }

    public async Task<SupportTicketDetailDto> GetTicketAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken = default)
    {
        var ticket = await _sender.Send(new GetSupportTicketQuery(currentUser.Id, ticketId), cancellationToken);
        EnsureTicketAccessForAdminPanel(currentUser, ticket);
        return ticket;
    }

    public async Task<SupportTicketDetailDto> CreateTicketAsync(MockUser currentUser, string title, SupportTicketCategory category, string body, IReadOnlyList<SupportTicketAttachmentInput> attachments, CancellationToken cancellationToken = default)
    {
        return await _sender.Send(new CreateSupportTicketCommand(currentUser.Id, title, category, body, attachments), cancellationToken);
    }

    public async Task<SupportTicketDetailDto> ReplyAsync(MockUser currentUser, long ticketId, string body, IReadOnlyList<SupportTicketAttachmentInput> attachments, long? representedUserId, CancellationToken cancellationToken = default)
    {
        await EnsureTicketActionAccessAsync(currentUser, ticketId, cancellationToken);
        return await _sender.Send(new ReplyToSupportTicketCommand(currentUser.Id, ticketId, body, attachments, representedUserId), cancellationToken);
    }

    public async Task<SupportTicketDetailDto> ChangeStatusAsync(MockUser currentUser, long ticketId, SupportTicketStatus status, string? note, CancellationToken cancellationToken = default)
    {
        await EnsureTicketActionAccessAsync(currentUser, ticketId, cancellationToken);
        return await _sender.Send(new ChangeSupportTicketStatusCommand(currentUser.Id, ticketId, status, note), cancellationToken);
    }

    public async Task<SupportTicketDetailDto> ReassignAsync(MockUser currentUser, long ticketId, long? assigneeUserId, string? note, CancellationToken cancellationToken = default)
    {
        return await _sender.Send(new ReassignSupportTicketCommand(currentUser.Id, ticketId, assigneeUserId, note), cancellationToken);
    }

    public async Task<IReadOnlyList<(long Id, string DisplayName)>> GetSupportUsersAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        if (currentUser.Role is not (AdminRole.Admin or AdminRole.SupportTeam))
            return Array.Empty<(long Id, string DisplayName)>();

        if (currentUser.Role == AdminRole.SupportTeam)
        {
            return new[] { (currentUser.Id, string.IsNullOrWhiteSpace(currentUser.FullName) ? currentUser.Mobile : currentUser.FullName) };
        }

        var users = await _users.ListActiveSupportUsersAsync(cancellationToken);
        return users.Select(user => (user.Id, DatabaseModelMapper.ResolveUserDisplayName(user))).ToList();
    }

    public async Task<SupportSubmitterFinanceContext> GetSubmitterFinanceAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketEntityForContextAsync(currentUser, ticketId, cancellationToken);
        var userId = ticket.SubmitterUserId;
        var account = await _db.BalanceAccounts
            .Include(account => account.Transactions)
            .AsNoTracking()
            .FirstOrDefaultAsync(account => account.UserId == userId, cancellationToken);

        var eventIds = (account?.Transactions.Select(transaction => transaction.DatingEventId) ?? Array.Empty<long?>())
            .Where(item => item.HasValue)
            .Select(item => item!.Value)
            .Distinct()
            .ToList();

        var payments = await _db.OnlinePayments
            .AsNoTracking()
            .Where(payment => payment.UserId == userId)
            .OrderByDescending(payment => payment.CreatedAt)
            .Take(10)
            .Select(payment => new SupportSubmitterPaymentItem(
                payment.Id,
                payment.Amount,
                payment.CurrencyCode,
                payment.ReportingAmountIrr,
                payment.GatewayName,
                payment.TrackingCode,
                payment.Status,
                payment.DatingEventId,
                payment.DatingEvent == null ? "بدون رویداد" : payment.DatingEvent.Title,
                payment.EventTicketId,
                payment.CreatedAt,
                payment.PaidAtUtc))
            .ToListAsync(cancellationToken);

        eventIds.AddRange(payments.Where(payment => payment.EventId.HasValue).Select(payment => payment.EventId!.Value));
        var eventTitles = await _db.DatingEvents
            .AsNoTracking()
            .Where(item => eventIds.Contains(item.Id))
            .ToDictionaryAsync(item => item.Id, item => item.Title, cancellationToken);

        var transactions = (account?.Transactions ?? Array.Empty<BalanceTransaction>())
            .OrderByDescending(transaction => transaction.CreatedAt)
            .Take(12)
            .Select(transaction => new SupportSubmitterTransactionItem(
                transaction.Id,
                transaction.Amount,
                transaction.CurrencyCode,
                transaction.ReportingAmountIrr,
                transaction.Type,
                transaction.Description,
                transaction.DatingEventId,
                transaction.DatingEventId is long eventId && eventTitles.TryGetValue(eventId, out var eventTitle) ? eventTitle : "بدون رویداد",
                transaction.CreatedAt))
            .ToList();

        return new SupportSubmitterFinanceContext(account?.Balance ?? 0m, account?.ReportingCurrencyCode ?? "IRR", transactions, payments);
    }

    public async Task<IReadOnlyList<SupportSubmitterEventBookingItem>> GetSubmitterEventsAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketEntityForContextAsync(currentUser, ticketId, cancellationToken);
        return await _db.EventTickets
            .AsNoTracking()
            .Include(eventTicket => eventTicket.DatingEvent).ThenInclude(datingEvent => datingEvent.EventPlannerUser).ThenInclude(user => user.Profile)
            .Where(eventTicket => eventTicket.UserId == ticket.SubmitterUserId)
            .OrderByDescending(eventTicket => eventTicket.CreatedAt)
            .Take(12)
            .Select(eventTicket => new SupportSubmitterEventBookingItem(
                eventTicket.Id,
                eventTicket.DatingEventId,
                eventTicket.DatingEvent.Title,
                eventTicket.DatingEvent.EventPlannerUser.Profile == null ? eventTicket.DatingEvent.EventPlannerUser.MobileNumber : eventTicket.DatingEvent.EventPlannerUser.Profile.DisplayName,
                eventTicket.DatingEvent.DateTimeStart,
                eventTicket.Price,
                eventTicket.CurrencyCode,
                eventTicket.IsRemoved ? "حذف و بازگشت وجه" : eventTicket.IsRefunded ? "بازگشت وجه" : eventTicket.DatingEvent.IsCancelled ? "رویداد لغو شده" : "فعال",
                eventTicket.CreatedAt))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupportTicketListItemDto>> GetSubmitterPreviousTicketsAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken = default)
    {
        var ticket = await GetTicketEntityForContextAsync(currentUser, ticketId, cancellationToken);
        var previousTickets = await _db.SupportTickets
            .AsNoTracking()
            .Include(item => item.SubmitterUser).ThenInclude(user => user.Profile)
            .Include(item => item.AssignedSupportUser).ThenInclude(user => user!.Profile)
            .Where(item => item.SubmitterUserId == ticket.SubmitterUserId && item.Id != ticket.Id)
            .OrderByDescending(item => item.UpdatedAt ?? item.CreatedAt)
            .Take(10)
            .ToListAsync(cancellationToken);

        return previousTickets.Select(SupportTicketDtoMapper.ToListItem).ToList();
    }

    private static void EnsureSupportOrAdmin(MockUser currentUser)
    {
        if (currentUser.Role is not (AdminRole.Admin or AdminRole.SupportTeam))
            throw new UnauthorizedAccessException("Only support team and admin users can view the support dashboard.");
    }

    private static long? ResolveAssigneeFilter(MockUser currentUser, long? requestedAssigneeUserId) =>
        currentUser.Role == AdminRole.SupportTeam ? currentUser.Id : requestedAssigneeUserId;

    private static void EnsureTicketAccessForAdminPanel(MockUser currentUser, SupportTicketDetailDto ticket)
    {
        if (currentUser.Role == AdminRole.Admin || currentUser.Id == ticket.Submitter.UserId)
            return;

        if (currentUser.Role == AdminRole.SupportTeam && ticket.AssignedSupportUserId == currentUser.Id)
            return;

        throw new UnauthorizedAccessException("این تیکت به شما تخصیص داده نشده است.");
    }

    private async Task EnsureTicketActionAccessAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken)
    {
        if (currentUser.Role == AdminRole.Admin)
            return;

        if (currentUser.Role != AdminRole.SupportTeam)
            return;

        var isAssigned = await _db.SupportTickets
            .AnyAsync(ticket => ticket.Id == ticketId && ticket.AssignedSupportUserId == currentUser.Id, cancellationToken);
        if (!isAssigned)
            throw new UnauthorizedAccessException("این تیکت به شما تخصیص داده نشده است.");
    }

    private async Task<SupportTicket> GetTicketEntityForContextAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken)
    {
        EnsureSupportOrAdmin(currentUser);
        var ticket = await _db.SupportTickets
            .AsNoTracking()
            .Include(item => item.SubmitterUser).ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(item => item.Id == ticketId, cancellationToken)
            ?? throw new InvalidOperationException("تیکت مورد نظر پیدا نشد.");

        if (currentUser.Role == AdminRole.SupportTeam && ticket.AssignedSupportUserId != currentUser.Id)
            throw new UnauthorizedAccessException("این تیکت به شما تخصیص داده نشده است.");

        return ticket;
    }

    private static string ToPersianStatus(SupportTicketStatus status) => status switch
    {
        SupportTicketStatus.Open => "باز",
        SupportTicketStatus.InProgress => "در حال رسیدگی",
        SupportTicketStatus.WaitingForUser => "منتظر ثبت‌کننده",
        SupportTicketStatus.Closed => "بسته",
        SupportTicketStatus.Reopened => "بازگشایی شده",
        _ => status.ToString()
    };

    private static string ToPersianCategory(SupportTicketCategory category) => category switch
    {
        SupportTicketCategory.FinancialProblem => "مشکل مالی",
        SupportTicketCategory.EventProblem => "مشکل رویداد",
        SupportTicketCategory.GeneralQuestion => "سوال عمومی",
        _ => category.ToString()
    };
}
