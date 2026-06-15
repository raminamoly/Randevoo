using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class NotificationTemplate : BaseEntity, IAggregateRoot
{
    public string Code { get; private set; } = null!;
    public string TitleTemplate { get; private set; } = null!;
    public string BodyTemplate { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public bool RequiresApproval { get; private set; }
    public bool IsActive { get; private set; }

    private NotificationTemplate() { }

    public NotificationTemplate(string code, string titleTemplate, string bodyTemplate, NotificationType type, NotificationPriority priority = NotificationPriority.Normal, bool requiresApproval = false)
    {
        Code = GuardAgainst.String.InvalidLength(code.Trim(), nameof(code), 2, 80);
        TitleTemplate = GuardAgainst.String.InvalidLength(titleTemplate.Trim(), nameof(titleTemplate), 2, 180);
        BodyTemplate = GuardAgainst.String.InvalidLength(bodyTemplate.Trim(), nameof(bodyTemplate), 2, 2000);
        Type = GuardAgainst.Number.AgainstInvalidEnum<NotificationType>((int)type, nameof(type));
        Priority = GuardAgainst.Number.AgainstInvalidEnum<NotificationPriority>((int)priority, nameof(priority));
        RequiresApproval = requiresApproval;
        IsActive = true;

        AddDomainEvent(new EntityCreatedEvent<NotificationTemplate>(this));
    }

    public void Activate()
    {
        IsActive = true;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }
}
