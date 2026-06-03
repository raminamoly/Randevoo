namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventDraftState
{
    public EventDraftInput Draft { get; set; } = new();

    public string SubmittedByName { get; set; } = string.Empty;

    public DateTimeOffset SubmittedAtUtc { get; set; }

    public string? ReviewNote { get; set; }
}

