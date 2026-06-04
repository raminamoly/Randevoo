namespace Randevoo.AdminPanel.Models.Users;

public sealed class AdminUserProfileEditor
{
    public long UserId { get; set; }
    public long ProfileId { get; set; }
    public string MobileNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public AdminUserProfileEditorInput Input { get; set; } = new();
    public IReadOnlyList<UserProfileImageItem> Images { get; set; } = Array.Empty<UserProfileImageItem>();
    public IReadOnlyList<string> Interests { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> AvailableInterests { get; set; } = Array.Empty<string>();
}
