namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventSmsRequest
{
    public long Id { get; set; }

    public string Message { get; set; } = string.Empty;

    public string? ApprovedMessage { get; set; }

    public string RequestedByName { get; set; } = string.Empty;

    public DateTimeOffset RequestedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? PlannedSendAtUtc { get; set; }

    public EventSmsRequestStatus Status { get; set; } = EventSmsRequestStatus.Pending;

    public string? ReviewNote { get; set; }

    public string? ReviewedByName { get; set; }

    public DateTimeOffset? ReviewedAtUtc { get; set; }

    public int QueuedRecipientsCount { get; set; }

    public string EffectiveMessage => string.IsNullOrWhiteSpace(ApprovedMessage) ? Message : ApprovedMessage!;

    public bool HasAdminEdits => !string.IsNullOrWhiteSpace(ApprovedMessage) && !string.Equals(Message, ApprovedMessage, StringComparison.Ordinal);
}
