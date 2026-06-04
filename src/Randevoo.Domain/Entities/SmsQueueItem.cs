using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class SmsQueueItem : BaseEntity, IAggregateRoot
{
    public long? EventParticipantSmsRequestId { get; private set; }
    public EventParticipantSmsRequest? EventParticipantSmsRequest { get; private set; }
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long RecipientUserId { get; private set; }
    public User RecipientUser { get; private set; } = null!;
    public string MobileNumber { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public DateTime? PlannedSendAtUtc { get; private set; }
    public SmsQueueItemStatus Status { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime? SentAt { get; private set; }
    public string? FailureReason { get; private set; }

    private SmsQueueItem() { }

    public SmsQueueItem(User recipientUser, DatingEvent datingEvent, string message, DateTime? plannedSendAtUtc = null, long? eventParticipantSmsRequestId = null)
    {
        RecipientUser = GuardAgainst.Object.Null(recipientUser, nameof(recipientUser));
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        RecipientUserId = recipientUser.Id;
        DatingEventId = datingEvent.Id;
        EventParticipantSmsRequestId = eventParticipantSmsRequestId;
        MobileNumber = GuardAgainst.String.InvalidLength(recipientUser.MobileNumber.Trim(), nameof(recipientUser.MobileNumber), 5, 20);
        Message = GuardAgainst.String.InvalidLength(message.Trim(), nameof(message), 5, 480);
        PlannedSendAtUtc = plannedSendAtUtc?.ToUniversalTime();
        Status = SmsQueueItemStatus.Pending;
        AddDomainEvent(new EntityCreatedEvent<SmsQueueItem>(this));
    }

    public void MarkSent()
    {
        Status = SmsQueueItemStatus.Sent;
        SentAt = DateTime.UtcNow;
        FailureReason = null;
        UpdateTimestamp();
    }

    public void MarkFailed(string reason)
    {
        Status = SmsQueueItemStatus.Failed;
        AttemptCount++;
        FailureReason = GuardAgainst.String.InvalidLength(reason.Trim(), nameof(reason), 3, 1000);
        UpdateTimestamp();
    }
}
