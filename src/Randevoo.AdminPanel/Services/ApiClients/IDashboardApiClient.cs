using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Auth;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IDashboardApiClient
{
    Task<DashboardStats> GetStatsAsync(MockUser currentUser, CancellationToken cancellationToken = default);
}

