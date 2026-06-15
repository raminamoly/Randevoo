using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.Permissions;

public sealed class DatabaseOperationPermissionService : IOperationPermissionService
{
    private readonly RandevooDbContext _db;

    public DatabaseOperationPermissionService(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlySet<string>> GetAllowedActionsAsync(MockUser user, string entity, CancellationToken cancellationToken = default)
    {
        var normalizedEntity = Normalize(entity);
        var role = MapRole(user.Role);
        var nowUtc = DateTime.UtcNow;

        var activeActions = await _db.PermissionActions
            .AsNoTracking()
            .Where(item => item.Entity == normalizedEntity && item.IsActive)
            .Select(item => item.Action)
            .ToListAsync(cancellationToken);

        var rolePermissions = await _db.RoleOperationPermissions
            .AsNoTracking()
            .Where(item => item.Role == role && item.Entity == normalizedEntity)
            .ToDictionaryAsync(item => item.Action, item => item.Allowed, cancellationToken);

        var overrides = await _db.UserOperationPermissionOverrides
            .AsNoTracking()
            .Where(item => item.UserId == user.Id
                && item.Entity == normalizedEntity
                && (item.ExpiresAtUtc == null || item.ExpiresAtUtc > nowUtc))
            .ToDictionaryAsync(item => item.Action, item => item.Allowed, cancellationToken);

        var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var action in activeActions)
        {
            if (overrides.TryGetValue(action, out var overrideAllowed))
            {
                if (overrideAllowed)
                    allowed.Add(action);

                continue;
            }

            if (rolePermissions.TryGetValue(action, out var roleAllowed) && roleAllowed)
                allowed.Add(action);
        }

        return allowed;
    }

    public async Task<bool> IsAllowedAsync(MockUser user, string entity, string action, CancellationToken cancellationToken = default)
    {
        var actions = await GetAllowedActionsAsync(user, entity, cancellationToken);
        return actions.Contains(Normalize(action));
    }

    public static UserRole MapRole(AdminRole role) => role switch
    {
        AdminRole.Admin => UserRole.Admin,
        AdminRole.EventPlanner => UserRole.EventPlanner,
        AdminRole.SupportTeam => UserRole.PlatformSupportTeam,
        _ => UserRole.EndUser
    };

    private static string Normalize(string value) => (value ?? string.Empty).Trim();
}
