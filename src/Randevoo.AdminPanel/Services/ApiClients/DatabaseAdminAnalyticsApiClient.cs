using System.Globalization;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Dashboard;
using Randevoo.AdminPanel.Models.Logs;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseAdminAnalyticsApiClient : IAdminAnalyticsApiClient
{
    private readonly RandevooDbContext _db;

    public DatabaseAdminAnalyticsApiClient(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<UserDashboardReport> GetUserDashboardAsync(MockUser currentUser, DashboardDateRangeValue range, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var users = await _db.Users
            .Include(user => user.Profile)
                .ThenInclude(profile => profile!.Images)
            .Include(user => user.Profile)
                .ThenInclude(profile => profile!.Interests)
            .Where(user => user.Role == UserRole.EndUser)
            .ToListAsync(cancellationToken);

        var tickets = await _db.EventTickets
            .Where(ticket => !ticket.IsRefunded && !ticket.IsRemoved)
            .Include(ticket => ticket.DatingEvent)
            .ToListAsync(cancellationToken);

        var auditLogs = await ApplyRange(_db.AuditLogs.AsNoTracking(), range, log => log.CreatedAt)
            .Where(log => log.ActorUserId != null)
            .ToListAsync(cancellationToken);

        var onlineNowSince = DateTime.UtcNow.AddMinutes(-30);
        var onlineNow = await _db.AuditLogs
            .Where(log => log.ActorUserId != null && log.CreatedAt >= onlineNowSince)
            .Select(log => log.ActorUserId!.Value)
            .Distinct()
            .CountAsync(cancellationToken);

        var rangeTickets = ApplyRange(tickets.AsEnumerable(), range, ticket => ticket.CreatedAt).ToList();
        var rangeUsers = ApplyRange(users.AsEnumerable(), range, user => user.CreatedAt).ToList();

        var averageUsersPerEvent = rangeTickets
            .GroupBy(ticket => ticket.DatingEventId)
            .Select(group => group.Count())
            .DefaultIfEmpty(0)
            .Average();

        var timeSpentLogs = auditLogs.Where(log => string.Equals(log.LogType, "time_spent", StringComparison.OrdinalIgnoreCase)).ToList();
        var clickLogs = auditLogs.Where(log => string.Equals(log.LogType, "click", StringComparison.OrdinalIgnoreCase)).ToList();

        return new UserDashboardReport
        {
            Metrics =
            [
                new SummaryMetric { Label = "کل شرکت‌کنندگان", Value = users.Count.ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "شرکت‌کنندگان فعال", Value = auditLogs.Select(log => log.ActorUserId!.Value).Distinct().Count().ToString("N0", CultureInfo.InvariantCulture), Hint = range.Label },
                new SummaryMetric { Label = "پروفایل ناقص", Value = users.Count(IsProfileIncomplete).ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "شرکت‌کنندگان وفادار", Value = rangeTickets.GroupBy(ticket => ticket.UserId).Count(group => group.Count() > 1).ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "آنلاین در ۳۰ دقیقه اخیر", Value = onlineNow.ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "میانگین شرکت‌کننده به رویداد", Value = averageUsersPerEvent.ToString("N1", CultureInfo.InvariantCulture) }
            ],
            SignupTrend = BuildTrend(rangeUsers, user => user.CreatedAt, group => group.Count(), range),
            DailyActiveUsers = BuildTrend(auditLogs, log => log.CreatedAt, group => group.Select(item => item.ActorUserId).Distinct().Count(), range),
            TimeSpentTrend = BuildTrend(timeSpentLogs, log => log.CreatedAt, group => group.Sum(GetDurationSeconds), range),
            ClickTrend = BuildTrend(clickLogs, log => log.CreatedAt, group => group.Count(), range)
        };
    }

    public async Task<SalesDashboardReport> GetSalesDashboardAsync(MockUser currentUser, DashboardDateRangeValue range, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var tickets = await ApplyRange(
                _db.EventTickets
                    .Where(ticket => !ticket.IsRefunded && !ticket.IsRemoved)
                    .Include(ticket => ticket.DatingEvent)
                        .ThenInclude(datingEvent => datingEvent.EventMode)
                    .Include(ticket => ticket.DatingEvent)
                        .ThenInclude(datingEvent => datingEvent.EventType)
                    .Include(ticket => ticket.DatingEvent)
                        .ThenInclude(datingEvent => datingEvent.City),
                range,
                ticket => ticket.CreatedAt)
            .ToListAsync(cancellationToken);

        return new SalesDashboardReport
        {
            Metrics =
            [
                new SummaryMetric { Label = "کل بلیت فروخته شده", Value = tickets.Count.ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "کل خریداران", Value = tickets.Select(ticket => ticket.UserId).Distinct().Count().ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "مجموع فروش معادل ریالی", Value = tickets.Sum(ticket => ticket.ReportingPriceIrr).ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "میانگین قیمت بلیت معادل ریالی", Value = tickets.Select(ticket => ticket.ReportingPriceIrr).DefaultIfEmpty(0m).Average().ToString("N0", CultureInfo.InvariantCulture) }
            ],
            PurchaseByMode = BuildPie(tickets, ticket => ticket.DatingEvent.EventMode?.Name ?? "نامشخص", ticket => ticket.ReportingPriceIrr),
            PurchaseByType = BuildPie(tickets, ticket => ticket.DatingEvent.EventType.Name, ticket => ticket.ReportingPriceIrr),
            PurchaseByCity = BuildPie(tickets, ticket => ticket.DatingEvent.City?.Name ?? ticket.DatingEvent.Location.City, ticket => ticket.ReportingPriceIrr),
            SalesTrend = BuildTrend(tickets, ticket => ticket.CreatedAt, group => group.Sum(item => item.ReportingPriceIrr), range),
            TopEvents = tickets
                .GroupBy(ticket => ticket.DatingEvent.Title)
                .Select(group => new RankingItem
                {
                    Label = group.Key,
                    Value = group.Sum(item => item.ReportingPriceIrr),
                    Meta = $"{group.Count():N0} بلیت"
                })
                .OrderByDescending(item => item.Value)
                .Take(8)
                .ToList()
        };
    }

    public async Task<EventDashboardReport> GetEventDashboardAsync(MockUser currentUser, DashboardDateRangeValue range, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var events = await ApplyRange(
                _db.DatingEvents
                    .Include(datingEvent => datingEvent.EventType)
                    .Include(datingEvent => datingEvent.City)
                    .Include(datingEvent => datingEvent.EventPlannerUser)
                        .ThenInclude(user => user.Profile),
                range,
                datingEvent => datingEvent.CreatedAt)
            .ToListAsync(cancellationToken);

        return new EventDashboardReport
        {
            Metrics =
            [
                new SummaryMetric { Label = "کل رویدادها", Value = events.Count.ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "در حال فروش", Value = events.Count(item => item.IsOpenForSell && !item.IsCancelled).ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "تعداد برگزارکننده", Value = events.Select(item => item.EventPlannerUserId).Distinct().Count().ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "رویدادهای لغو شده", Value = events.Count(item => item.IsCancelled).ToString("N0", CultureInfo.InvariantCulture) }
            ],
            EventsByType = BuildPie(events, item => item.EventType.Name, item => 1m),
            EventsByCity = BuildPie(events, item => item.City?.Name ?? item.Location.City, item => 1m),
            EventsByStatus = BuildPie(events, ResolveEventStatus, item => 1m),
            CreatedTrend = BuildTrend(events, item => item.CreatedAt, group => group.Count(), range),
            TopPlanners = events
                .GroupBy(item => DatabaseModelMapper.ResolveUserDisplayName(item.EventPlannerUser))
                .Select(group => new RankingItem
                {
                    Label = group.Key,
                    Value = group.Count(),
                    Meta = $"{group.Count(item => item.IsOpenForSell && !item.IsCancelled):N0} فعال"
                })
                .OrderByDescending(item => item.Value)
                .Take(8)
                .ToList()
        };
    }

    public async Task<MoneyDashboardReport> GetMoneyDashboardAsync(MockUser currentUser, DashboardDateRangeValue range, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var tickets = await ApplyRange(
                _db.EventTickets
                    .Where(ticket => !ticket.IsRefunded && !ticket.IsRemoved)
                    .Include(ticket => ticket.User)
                        .ThenInclude(user => user.Profile)
                    .Include(ticket => ticket.DatingEvent)
                        .ThenInclude(datingEvent => datingEvent.EventPlannerUser)
                            .ThenInclude(user => user.Profile)
                    .Include(ticket => ticket.DatingEvent)
                        .ThenInclude(datingEvent => datingEvent.EventType),
                range,
                ticket => ticket.CreatedAt)
            .ToListAsync(cancellationToken);

        var platformIncomeRows = tickets
            .Select(ticket => new
            {
                Ticket = ticket,
                PlatformIncome = ticket.ReportingPriceIrr * ticket.DatingEvent.EventPlannerCommissionPercent / 100m,
                PlannerIncome = ticket.ReportingPriceIrr * (100m - ticket.DatingEvent.EventPlannerCommissionPercent) / 100m
            })
            .ToList();

        return new MoneyDashboardReport
        {
            Metrics =
            [
                new SummaryMetric { Label = "درآمد کل پلتفرم", Value = platformIncomeRows.Sum(item => item.PlatformIncome).ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "درآمد برگزارکنندگان", Value = platformIncomeRows.Sum(item => item.PlannerIncome).ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "میانگین درآمد پلتفرم به شرکت‌کننده", Value = platformIncomeRows.GroupBy(item => item.Ticket.UserId).Select(group => group.Sum(item => item.PlatformIncome)).DefaultIfEmpty(0m).Average().ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "میانگین کمیسیون", Value = tickets.Select(ticket => ticket.DatingEvent.EventPlannerCommissionPercent).DefaultIfEmpty(0m).Average().ToString("N1", CultureInfo.InvariantCulture) + "%" }
            ],
            PlatformIncomeByType = BuildPie(platformIncomeRows, item => item.Ticket.DatingEvent.EventType.Name, item => item.PlatformIncome),
            PlatformIncomeByPlanner = BuildPie(platformIncomeRows, item => DatabaseModelMapper.ResolveUserDisplayName(item.Ticket.DatingEvent.EventPlannerUser), item => item.PlatformIncome),
            PlannerIncomeShare = BuildPie(platformIncomeRows, item => DatabaseModelMapper.ResolveUserDisplayName(item.Ticket.DatingEvent.EventPlannerUser), item => item.PlannerIncome),
            PlatformIncomeTrend = BuildTrend(platformIncomeRows, item => item.Ticket.CreatedAt, group => group.Sum(item => item.PlatformIncome), range),
            IncomePerUser = platformIncomeRows
                .GroupBy(item => DatabaseModelMapper.ResolveUserDisplayName(item.Ticket.User))
                .Select(group => new RankingItem
                {
                    Label = group.Key,
                    Value = group.Sum(item => item.PlatformIncome),
                    Meta = $"{group.Count():N0} خرید"
                })
                .OrderByDescending(item => item.Value)
                .Take(8)
                .ToList(),
            TopRevenueEvents = platformIncomeRows
                .GroupBy(item => item.Ticket.DatingEvent.Title)
                .Select(group => new RankingItem
                {
                    Label = group.Key,
                    Value = group.Sum(item => item.PlatformIncome),
                    Meta = group.Sum(item => item.Ticket.ReportingPriceIrr).ToString("N0", CultureInfo.InvariantCulture)
                })
                .OrderByDescending(item => item.Value)
                .Take(8)
                .ToList(),
            TopRevenuePlanners = platformIncomeRows
                .GroupBy(item => DatabaseModelMapper.ResolveUserDisplayName(item.Ticket.DatingEvent.EventPlannerUser))
                .Select(group => new RankingItem
                {
                    Label = group.Key,
                    Value = group.Sum(item => item.PlatformIncome),
                    Meta = group.Sum(item => item.PlannerIncome).ToString("N0", CultureInfo.InvariantCulture)
                })
                .OrderByDescending(item => item.Value)
                .Take(8)
                .ToList()
        };
    }

    public async Task<AuditLogListResult> GetAuditLogsAsync(MockUser currentUser, AuditLogFilter filter, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var query =
            from log in _db.AuditLogs.AsNoTracking()
            join user in _db.Users.AsNoTracking().Include(item => item.Profile) on log.ActorUserId equals user.Id into userGroup
            from user in userGroup.DefaultIfEmpty()
            select new
            {
                Log = log,
                UserDisplayName = log.ActorDisplayName
                    ?? (user != null ? DatabaseModelMapper.ResolveUserDisplayName(user) : "سیستم"),
                UserRole = log.ActorRole ?? (user != null ? user.Role.ToString() : "System")
            };

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(item =>
                item.UserDisplayName.Contains(search)
                || item.Log.Action.Contains(search)
                || (item.Log.Description != null && item.Log.Description.Contains(search))
                || (item.Log.RequestPath != null && item.Log.RequestPath.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Role))
            query = query.Where(item => item.UserRole == filter.Role);
        if (!string.IsNullOrWhiteSpace(filter.Action))
            query = query.Where(item => item.Log.Action == filter.Action);
        if (!string.IsNullOrWhiteSpace(filter.LogType))
            query = query.Where(item => item.Log.LogType == filter.LogType);
        if (!string.IsNullOrWhiteSpace(filter.Module))
            query = query.Where(item => item.Log.Module == filter.Module);
        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(item => item.Log.Status == filter.Status);
        if (!string.IsNullOrWhiteSpace(filter.IpAddress))
            query = query.Where(item => item.Log.IpAddress == filter.IpAddress);
        if (filter.StartUtc.HasValue)
            query = query.Where(item => item.Log.CreatedAt >= filter.StartUtc.Value);
        if (filter.EndUtc.HasValue)
            query = query.Where(item => item.Log.CreatedAt <= filter.EndUtc.Value);

        query = string.Equals(filter.Sort, "oldest", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(item => item.Log.CreatedAt)
            : query.OrderByDescending(item => item.Log.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((Math.Max(filter.PageNumber, 1) - 1) * Math.Max(filter.PageSize, 1))
            .Take(Math.Max(filter.PageSize, 1))
            .Select(item => new AuditLogListItem
            {
                Id = item.Log.Id,
                UserDisplayName = item.UserDisplayName,
                UserRole = item.UserRole,
                Action = item.Log.Action,
                LogType = item.Log.LogType,
                Module = item.Log.Module ?? "system",
                Description = item.Log.Description ?? item.Log.Reason ?? string.Empty,
                IpAddress = item.Log.IpAddress,
                Status = item.Log.Status,
                CreatedAtUtc = item.Log.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new AuditLogListResult
        {
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<SmsQueueListResult> GetSmsQueueAsync(MockUser currentUser, SmsQueueListFilter filter, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(currentUser);

        var pageNumber = Math.Max(filter.PageNumber, 1);
        var pageSize = Math.Clamp(filter.PageSize, 10, 100);

        var query = _db.SmsQueueItems
            .AsNoTracking()
            .Include(item => item.RecipientUser)
                .ThenInclude(user => user.Profile)
            .Include(item => item.DatingEvent)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(item =>
                item.MobileNumber.Contains(search)
                || item.Message.Contains(search)
                || item.DatingEvent.Title.Contains(search)
                || (item.RecipientUser.Profile != null && item.RecipientUser.Profile.DisplayName.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(filter.Status) && Enum.TryParse<SmsQueueItemStatus>(filter.Status, true, out var status))
            query = query.Where(item => item.Status == status);
        if (filter.EventId.HasValue)
            query = query.Where(item => item.DatingEventId == filter.EventId.Value);

        query = string.Equals(filter.Sort, "oldest", StringComparison.OrdinalIgnoreCase)
            ? query.OrderBy(item => item.CreatedAt)
            : query.OrderByDescending(item => item.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new
            {
                Id = item.Id,
                ProfileDisplayName = item.RecipientUser.Profile != null ? item.RecipientUser.Profile.DisplayName : null,
                Mobile = item.RecipientUser.MobileNumber,
                MobileNumber = item.MobileNumber,
                EventTitle = item.DatingEvent.Title,
                Message = item.Message,
                Status = item.Status,
                AttemptCount = item.AttemptCount,
                FailureReason = item.FailureReason,
                CreatedAtUtc = item.CreatedAt,
                PlannedSendAtUtc = item.PlannedSendAtUtc
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(item => new SmsQueueListItem
        {
            Id = item.Id,
            RecipientDisplayName = string.IsNullOrWhiteSpace(item.ProfileDisplayName) ? item.Mobile : item.ProfileDisplayName,
            MobileNumber = item.MobileNumber,
            EventTitle = item.EventTitle,
            MessagePreview = item.Message.Length > 80 ? item.Message[..80] + "..." : item.Message,
            Status = item.Status.ToString(),
            AttemptCount = item.AttemptCount,
            FailureReason = item.FailureReason,
            CreatedAtUtc = item.CreatedAtUtc,
            PlannedSendAtUtc = item.PlannedSendAtUtc
        }).ToList();

        var metricsSource = await _db.SmsQueueItems.AsNoTracking().ToListAsync(cancellationToken);

        return new SmsQueueListResult
        {
            TotalCount = totalCount,
            Metrics =
            [
                new SummaryMetric { Label = "کل پیام ها", Value = metricsSource.Count.ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "در انتظار ارسال", Value = metricsSource.Count(item => item.Status == SmsQueueItemStatus.Pending).ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "ارسال شده", Value = metricsSource.Count(item => item.Status == SmsQueueItemStatus.Sent).ToString("N0", CultureInfo.InvariantCulture) },
                new SummaryMetric { Label = "ناموفق", Value = metricsSource.Count(item => item.Status == SmsQueueItemStatus.Failed).ToString("N0", CultureInfo.InvariantCulture) }
            ],
            Items = items
        };
    }

    private static IQueryable<T> ApplyRange<T>(IQueryable<T> query, DashboardDateRangeValue range, Expression<Func<T, DateTime>> selector)
    {
        if (!range.StartUtc.HasValue)
            return query;

        var startUtc = range.StartUtc.Value;
        var endUtc = range.EndUtc;
        var parameter = selector.Parameters[0];
        var body = Expression.AndAlso(
            Expression.GreaterThanOrEqual(selector.Body, Expression.Constant(startUtc)),
            Expression.LessThanOrEqual(selector.Body, Expression.Constant(endUtc)));
        var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
        return query.Where(predicate);
    }

    private static IEnumerable<T> ApplyRange<T>(IEnumerable<T> items, DashboardDateRangeValue range, Func<T, DateTime> selector)
    {
        if (!range.StartUtc.HasValue)
            return items;

        var startUtc = range.StartUtc.Value;
        var endUtc = range.EndUtc;
        return items.Where(item => selector(item) >= startUtc && selector(item) <= endUtc);
    }

    private static List<PieSlice> BuildPie<T>(IEnumerable<T> items, Func<T, string> keySelector, Func<T, decimal> valueSelector)
    {
        return items
            .GroupBy(item => string.IsNullOrWhiteSpace(keySelector(item)) ? "نامشخص" : keySelector(item))
            .Select(group => new PieSlice
            {
                Label = group.Key,
                Value = group.Sum(valueSelector)
            })
            .OrderByDescending(item => item.Value)
            .Take(8)
            .ToList();
    }

    private static List<ChartPoint> BuildTrend<T>(IEnumerable<T> items, Func<T, DateTime> dateSelector, Func<IEnumerable<T>, decimal> aggregate, DashboardDateRangeValue range)
    {
        var useMonthlyBuckets = range.StartUtc.HasValue && (range.EndUtc - range.StartUtc.Value).TotalDays > 45;

        return items
            .GroupBy(item => Bucket(dateSelector(item), useMonthlyBuckets))
            .OrderBy(group => group.Key)
            .Select(group => new ChartPoint
            {
                Label = useMonthlyBuckets
                    ? group.Key.ToString("yyyy/MM", CultureInfo.InvariantCulture)
                    : group.Key.ToString("MM/dd", CultureInfo.InvariantCulture),
                Value = aggregate(group)
            })
            .ToList();
    }

    private static List<ChartPoint> BuildTrend<T>(IEnumerable<T> items, Func<T, DateTime> dateSelector, Func<IEnumerable<T>, int> aggregate, DashboardDateRangeValue range)
    {
        var useMonthlyBuckets = range.StartUtc.HasValue && (range.EndUtc - range.StartUtc.Value).TotalDays > 45;

        return items
            .GroupBy(item => Bucket(dateSelector(item), useMonthlyBuckets))
            .OrderBy(group => group.Key)
            .Select(group => new ChartPoint
            {
                Label = useMonthlyBuckets
                    ? group.Key.ToString("yyyy/MM", CultureInfo.InvariantCulture)
                    : group.Key.ToString("MM/dd", CultureInfo.InvariantCulture),
                Value = aggregate(group)
            })
            .ToList();
    }

    private static DateTime Bucket(DateTime value, bool monthly)
        => monthly
            ? new DateTime(value.Year, value.Month, 1, 0, 0, 0, DateTimeKind.Utc)
            : value.Date;

    private static bool IsProfileIncomplete(User user)
    {
        var profile = user.Profile;
        if (profile is null)
            return true;

        return profile.EducationLevel == EducationLevel.NotSpecified
            || profile.Images.Count == 0
            || profile.Interests.Count == 0
            || string.IsNullOrWhiteSpace(profile.Location.City)
            || string.IsNullOrWhiteSpace(profile.Location.Country);
    }

    private static decimal GetDurationSeconds(AuditLog log)
    {
        if (string.IsNullOrWhiteSpace(log.MetadataJson))
            return 0m;

        try
        {
            using var document = JsonDocument.Parse(log.MetadataJson);
            if (document.RootElement.TryGetProperty("durationSeconds", out var durationElement)
                && durationElement.TryGetDecimal(out var duration))
            {
                return duration;
            }
        }
        catch (JsonException)
        {
        }

        return 0m;
    }

    private static string ResolveEventStatus(DatingEvent datingEvent)
    {
        if (datingEvent.IsCancelled)
            return "لغو شده";
        if (datingEvent.DateTimeEnd <= DateTime.UtcNow)
            return "بسته شده";
        if (datingEvent.IsOpenForSell)
            return "در حال فروش";

        return "پیش نویس";
    }

    private static void EnsureAdmin(MockUser currentUser)
    {
        if (currentUser.Role != AdminRole.Admin)
            throw new InvalidOperationException("این بخش فقط برای مدیر قابل استفاده است.");
    }
}
