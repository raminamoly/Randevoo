using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IUsersApiClient
{
    Task<IReadOnlyList<MockUser>> GetUsersAsync(CancellationToken cancellationToken = default);

    Task<MockUser?> GetUserAsync(long id, CancellationToken cancellationToken = default);

    Task<MockUser> UpsertUserAsync(UserUpsertInput input, long? existingUserId = null, CancellationToken cancellationToken = default);
}
