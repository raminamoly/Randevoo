using Randevoo.ControlCenter.Models.Auth;
using Randevoo.ControlCenter.Models.Common;
using Randevoo.ControlCenter.Services.MockData;

namespace Randevoo.ControlCenter.Services.ApiClients;

public sealed class MockDashboardApiClient(ControlCenterMockData data) : IDashboardApiClient
{
    public Task<IReadOnlyList<DashboardMetric>> GetDashboardMetricsAsync(ControlCenterRole role, CancellationToken cancellationToken = default)
    {
        var metrics = role == ControlCenterRole.Admin ? data.AdminMetrics : data.PlannerMetrics;
        return Task.FromResult(metrics);
    }
}
