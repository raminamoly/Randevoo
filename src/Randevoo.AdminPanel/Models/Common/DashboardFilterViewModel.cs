namespace Randevoo.AdminPanel.Models.Common;

public sealed class DashboardFilterViewModel
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string PagePath { get; set; } = string.Empty;

    public string CurrentRangeKey { get; set; } = DashboardDateRange.Default;
}
