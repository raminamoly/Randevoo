using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class UserFacingEventStatus : BaseEntity, IAggregateRoot
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public UserFacingEventStatusKind Status { get; private set; }
    public DateTime? ParticipantProfilesOpenAtUtc { get; private set; }
    public DateTime? LikeWindowOpenAtUtc { get; private set; }
    public DateTime? LikeWindowCloseAtUtc { get; private set; }
    public DateTime LastEvaluatedAtUtc { get; private set; }

    private UserFacingEventStatus() { }

    public UserFacingEventStatus(DatingEvent datingEvent, UserFacingEventStatusKind status, DateTime evaluatedAtUtc)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        DatingEventId = datingEvent.Id;
        Status = GuardAgainst.Number.AgainstInvalidEnum<UserFacingEventStatusKind>((int)status, nameof(status));
        LastEvaluatedAtUtc = NormalizeUtc(evaluatedAtUtc);

        AddDomainEvent(new EntityCreatedEvent<UserFacingEventStatus>(this));
    }

    public void Update(
        UserFacingEventStatusKind status,
        DateTime evaluatedAtUtc,
        DateTime? participantProfilesOpenAtUtc = null,
        DateTime? likeWindowOpenAtUtc = null,
        DateTime? likeWindowCloseAtUtc = null)
    {
        Status = GuardAgainst.Number.AgainstInvalidEnum<UserFacingEventStatusKind>((int)status, nameof(status));
        LastEvaluatedAtUtc = NormalizeUtc(evaluatedAtUtc);
        ParticipantProfilesOpenAtUtc = NormalizeNullableUtc(participantProfilesOpenAtUtc);
        LikeWindowOpenAtUtc = NormalizeNullableUtc(likeWindowOpenAtUtc);
        LikeWindowCloseAtUtc = NormalizeNullableUtc(likeWindowCloseAtUtc);
        UpdateTimestamp();
    }

    private static DateTime NormalizeUtc(DateTime value)
        => value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);

    private static DateTime? NormalizeNullableUtc(DateTime? value)
        => value.HasValue ? NormalizeUtc(value.Value) : null;
}
