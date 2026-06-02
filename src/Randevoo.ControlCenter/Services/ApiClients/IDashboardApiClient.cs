using Randevoo.ControlCenter.Models.Auth;
using Randevoo.ControlCenter.Models.Common;

namespace Randevoo.ControlCenter.Services.ApiClients;

public interface IDashboardApiClient
{
    Task<IReadOnlyList<DashboardMetric>> GetDashboardMetricsAsync(ControlCenterRole role, CancellationToken cancellationToken = default);
}
