using Randevoo.AdminPanel.Models.Dashboard;

namespace Randevoo.AdminPanel.Models.Common;

public sealed class DashboardStats
{
    public int UsersCount { get; set; }

    public int OnlineUsersCount { get; set; }

    public int PlannerCount { get; set; }

    public int MyEventsCount { get; set; }

    public int PendingEventsCount { get; set; }

    public int LiveEventsCount { get; set; }

    public int ClosedEventsCount { get; set; }

    public int TicketsSoldCount { get; set; }

    public decimal TotalTicketSales { get; set; }

    public decimal PendingRevenue { get; set; }

    public List<PieSlice> EventStatusBreakdown { get; set; } = [];

    public List<PieSlice> EventTypeBreakdown { get; set; } = [];

    public List<ChartPoint> RevenueTrend { get; set; } = [];

    public List<ChartPoint> EventCreatedTrend { get; set; } = [];

    public List<DashboardMapPoint> LocationPoints { get; set; } = [];
}

public sealed class DashboardMapPoint
{
    public long CityId { get; set; }

    public string City { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public int EventCount { get; set; }

    public int SellingCount { get; set; }

    public int TicketCount { get; set; }

    public decimal Revenue { get; set; }
}
