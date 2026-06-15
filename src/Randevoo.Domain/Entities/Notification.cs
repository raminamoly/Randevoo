using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class Notification : BaseEntity, IAggregateRoot
{
    private readonly List<NotificationRecipient> _recipients = new();

    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public NotificationApprovalStatus ApprovalStatus { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public long? DatingEventId { get; private set; }
    public DatingEvent? DatingEvent { get; private set; }
    public string? ReferenceType { get; private set; }
    public long? ReferenceId { get; private set; }
    public long CreatedByUserId { get; private set; }
    public User CreatedByUser { get; private set; } = null!;
    public long? ReviewedByUserId { get; private set; }
    public User? ReviewedByUser { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public string? ReviewNote { get; private set; }
    public IReadOnlyList<NotificationRecipient> Recipients => _recipients.AsReadOnly();

    private Notification() { }

    public Notification(
        User createdByUser,
        NotificationType type,
        string title,
        string body,
        NotificationPriority priority = NotificationPriority.Normal,
        bool requiresApproval = false,
        DatingEvent? datingEvent = null,
        string? referenceType = null,
        long? referenceId = null)
    {
        CreatedByUser = GuardAgainst.Object.Null(createdByUser, nameof(createdByUser));
        CreatedByUserId = createdByUser.Id;
        Type = GuardAgainst.Number.AgainstInvalidEnum<NotificationType>((int)type, nameof(type));
        Priority = GuardAgainst.Number.AgainstInvalidEnum<NotificationPriority>((int)priority, nameof(priority));
        ApprovalStatus = requiresApproval ? NotificationApprovalStatus.Pending : NotificationApprovalStatus.NotRequired;
        Title = GuardAgainst.String.InvalidLength(title.Trim(), nameof(title), 2, 180);
        Body = GuardAgainst.String.InvalidLength(body.Trim(), nameof(body), 2, 2000);
        DatingEvent = datingEvent;
        DatingEventId = datingEvent?.Id;
        ReferenceType = NormalizeOptional(referenceType, 100);
        ReferenceId = referenceId;

        AddDomainEvent(new EntityCreatedEvent<Notification>(this));
    }

    public void AddRecipient(User recipient, NotificationDeliveryChannel channel)
    {
        var normalizedRecipient = GuardAgainst.Object.Null(recipient, nameof(recipient));
        var normalizedChannel = GuardAgainst.Number.AgainstInvalidEnum<NotificationDeliveryChannel>((int)channel, nameof(channel));
        if (_recipients.Any(item => item.RecipientUserId == normalizedRecipient.Id && item.Channel == normalizedChannel))
            return;

        _recipients.Add(new NotificationRecipient(this, normalizedRecipient, normalizedChannel));
        UpdateTimestamp();
    }

    public void Approve(User reviewer, string? reviewNote = null)
    {
        if (ApprovalStatus != NotificationApprovalStatus.Pending)
            throw new BusinessRuleViolationException("Notification not pending", "Only pending notifications can be approved.");

        ReviewedByUser = GuardAgainst.Object.Null(reviewer, nameof(reviewer));
        ReviewedByUserId = reviewer.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = NormalizeOptional(reviewNote, 1000);
        ApprovalStatus = NotificationApprovalStatus.Approved;
        foreach (var recipient in _recipients)
            recipient.MarkDelivered();
        UpdateTimestamp();
    }

    public void Reject(User reviewer, string reviewNote)
    {
        if (ApprovalStatus != NotificationApprovalStatus.Pending)
            throw new BusinessRuleViolationException("Notification not pending", "Only pending notifications can be rejected.");

        ReviewedByUser = GuardAgainst.Object.Null(reviewer, nameof(reviewer));
        ReviewedByUserId = reviewer.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = GuardAgainst.String.InvalidLength(reviewNote.Trim(), nameof(reviewNote), 3, 1000);
        ApprovalStatus = NotificationApprovalStatus.Rejected;
        foreach (var recipient in _recipients)
            recipient.MarkRejected();
        UpdateTimestamp();
    }

    private static string? NormalizeOptional(string? value, int maxLength)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), nameof(value), maxLength);
}
