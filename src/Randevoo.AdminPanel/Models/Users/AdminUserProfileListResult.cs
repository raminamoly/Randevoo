namespace Randevoo.AdminPanel.Models.Users;

public sealed class AdminUserProfileListResult
{
    public int TotalCount { get; set; }
    public IReadOnlyList<AdminUserProfileListItem> Items { get; set; } = Array.Empty<AdminUserProfileListItem>();
}
