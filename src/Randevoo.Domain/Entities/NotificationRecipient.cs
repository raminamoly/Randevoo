using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class NotificationRecipient : BaseEntity
{
    public long NotificationId { get; private set; }
    public Notification Notification { get; private set; } = null!;
    public long RecipientUserId { get; private set; }
    public User RecipientUser { get; private set; } = null!;
    public NotificationDeliveryChannel Channel { get; private set; }
    public NotificationRecipientStatus Status { get; private set; }
    public DateTime? DeliveredAtUtc { get; private set; }
    public DateTime? ReadAtUtc { get; private set; }
    public string? FailureReason { get; private set; }

    private NotificationRecipient() { }

    internal NotificationRecipient(Notification notification, User recipientUser, NotificationDeliveryChannel channel)
    {
        Notification = GuardAgainst.Object.Null(notification, nameof(notification));
        RecipientUser = GuardAgainst.Object.Null(recipientUser, nameof(recipientUser));
        RecipientUserId = recipientUser.Id;
        Channel = GuardAgainst.Number.AgainstInvalidEnum<NotificationDeliveryChannel>((int)channel, nameof(channel));
        Status = notification.ApprovalStatus == NotificationApprovalStatus.Pending
            ? NotificationRecipientStatus.Pending
            : NotificationRecipientStatus.Delivered;
        DeliveredAtUtc = Status == NotificationRecipientStatus.Delivered ? DateTime.UtcNow : null;
    }

    public void MarkDelivered()
    {
        if (Status is NotificationRecipientStatus.Rejected)
            return;

        Status = NotificationRecipientStatus.Delivered;
        DeliveredAtUtc ??= DateTime.UtcNow;
        FailureReason = null;
        UpdateTimestamp();
    }

    public void MarkRead()
    {
        if (Status is not (NotificationRecipientStatus.Delivered or NotificationRecipientStatus.Read))
            throw new BusinessRuleViolationException("Notification is not readable", "This notification cannot be marked as read.");

        Status = NotificationRecipientStatus.Read;
        DeliveredAtUtc ??= DateTime.UtcNow;
        ReadAtUtc ??= DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void MarkFailed(string reason)
    {
        Status = NotificationRecipientStatus.Failed;
        FailureReason = GuardAgainst.String.InvalidLength(reason.Trim(), nameof(reason), 3, 1000);
        UpdateTimestamp();
    }

    public void MarkRejected()
    {
        Status = NotificationRecipientStatus.Rejected;
        UpdateTimestamp();
    }
}
