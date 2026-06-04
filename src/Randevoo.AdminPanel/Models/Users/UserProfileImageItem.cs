namespace Randevoo.AdminPanel.Models.Users;

public sealed class UserProfileImageItem
{
    public string ImageUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}
