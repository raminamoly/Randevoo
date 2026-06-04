using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class Tag : BaseEntity, IAggregateRoot
{
    private readonly List<EventTag> _eventTags = new();

    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public IReadOnlyList<EventTag> EventTags => _eventTags.AsReadOnly();

    private Tag() { }

    public Tag(string name)
    {
        Name = GuardAgainst.String.InvalidLength(name, nameof(name), 2, 50);
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<Tag>(this));
    }

    public void Update(string name, bool isActive)
    {
        Name = GuardAgainst.String.InvalidLength(name, nameof(name), 2, 50);
        IsActive = isActive;
        UpdateTimestamp();
    }
}
