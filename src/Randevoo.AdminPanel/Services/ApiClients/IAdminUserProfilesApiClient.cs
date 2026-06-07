using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IAdminUserProfilesApiClient
{
    Task<AdminUserProfileListResult> GetProfilesAsync(MockUser admin, AdminUserProfileListFilter filter, CancellationToken cancellationToken = default);

    Task<AdminUserProfileEditor> GetEditorAsync(long userId, MockUser admin, CancellationToken cancellationToken = default);

    Task SaveProfileAsync(long userId, MockUser admin, AdminUserProfileEditorInput input, CancellationToken cancellationToken = default);

    Task AddImageAsync(long userId, MockUser admin, AdminUserProfileImageInput input, CancellationToken cancellationToken = default);

    Task RemoveImageAsync(long userId, MockUser admin, string imageUrl, CancellationToken cancellationToken = default);

    Task AddInterestAsync(long userId, MockUser admin, AdminUserProfileInterestInput input, CancellationToken cancellationToken = default);

    Task RemoveInterestAsync(long userId, MockUser admin, string interestName, CancellationToken cancellationToken = default);

    Task SendInstantSmsAsync(long userId, MockUser admin, AdminInstantSmsInput input, CancellationToken cancellationToken = default);
}
