using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Constants;

public sealed record OperationPermissionDefinition(
    string Entity,
    string EntityLabel,
    string Action,
    string Label,
    string? Description,
    string GroupKey,
    string GroupLabel,
    string? PagePath,
    string? HandlerName,
    string UiSurface,
    string RiskLevel,
    int DisplayOrder,
    bool DefaultAdmin = true,
    bool DefaultPlanner = false,
    bool DefaultSupport = false,
    bool IsSystemAction = true)
{
    public bool DefaultAllowedFor(UserRole role) => role switch
    {
        UserRole.Admin => DefaultAdmin,
        UserRole.EventPlanner => DefaultPlanner,
        UserRole.PlatformSupportTeam => DefaultSupport,
        _ => false
    };
}
