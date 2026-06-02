namespace Randevoo.ControlCenter.Models.Common;

public sealed record DashboardMetric(
    string Label,
    string Value,
    string Detail,
    string Icon,
    string AccentColor);
