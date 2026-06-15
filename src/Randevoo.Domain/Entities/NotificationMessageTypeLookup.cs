using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Entities;

public class NotificationMessageTypeLookup : BaseEntity
{
    public NotificationType Type { get; private set; }
    public string Code { get; private set; } = null!;
    public string DisplayNameFa { get; private set; } = null!;
    public string DescriptionFa { get; private set; } = null!;
    public bool RequiresApproval { get; private set; }
    public bool SupportsSms { get; private set; }
    public string AllowedSenderRoles { get; private set; } = null!;
    public string AllowedTargets { get; private set; } = null!;
    public NotificationPriority DefaultPriority { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private NotificationMessageTypeLookup() { }
}
