using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Dashboard;
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
        var onlineSinceUtc = nowUtc.AddMinutes(-30);
        var trendStartUtc = nowUtc.Date.AddDays(-13);

        var eventsQuery = _db.DatingEvents.AsQueryable();
        var ticketsQuery = _db.EventTickets.Where(ticket => !ticket.IsRefunded).AsQueryable();

        if (!isAdmin)
        {
            eventsQuery = eventsQuery.Where(item => item.EventPlannerUserId == currentUser.Id);
            ticketsQuery = ticketsQuery.Where(ticket => ticket.DatingEvent.EventPlannerUserId == currentUser.Id);
        }

        var usersCount = isAdmin
            ? await _db.Users.CountAsync(cancellationToken)
            : 0;

        var onlineUsersCount = isAdmin
            ? await _db.AuditLogs
                .Where(log => log.ActorUserId.HasValue && log.CreatedAt >= onlineSinceUtc)
                .Select(log => log.ActorUserId!.Value)
                .Distinct()
                .CountAsync(cancellationToken)
            : await _db.AuditLogs
                .Where(log => log.ActorUserId.HasValue && log.CreatedAt >= onlineSinceUtc)
                .Where(log => log.ActorUserId == currentUser.Id)
                .Select(log => log.ActorUserId!.Value)
                .Distinct()
                .CountAsync(cancellationToken);

        var plannerCount = isAdmin
            ? await _db.Users.CountAsync(item => item.Role == UserRole.EventPlanner, cancellationToken)
            : 0;

        var myEventsCount = await eventsQuery.CountAsync(cancellationToken);
        var pendingEventsCount = await eventsQuery.CountAsync(item => !item.IsCancelled && !item.IsOpenForSell && item.DateTimeEnd > nowUtc, cancellationToken);
        var liveEventsCount = await eventsQuery.CountAsync(item => item.IsOpenForSell && !item.IsCancelled, cancellationToken);
        var closedEventsCount = await eventsQuery.CountAsync(item => item.IsCancelled || item.DateTimeEnd <= nowUtc, cancellationToken);
        var ticketsSoldCount = await ticketsQuery.CountAsync(cancellationToken);
        var totalTicketSales = await ticketsQuery.SumAsync(ticket => (decimal?)ticket.ReportingPriceIrr, cancellationToken) ?? 0m;
        var pendingRevenue = await ticketsQuery
            .Where(ticket => !ticket.DatingEvent.IsCancelled && ticket.DatingEvent.DateTimeEnd > nowUtc)
            .SumAsync(ticket => (decimal?)ticket.ReportingPriceIrr, cancellationToken) ?? 0m;

        var revenueRows = await ticketsQuery
            .Where(ticket => ticket.CreatedAt >= trendStartUtc)
            .GroupBy(ticket => ticket.CreatedAt.Date)
            .Select(group => new { Date = group.Key, Value = group.Sum(ticket => ticket.ReportingPriceIrr) })
            .ToListAsync(cancellationToken);

        var eventCreatedRows = await eventsQuery
            .Where(item => item.CreatedAt >= trendStartUtc)
            .GroupBy(item => item.CreatedAt.Date)
            .Select(group => new { Date = group.Key, Value = group.Count() })
            .ToListAsync(cancellationToken);

        var eventTypeBreakdown = await eventsQuery
            .GroupBy(item => item.EventType.Name)
            .Select(group => new PieSlice
            {
                Label = group.Key,
                Value = group.Count()
            })
            .OrderByDescending(item => item.Value)
            .Take(6)
            .ToListAsync(cancellationToken);

        var cityEventRows = await eventsQuery
            .Where(item => item.CityId.HasValue && item.City != null && item.Country != null)
            .GroupBy(item => new
            {
                CityId = item.CityId!.Value,
                City = item.City!.Name,
                Country = item.Country!.Name,
                item.City.Latitude,
                item.City.Longitude
            })
            .Select(group => new DashboardMapPoint
            {
                CityId = group.Key.CityId,
                City = group.Key.City,
                Country = group.Key.Country,
                Latitude = group.Key.Latitude,
                Longitude = group.Key.Longitude,
                EventCount = group.Count(),
                SellingCount = group.Count(item => item.IsOpenForSell && !item.IsCancelled && item.DateTimeEnd > nowUtc)
            })
            .OrderByDescending(item => item.EventCount)
            .Take(12)
            .ToListAsync(cancellationToken);

        var cityTicketRows = await ticketsQuery
            .Where(ticket => ticket.DatingEvent.CityId.HasValue)
            .GroupBy(ticket => ticket.DatingEvent.CityId!.Value)
            .Select(group => new
            {
                CityId = group.Key,
                TicketCount = group.Count(),
                Revenue = group.Sum(ticket => ticket.ReportingPriceIrr)
            })
            .ToDictionaryAsync(item => item.CityId, cancellationToken);

        foreach (var point in cityEventRows)
        {
            if (!cityTicketRows.TryGetValue(point.CityId, out var ticketRow))
                continue;

            point.TicketCount = ticketRow.TicketCount;
            point.Revenue = ticketRow.Revenue;
        }

        return new DashboardStats
        {
            UsersCount = usersCount,
            OnlineUsersCount = onlineUsersCount,
            PlannerCount = plannerCount,
            MyEventsCount = myEventsCount,
            PendingEventsCount = pendingEventsCount,
            LiveEventsCount = liveEventsCount,
            ClosedEventsCount = closedEventsCount,
            TicketsSoldCount = ticketsSoldCount,
            TotalTicketSales = totalTicketSales,
            PendingRevenue = pendingRevenue,
            EventStatusBreakdown =
            [
                new PieSlice { Label = "پیش نویس", Value = pendingEventsCount },
                new PieSlice { Label = "در حال فروش", Value = liveEventsCount },
                new PieSlice { Label = "تمام شده", Value = await eventsQuery.CountAsync(item => !item.IsCancelled && item.DateTimeEnd <= nowUtc, cancellationToken) },
                new PieSlice { Label = "لغو شده", Value = await eventsQuery.CountAsync(item => item.IsCancelled, cancellationToken) }
            ],
            EventTypeBreakdown = eventTypeBreakdown,
            RevenueTrend = BuildDailyTrend(trendStartUtc, revenueRows.Select(item => (item.Date, item.Value))),
            EventCreatedTrend = BuildDailyTrend(trendStartUtc, eventCreatedRows.Select(item => (item.Date, (decimal)item.Value))),
            LocationPoints = cityEventRows
        };
    }

    private static List<ChartPoint> BuildDailyTrend(DateTime startDateUtc, IEnumerable<(DateTime Date, decimal Value)> rows)
    {
        var values = rows.ToDictionary(item => item.Date.Date, item => item.Value);

        return Enumerable.Range(0, 14)
            .Select(offset => startDateUtc.Date.AddDays(offset))
            .Select(date => new ChartPoint
            {
                Label = date.ToString("MM/dd"),
                Value = values.TryGetValue(date, out var value) ? value : 0m
            })
            .ToList();
    }
}
