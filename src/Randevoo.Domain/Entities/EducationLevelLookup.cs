using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class EducationLevelLookup : BaseEntity, IAggregateRoot
{
    public string Title { get; private set; } = null!;
    public int Rank { get; private set; }
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private EducationLevelLookup() { }

    public EducationLevelLookup(string title, int rank, int displayOrder)
    {
        Title = GuardAgainst.String.InvalidLength(title, nameof(title), 2, 150);
        Rank = rank;
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<EducationLevelLookup>(this));
    }
}
