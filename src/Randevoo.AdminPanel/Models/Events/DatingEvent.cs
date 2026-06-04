namespace Randevoo.AdminPanel.Models.Events;

public sealed class DatingEvent
{
    public long Id { get; set; }

    public long PlannerUserId { get; set; }

    public string PlannerName { get; set; } = string.Empty;

    public EventDraftInput Live { get; set; } = new();

    public EventDraftState? Pending { get; set; }

    public List<EventSmsRequest> SmsRequests { get; set; } = new();

    public List<EventChangeLogEntry> ChangeLog { get; set; } = new();

    public EventApprovalState Status { get; set; } = EventApprovalState.Draft;

    public string? AdminReviewNote { get; set; }

    public string? ReviewedByName { get; set; }

    public DateTimeOffset? ReviewedAtUtc { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public bool IsVisibleToEndUsers { get; set; }

    public string DisplayTitle => Pending?.Draft.Title is { Length: > 0 } pendingTitle ? pendingTitle : Live.Title;

    public EventDraftInput ActiveDraft => Pending?.Draft ?? Live;
}
