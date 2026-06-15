using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventChangeRequest : BaseEntity
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public EventChangeRequestStatus Status { get; private set; }
    public long RequestedByUserId { get; private set; }
    public User RequestedByUser { get; private set; } = null!;
    public DateTime RequestedAtUtc { get; private set; }
    public long? ReviewedByUserId { get; private set; }
    public User? ReviewedByUser { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public string? Reason { get; private set; }
    public string BeforeJson { get; private set; } = null!;
    public string AfterJson { get; private set; } = null!;
    public string? ReviewNote { get; private set; }

    private EventChangeRequest() { }

    public EventChangeRequest(DatingEvent datingEvent, User requester, string beforeJson, string afterJson, string? reason)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        DatingEventId = datingEvent.Id;
        RequestedByUser = GuardAgainst.Object.Null(requester, nameof(requester));
        RequestedByUserId = requester.Id;
        BeforeJson = GuardAgainst.String.InvalidLength(beforeJson, nameof(beforeJson), 2, 8000);
        AfterJson = GuardAgainst.String.InvalidLength(afterJson, nameof(afterJson), 2, 8000);
        Reason = Normalize(reason);
        Status = EventChangeRequestStatus.Pending;
        RequestedAtUtc = DateTime.UtcNow;
    }

    public void Approve(User reviewer, string? note)
    {
        EnsurePending();
        ReviewedByUser = reviewer;
        ReviewedByUserId = reviewer.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = Normalize(note);
        Status = EventChangeRequestStatus.Approved;
        UpdateTimestamp();
    }

    public void Reject(User reviewer, string? note)
    {
        EnsurePending();
        ReviewedByUser = reviewer;
        ReviewedByUserId = reviewer.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = Normalize(note);
        Status = EventChangeRequestStatus.Rejected;
        UpdateTimestamp();
    }

    public void Cancel()
    {
        EnsurePending();
        Status = EventChangeRequestStatus.Cancelled;
        UpdateTimestamp();
    }

    private void EnsurePending()
    {
        if (Status != EventChangeRequestStatus.Pending)
            throw new BusinessRuleViolationException("Change request reviewed", "Only pending change requests can be reviewed.");
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), nameof(value), 1000);
}
