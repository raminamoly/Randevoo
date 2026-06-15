using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Entities;

namespace Randevoo.Infrastructure.Data;

public static class OperationPermissionCatalogSync
{
    public static async Task SyncOperationPermissionCatalogAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();

        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }

        await SyncAsync(db, cancellationToken);
    }

    public static async Task SyncAsync(RandevooDbContext db, CancellationToken cancellationToken = default)
    {
        var definitions = OperationPermissionCatalog.All
            .GroupBy(item => (Entity: Normalize(item.Entity), Action: Normalize(item.Action)))
            .Select(group => group.Last())
            .ToList();

        var catalogKeys = definitions
            .Select(item => $"{Normalize(item.Entity)}::{Normalize(item.Action)}")
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var existingActions = await db.PermissionActions
            .IgnoreQueryFilters()
            .ToDictionaryAsync(item => $"{Normalize(item.Entity)}::{Normalize(item.Action)}", StringComparer.OrdinalIgnoreCase, cancellationToken);

        foreach (var definition in definitions)
        {
            var key = $"{Normalize(definition.Entity)}::{Normalize(definition.Action)}";
            if (!existingActions.TryGetValue(key, out var permissionAction))
            {
                permissionAction = new PermissionAction(
                    definition.Entity,
                    definition.Action,
                    definition.Label,
                    definition.Description,
                    definition.DisplayOrder,
                    definition.EntityLabel,
                    definition.GroupKey,
                    definition.GroupLabel,
                    definition.PagePath,
                    definition.HandlerName,
                    definition.UiSurface,
                    definition.RiskLevel,
                    definition.IsSystemAction);
                db.PermissionActions.Add(permissionAction);
                existingActions[key] = permissionAction;
            }
            else
            {
                permissionAction.ApplyCatalogMetadata(
                    definition.Label,
                    definition.Description,
                    definition.DisplayOrder,
                    definition.EntityLabel,
                    definition.GroupKey,
                    definition.GroupLabel,
                    definition.PagePath,
                    definition.HandlerName,
                    definition.UiSurface,
                    definition.RiskLevel,
                    definition.IsSystemAction);
            }
        }

        foreach (var action in existingActions.Values)
        {
            var key = $"{Normalize(action.Entity)}::{Normalize(action.Action)}";
            if (action.IsSystemAction && !catalogKeys.Contains(key))
                action.MarkDeprecated();
        }

        var existingRolePermissions = await db.RoleOperationPermissions
            .IgnoreQueryFilters()
            .ToDictionaryAsync(
                item => $"{item.Role}::{Normalize(item.Entity)}::{Normalize(item.Action)}",
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        foreach (var definition in definitions)
        {
            foreach (var role in OperationPermissionCatalog.AdminPanelRoles)
            {
                var key = $"{role}::{Normalize(definition.Entity)}::{Normalize(definition.Action)}";
                if (existingRolePermissions.ContainsKey(key))
                    continue;

                db.RoleOperationPermissions.Add(new RoleOperationPermission(
                    role,
                    definition.Entity,
                    definition.Action,
                    definition.DefaultAllowedFor(role)));
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static string Normalize(string value) => (value ?? string.Empty).Trim();
}
