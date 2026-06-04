using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventParticipantSmsRequest : BaseEntity, IAggregateRoot
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long RequestedByUserId { get; private set; }
    public User RequestedByUser { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? ApprovedMessage { get; private set; }
    public DateTime? PlannedSendAtUtc { get; private set; }
    public EventParticipantSmsRequestStatus Status { get; private set; }
    public string? ReviewNote { get; private set; }
    public long? ReviewedByAdminUserId { get; private set; }
    public User? ReviewedByAdminUser { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public int QueuedRecipientsCount { get; private set; }

    private EventParticipantSmsRequest() { }

    public EventParticipantSmsRequest(User requestedByUser, DatingEvent datingEvent, string message, DateTime? plannedSendAtUtc = null)
    {
        RequestedByUser = GuardAgainst.Object.Null(requestedByUser, nameof(requestedByUser));
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        RequestedByUserId = requestedByUser.Id;
        DatingEventId = datingEvent.Id;
        Message = GuardAgainst.String.InvalidLength(message.Trim(), nameof(message), 5, 480);
        PlannedSendAtUtc = NormalizePlannedSendAtUtc(plannedSendAtUtc);
        Status = EventParticipantSmsRequestStatus.Pending;
        AddDomainEvent(new EntityCreatedEvent<EventParticipantSmsRequest>(this));
    }

    public void Approve(long adminUserId, int queuedRecipientsCount, string approvedMessage, DateTime? plannedSendAtUtc, string? note)
    {
        EnsurePending();

        ReviewedByAdminUserId = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        ApprovedMessage = GuardAgainst.String.InvalidLength(approvedMessage.Trim(), nameof(approvedMessage), 5, 480);
        PlannedSendAtUtc = NormalizePlannedSendAtUtc(plannedSendAtUtc);
        QueuedRecipientsCount = GuardAgainst.Number.OutOfRange(queuedRecipientsCount, nameof(queuedRecipientsCount), 0, 10000);
        ReviewNote = string.IsNullOrWhiteSpace(note) ? null : GuardAgainst.String.MaxLength(note.Trim(), nameof(note), 1000);
        Status = EventParticipantSmsRequestStatus.Approved;
        UpdateTimestamp();
    }

    public void Reject(long adminUserId, string note)
    {
        EnsurePending();

        ReviewedByAdminUserId = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        ReviewNote = GuardAgainst.String.InvalidLength(note.Trim(), nameof(note), 3, 1000);
        Status = EventParticipantSmsRequestStatus.Rejected;
        QueuedRecipientsCount = 0;
        UpdateTimestamp();
    }

    public string GetEffectiveMessage() => string.IsNullOrWhiteSpace(ApprovedMessage) ? Message : ApprovedMessage;

    private void EnsurePending()
    {
        if (Status != EventParticipantSmsRequestStatus.Pending)
        {
            throw new BusinessRuleViolationException(
                "Invalid SMS request state",
                "Only pending participant SMS requests can be reviewed");
        }
    }

    private static DateTime? NormalizePlannedSendAtUtc(DateTime? plannedSendAtUtc)
    {
        if (!plannedSendAtUtc.HasValue)
            return null;

        var normalized = plannedSendAtUtc.Value.Kind == DateTimeKind.Utc
            ? plannedSendAtUtc.Value
            : plannedSendAtUtc.Value.ToUniversalTime();

        if (normalized <= DateTime.UtcNow)
        {
            throw new BusinessRuleViolationException(
                "Invalid SMS planned send time",
                "Planned SMS send time must be in the future");
        }

        return normalized;
    }
}
