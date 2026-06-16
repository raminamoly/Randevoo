using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class InterestTagMapping : BaseEntity, IAggregateRoot
{
    public long InterestId { get; private set; }
    public Interest Interest { get; private set; } = null!;
    public long TagId { get; private set; }
    public Tag Tag { get; private set; } = null!;
    public int RelevanceWeight { get; private set; }
    public bool IsActive { get; private set; }

    private InterestTagMapping() { }

    public InterestTagMapping(Interest interest, Tag tag, int relevanceWeight = 100, bool isActive = true)
    {
        Interest = GuardAgainst.Object.Null(interest, nameof(interest));
        Tag = GuardAgainst.Object.Null(tag, nameof(tag));
        InterestId = interest.Id;
        TagId = tag.Id;
        RelevanceWeight = GuardAgainst.Number.OutOfRange(relevanceWeight, nameof(relevanceWeight), 1, 100);
        IsActive = isActive;

        AddDomainEvent(new EntityCreatedEvent<InterestTagMapping>(this));
    }

    public void UpdateWeight(int relevanceWeight)
    {
        RelevanceWeight = GuardAgainst.Number.OutOfRange(relevanceWeight, nameof(relevanceWeight), 1, 100);
        UpdateTimestamp();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdateTimestamp();
    }
}
