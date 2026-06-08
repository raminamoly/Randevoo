namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventListResult
{
    public int TotalCount { get; set; }
    public IReadOnlyList<DatingEvent> Items { get; set; } = Array.Empty<DatingEvent>();
}
