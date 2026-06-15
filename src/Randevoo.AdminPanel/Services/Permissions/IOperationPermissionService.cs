using Randevoo.AdminPanel.Models.Auth;

namespace Randevoo.AdminPanel.Services.Permissions;

public interface IOperationPermissionService
{
    Task<IReadOnlySet<string>> GetAllowedActionsAsync(MockUser user, string entity, CancellationToken cancellationToken = default);

    Task<bool> IsAllowedAsync(MockUser user, string entity, string action, CancellationToken cancellationToken = default);
}
