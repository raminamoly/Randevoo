using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Entities;

public class NotificationPriorityLookup : BaseEntity
{
    public NotificationPriority Priority { get; private set; }
    public string Code { get; private set; } = null!;
    public string DisplayNameFa { get; private set; } = null!;
    public string DescriptionFa { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private NotificationPriorityLookup() { }
}
