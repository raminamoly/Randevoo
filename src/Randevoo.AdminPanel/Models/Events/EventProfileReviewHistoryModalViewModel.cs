namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventProfileReviewHistoryModalViewModel
{
    public long EventId { get; init; }

    public string EventTitle { get; init; } = string.Empty;

    public IReadOnlyList<EventChangeLogEntry> Entries { get; init; } = Array.Empty<EventChangeLogEntry>();
}
