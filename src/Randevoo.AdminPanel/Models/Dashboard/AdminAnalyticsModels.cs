namespace Randevoo.AdminPanel.Models.Dashboard;

public sealed class SummaryMetric
{
    public string Label { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string? Hint { get; set; }
}

public sealed class ChartPoint
{
    public string Label { get; set; } = string.Empty;

    public decimal Value { get; set; }
}

public sealed class PieSlice
{
    public string Label { get; set; } = string.Empty;

    public decimal Value { get; set; }
}

public sealed class RankingItem
{
    public string Label { get; set; } = string.Empty;

    public decimal Value { get; set; }

    public string? Meta { get; set; }
}

public sealed class UserDashboardReport
{
    public List<SummaryMetric> Metrics { get; set; } = [];
    public List<ChartPoint> SignupTrend { get; set; } = [];
    public List<ChartPoint> DailyActiveUsers { get; set; } = [];
    public List<ChartPoint> TimeSpentTrend { get; set; } = [];
    public List<ChartPoint> ClickTrend { get; set; } = [];
}

public sealed class SalesDashboardReport
{
    public List<SummaryMetric> Metrics { get; set; } = [];
    public List<PieSlice> PurchaseByMode { get; set; } = [];
    public List<PieSlice> PurchaseByType { get; set; } = [];
    public List<PieSlice> PurchaseByCity { get; set; } = [];
    public List<ChartPoint> SalesTrend { get; set; } = [];
    public List<RankingItem> TopEvents { get; set; } = [];
}

public sealed class EventDashboardReport
{
    public List<SummaryMetric> Metrics { get; set; } = [];
    public List<PieSlice> EventsByType { get; set; } = [];
    public List<PieSlice> EventsByCity { get; set; } = [];
    public List<PieSlice> EventsByStatus { get; set; } = [];
    public List<ChartPoint> CreatedTrend { get; set; } = [];
    public List<RankingItem> TopPlanners { get; set; } = [];
}

public sealed class MoneyDashboardReport
{
    public List<SummaryMetric> Metrics { get; set; } = [];
    public List<PieSlice> PlatformIncomeByType { get; set; } = [];
    public List<PieSlice> PlatformIncomeByPlanner { get; set; } = [];
    public List<PieSlice> PlannerIncomeShare { get; set; } = [];
    public List<ChartPoint> PlatformIncomeTrend { get; set; } = [];
    public List<RankingItem> IncomePerUser { get; set; } = [];
    public List<RankingItem> TopRevenueEvents { get; set; } = [];
    public List<RankingItem> TopRevenuePlanners { get; set; } = [];
}
