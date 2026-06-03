using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.MockData;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class MockPlannerProfilesApiClient : IPlannerProfilesApiClient
{
    private readonly AdminPanelStore _store;

    public MockPlannerProfilesApiClient(AdminPanelStore store)
    {
        _store = store;
    }

    public Task<PlannerProfileViewModel?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.FindPlannerProfile(userId));

    public Task<PlannerProfileViewModel?> GetCurrentAsync(MockUser currentUser, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.FindPlannerProfile(currentUser.Id));

    public Task<PlannerProfileViewModel> UpsertAsync(MockUser currentUser, PlannerProfileInput input, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.UpsertPlannerProfile(currentUser, input));
}
