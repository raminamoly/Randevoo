namespace Randevoo.AdminPanel.Models.Common;

public static class DashboardDateRange
{
    public const string Today = "today";
    public const string LastWeek = "last-week";
    public const string LastTwoWeeks = "last-two-weeks";
    public const string LastMonth = "last-month";
    public const string LastThreeMonths = "last-three-months";
    public const string LastSixMonths = "last-six-months";
    public const string LastYear = "last-year";
    public const string YearToDate = "year-to-date";
    public const string AllTime = "all-time";
    public const string Default = LastMonth;

    public static readonly IReadOnlyList<DashboardDateRangeOption> Options =
    [
        new(Today, "امروز"),
        new(LastWeek, "هفته گذشته"),
        new(LastTwoWeeks, "دو هفته گذشته"),
        new(LastMonth, "ماه گذشته"),
        new(LastThreeMonths, "سه ماه گذشته"),
        new(LastSixMonths, "شش ماه گذشته"),
        new(LastYear, "یکسال گذشته"),
        new(YearToDate, "از اول سال"),
        new(AllTime, "از ابتدا")
    ];

    public static DashboardDateRangeValue Resolve(string? selectedKey)
    {
        var key = Options.Any(item => string.Equals(item.Key, selectedKey, StringComparison.OrdinalIgnoreCase))
            ? selectedKey!.Trim().ToLowerInvariant()
            : Default;

        var now = DateTime.UtcNow;
        var todayStart = now.Date;
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var startUtc = key switch
        {
            Today => todayStart,
            LastWeek => now.AddDays(-7),
            LastTwoWeeks => now.AddDays(-14),
            LastMonth => now.AddMonths(-1),
            LastThreeMonths => now.AddMonths(-3),
            LastSixMonths => now.AddMonths(-6),
            LastYear => now.AddYears(-1),
            YearToDate => yearStart,
            _ => (DateTime?)null
        };

        var label = Options.First(item => item.Key == key).Label;
        return new DashboardDateRangeValue(key, label, startUtc, now);
    }
}

public sealed record DashboardDateRangeOption(string Key, string Label);

public sealed record DashboardDateRangeValue(string Key, string Label, DateTime? StartUtc, DateTime EndUtc);
