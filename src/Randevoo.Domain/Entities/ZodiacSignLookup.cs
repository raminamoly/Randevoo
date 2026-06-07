using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class ZodiacSignLookup : BaseEntity, IAggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private ZodiacSignLookup() { }

    public ZodiacSignLookup(string code, string title, int displayOrder)
    {
        Code = GuardAgainst.String.InvalidLength(code, nameof(code), 2, 30);
        Title = GuardAgainst.String.InvalidLength(title, nameof(title), 2, 80);
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<ZodiacSignLookup>(this));
    }
}
