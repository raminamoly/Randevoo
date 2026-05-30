using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class EventType : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private EventType() { }

    public EventType(string name, string? description = null)
    {
        Name = GuardAgainst.String.InvalidLength(name, nameof(name), 2, 100);
        Description = string.IsNullOrWhiteSpace(description) ? null : GuardAgainst.String.MaxLength(description, nameof(description), 500);
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<EventType>(this));
    }

    public void Update(string name, string? description, bool isActive)
    {
        Name = GuardAgainst.String.InvalidLength(name, nameof(name), 2, 100);
        Description = string.IsNullOrWhiteSpace(description) ? null : GuardAgainst.String.MaxLength(description, nameof(description), 500);
        IsActive = isActive;
        UpdateTimestamp();
    }
}
