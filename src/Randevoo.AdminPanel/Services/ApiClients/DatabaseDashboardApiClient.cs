using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.Infrastructure.Data;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseDashboardApiClient : IDashboardApiClient
{
    private readonly RandevooDbContext _db;

    public DatabaseDashboardApiClient(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardStats> GetStatsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        var isAdmin = currentUser.Role == AdminRole.Admin;
        var nowUtc = DateTime.UtcNow;

        var eventsQuery = _db.DatingEvents.AsQueryable();
        var ticketsQuery = _db.EventTickets.Where(ticket => !ticket.IsRefunded).AsQueryable();

        if (!isAdmin)
        {
            eventsQuery = eventsQuery.Where(item => item.EventPlannerUserId == currentUser.Id);
            ticketsQuery = ticketsQuery.Where(ticket => ticket.DatingEvent.EventPlannerUserId == currentUser.Id);
        }

        var usersCount = isAdmin
            ? await _db.Users.CountAsync(item => item.Role != UserRole.EndUser, cancellationToken)
            : 0;

        var plannerCount = isAdmin
            ? await _db.Users.CountAsync(item => item.Role == UserRole.EventPlanner, cancellationToken)
            : 0;

        var myEventsCount = await eventsQuery.CountAsync(cancellationToken);
        var pendingEventsCount = await eventsQuery.CountAsync(item => !item.IsCancelled && !item.IsOpenForSell && item.DateTimeEnd > nowUtc, cancellationToken);
        var liveEventsCount = await eventsQuery.CountAsync(item => item.IsOpenForSell && !item.IsCancelled, cancellationToken);
        var closedEventsCount = await eventsQuery.CountAsync(item => item.IsCancelled || item.DateTimeEnd <= nowUtc, cancellationToken);
        var totalTicketSales = await ticketsQuery.SumAsync(ticket => (decimal?)ticket.Price, cancellationToken) ?? 0m;
        var pendingRevenue = await ticketsQuery
            .Where(ticket => !ticket.DatingEvent.IsCancelled && ticket.DatingEvent.DateTimeEnd > nowUtc)
            .SumAsync(ticket => (decimal?)ticket.Price, cancellationToken) ?? 0m;

        return new DashboardStats
        {
            UsersCount = usersCount,
            PlannerCount = plannerCount,
            MyEventsCount = myEventsCount,
            PendingEventsCount = pendingEventsCount,
            LiveEventsCount = liveEventsCount,
            ClosedEventsCount = closedEventsCount,
            TotalTicketSales = totalTicketSales,
            PendingRevenue = pendingRevenue
        };
    }
}
