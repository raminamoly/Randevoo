namespace Randevoo.AdminPanel.Models.Common;

public sealed class DashboardStats
{
    public int UsersCount { get; set; }

    public int PlannerCount { get; set; }

    public int MyEventsCount { get; set; }

    public int PendingEventsCount { get; set; }

    public int LiveEventsCount { get; set; }

    public int ClosedEventsCount { get; set; }

    public decimal TotalTicketSales { get; set; }

    public decimal PendingRevenue { get; set; }
}
