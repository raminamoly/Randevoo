using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class PermissionAction : BaseEntity, IAggregateRoot
{
    public string Entity { get; private set; } = null!;
    public string EntityLabel { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public string Label { get; private set; } = null!;
    public string? Description { get; private set; }
    public string GroupKey { get; private set; } = null!;
    public string GroupLabel { get; private set; } = null!;
    public string? PagePath { get; private set; }
    public string? HandlerName { get; private set; }
    public string UiSurface { get; private set; } = null!;
    public string RiskLevel { get; private set; } = null!;
    public bool IsSystemAction { get; private set; }
    public bool IsDeprecated { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private PermissionAction() { }

    public PermissionAction(
        string entity,
        string action,
        string label,
        string? description = null,
        int displayOrder = 0,
        string? entityLabel = null,
        string? groupKey = null,
        string? groupLabel = null,
        string? pagePath = null,
        string? handlerName = null,
        string uiSurface = "Manual",
        string riskLevel = "Low",
        bool isSystemAction = true)
    {
        Entity = Normalize(entity);
        EntityLabel = CleanLabel(entityLabel, nameof(entityLabel), Entity);
        Action = Normalize(action);
        Label = GuardAgainst.String.InvalidLength(label, nameof(label), 2, 120);
        Description = CleanOptional(description, nameof(description), 2, 500);
        GroupKey = Normalize(groupKey ?? Entity);
        GroupLabel = CleanLabel(groupLabel, nameof(groupLabel), EntityLabel);
        PagePath = CleanOptional(pagePath, nameof(pagePath), 2, 160);
        HandlerName = CleanOptional(handlerName, nameof(handlerName), 2, 120);
        UiSurface = CleanLabel(uiSurface, nameof(uiSurface), "Manual", 2, 40);
        RiskLevel = CleanLabel(riskLevel, nameof(riskLevel), "Low", 2, 20);
        IsSystemAction = isSystemAction;
        IsDeprecated = false;
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<PermissionAction>(this));
    }

    public void Update(
        string label,
        string? description,
        bool isActive,
        int displayOrder,
        string? entityLabel = null,
        string? groupKey = null,
        string? groupLabel = null,
        string? pagePath = null,
        string? handlerName = null,
        string? uiSurface = null,
        string? riskLevel = null,
        bool? isSystemAction = null,
        bool? isDeprecated = null)
    {
        Label = GuardAgainst.String.InvalidLength(label, nameof(label), 2, 120);
        Description = CleanOptional(description, nameof(description), 2, 500);
        EntityLabel = CleanLabel(entityLabel, nameof(entityLabel), EntityLabel);
        GroupKey = Normalize(groupKey ?? GroupKey);
        GroupLabel = CleanLabel(groupLabel, nameof(groupLabel), GroupLabel);
        PagePath = CleanOptional(pagePath, nameof(pagePath), 2, 160);
        HandlerName = CleanOptional(handlerName, nameof(handlerName), 2, 120);
        UiSurface = CleanLabel(uiSurface, nameof(uiSurface), UiSurface, 2, 40);
        RiskLevel = CleanLabel(riskLevel, nameof(riskLevel), RiskLevel, 2, 20);
        IsSystemAction = isSystemAction ?? IsSystemAction;
        IsDeprecated = isDeprecated ?? IsDeprecated;
        IsActive = isActive;
        DisplayOrder = displayOrder;
        UpdateTimestamp();
    }

    public void ApplyCatalogMetadata(
        string label,
        string? description,
        int displayOrder,
        string entityLabel,
        string groupKey,
        string groupLabel,
        string? pagePath,
        string? handlerName,
        string uiSurface,
        string riskLevel,
        bool isSystemAction)
    {
        Update(
            label,
            description,
            isActive: true,
            displayOrder,
            entityLabel,
            groupKey,
            groupLabel,
            pagePath,
            handlerName,
            uiSurface,
            riskLevel,
            isSystemAction,
            isDeprecated: false);
    }

    public void MarkDeprecated()
    {
        IsDeprecated = true;
        IsActive = false;
        UpdateTimestamp();
    }

    public static string Normalize(string value)
    {
        return GuardAgainst.String.InvalidLength((value ?? string.Empty).Trim(), nameof(value), 2, 80);
    }

    private static string CleanLabel(string? value, string name, string fallback, int min = 2, int max = 120)
    {
        var text = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        return GuardAgainst.String.InvalidLength(text, name, min, max);
    }

    private static string? CleanOptional(string? value, string name, int min, int max)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : GuardAgainst.String.InvalidLength(value.Trim(), name, min, max);
    }
}
