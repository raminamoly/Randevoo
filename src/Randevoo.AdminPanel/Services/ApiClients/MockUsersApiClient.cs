using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.MockData;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class MockUsersApiClient : IUsersApiClient
{
    private readonly AdminPanelStore _store;

    public MockUsersApiClient(AdminPanelStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<MockUser>> GetUsersAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_store.GetUsers());

    public Task<MockUser?> GetUserAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.FindUserById(id));

    public Task<MockUser> UpsertUserAsync(UserUpsertInput input, Guid? existingUserId = null, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.UpsertUser(input, existingUserId));
}

