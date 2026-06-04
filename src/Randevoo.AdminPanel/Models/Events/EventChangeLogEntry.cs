namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventChangeLogEntry
{
    public long Id { get; set; }

    public string Category { get; set; } = "general";

    public string Action { get; set; } = string.Empty;

    public string ActorName { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string? Details { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
}
