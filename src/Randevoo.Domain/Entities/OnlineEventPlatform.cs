using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class OnlineEventPlatform : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private OnlineEventPlatform() { }

    public OnlineEventPlatform(string name, int displayOrder = 0)
    {
        Name = GuardAgainst.String.InvalidLength(name, nameof(name), 2, 80);
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<OnlineEventPlatform>(this));
    }
}
