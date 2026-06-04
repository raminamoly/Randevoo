namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventTypeAdminItem
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public int EventUsageCount { get; set; }
}
