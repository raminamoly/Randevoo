using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventLike : BaseEntity, IAggregateRoot
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long FromUserId { get; private set; }
    public User FromUser { get; private set; } = null!;
    public long ToUserId { get; private set; }
    public User ToUser { get; private set; } = null!;
    public EventLikeStatus Status { get; private set; }
    public DateTime? RespondedAtUtc { get; private set; }

    private EventLike() { }

    public EventLike(DatingEvent datingEvent, User fromUser, User toUser)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        FromUser = GuardAgainst.Object.Null(fromUser, nameof(fromUser));
        ToUser = GuardAgainst.Object.Null(toUser, nameof(toUser));

        if (fromUser.Id == toUser.Id)
            throw new BusinessRuleViolationException("Invalid event like", "User cannot like themselves");

        DatingEventId = datingEvent.Id;
        FromUserId = fromUser.Id;
        ToUserId = toUser.Id;
        Status = EventLikeStatus.Pending;
        AddDomainEvent(new EntityCreatedEvent<EventLike>(this));
    }

    public void MarkMatched()
    {
        if (Status == EventLikeStatus.Rejected)
            throw new BusinessRuleViolationException("Like rejected", "Rejected likes cannot be matched");

        Status = EventLikeStatus.Matched;
        RespondedAtUtc = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Reject(long rejectedByUserId)
    {
        if (rejectedByUserId != ToUserId)
            throw new BusinessRuleViolationException("Invalid rejection", "Only the liked user can reject this like");

        if (Status == EventLikeStatus.Matched)
            throw new BusinessRuleViolationException("Like already matched", "Matched likes cannot be rejected");

        Status = EventLikeStatus.Rejected;
        RespondedAtUtc = DateTime.UtcNow;
        UpdateTimestamp();
    }
}
