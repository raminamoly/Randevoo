namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventModeOption
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsOnline { get; set; }
}
