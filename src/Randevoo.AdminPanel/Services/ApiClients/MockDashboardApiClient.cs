using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.MockData;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class MockDashboardApiClient : IDashboardApiClient
{
    private readonly AdminPanelStore _store;

    public MockDashboardApiClient(AdminPanelStore store)
    {
        _store = store;
    }

    public Task<DashboardStats> GetStatsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.GetDashboardStats(currentUser));
}

