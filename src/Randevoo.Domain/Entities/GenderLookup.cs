using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class GenderLookup : BaseEntity, IAggregateRoot
{
    public string Title { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private GenderLookup() { }

    public GenderLookup(string title, int displayOrder)
    {
        Title = GuardAgainst.String.InvalidLength(title, nameof(title), 2, 50);
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<GenderLookup>(this));
    }
}
