namespace Randevoo.AdminPanel.Models.Events;

public sealed class TagAdminItem
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int EventUsageCount { get; set; }
}
