using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventCancellationRequest : BaseEntity
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public EventCancellationRequestStatus Status { get; private set; }
    public long RequestedByUserId { get; private set; }
    public User RequestedByUser { get; private set; } = null!;
    public DateTime RequestedAtUtc { get; private set; }
    public long? ReviewedByUserId { get; private set; }
    public User? ReviewedByUser { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? ReviewNote { get; private set; }
    public string? PublicMessage { get; private set; }
    public string? PreviewJson { get; private set; }
    public DateTime? ExecutedAtUtc { get; private set; }

    private EventCancellationRequest() { }

    public EventCancellationRequest(DatingEvent datingEvent, User requester, string reason)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        DatingEventId = datingEvent.Id;
        RequestedByUser = GuardAgainst.Object.Null(requester, nameof(requester));
        RequestedByUserId = requester.Id;
        Reason = GuardAgainst.String.InvalidLength(reason.Trim(), nameof(reason), 5, 1000);
        Status = EventCancellationRequestStatus.Pending;
        RequestedAtUtc = DateTime.UtcNow;
    }

    public void Approve(User reviewer, string? note)
    {
        Approve(reviewer, note, null, null);
    }

    public void Approve(User reviewer, string? note, string? publicMessage, string? previewJson)
    {
        EnsurePending();
        ReviewedByUser = reviewer;
        ReviewedByUserId = reviewer.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = Normalize(note);
        PublicMessage = Normalize(publicMessage);
        PreviewJson = NormalizePreview(previewJson);
        ExecutedAtUtc = DateTime.UtcNow;
        Status = EventCancellationRequestStatus.Approved;
        UpdateTimestamp();
    }

    public void Reject(User reviewer, string? note)
    {
        EnsurePending();
        ReviewedByUser = reviewer;
        ReviewedByUserId = reviewer.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = Normalize(note);
        Status = EventCancellationRequestStatus.Rejected;
        UpdateTimestamp();
    }

    public void Cancel()
    {
        EnsurePending();
        Status = EventCancellationRequestStatus.Cancelled;
        UpdateTimestamp();
    }

    private void EnsurePending()
    {
        if (Status != EventCancellationRequestStatus.Pending)
            throw new BusinessRuleViolationException("Cancellation request reviewed", "Only pending cancellation requests can be reviewed.");
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), nameof(value), 1000);

    private static string? NormalizePreview(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), nameof(value), 8000);
}
