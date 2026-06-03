using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IPlannerProfilesApiClient
{
    Task<PlannerProfileViewModel?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<PlannerProfileViewModel?> GetCurrentAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<PlannerProfileViewModel> UpsertAsync(MockUser currentUser, PlannerProfileInput input, CancellationToken cancellationToken = default);
}
