using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IUserProfilesApiClient
{
    Task<UserProfileDetailsViewModel?> GetProfileAsync(long userId, MockUser viewer, CancellationToken cancellationToken = default);
}
