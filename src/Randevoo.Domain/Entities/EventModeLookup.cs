using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class EventModeLookup : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public bool IsOnline { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private EventModeLookup() { }

    public EventModeLookup(string name, bool isOnline, int displayOrder = 0)
    {
        Name = GuardAgainst.String.InvalidLength(name, nameof(name), 2, 80);
        IsOnline = isOnline;
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<EventModeLookup>(this));
    }
}
