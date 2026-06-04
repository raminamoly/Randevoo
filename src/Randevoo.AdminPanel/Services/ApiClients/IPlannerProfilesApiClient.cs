using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IPlannerProfilesApiClient
{
    Task<PlannerProfileViewModel?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);

    Task<PlannerProfileViewModel?> GetCurrentAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlannerProfileApprovalItem>> ListForApprovalAsync(CancellationToken cancellationToken = default);

    Task<PlannerProfileViewModel> UpsertAsync(MockUser currentUser, PlannerProfileInput input, CancellationToken cancellationToken = default);

    Task<PlannerProfileViewModel> ApproveAsync(MockUser adminUser, long plannerUserId, PlannerProfileApprovalInput input, CancellationToken cancellationToken = default);

    Task<PlannerProfileViewModel> RejectAsync(MockUser adminUser, long plannerUserId, string? reviewNote, CancellationToken cancellationToken = default);
}
