namespace Randevoo.AdminPanel.Models.Users;

public sealed class AdminUserProfileListItem
{
    public long UserId { get; set; }
    public long ProfileId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public string RoleTitle { get; set; } = string.Empty;
    public string GenderTitle { get; set; } = string.Empty;
    public string CityTitle { get; set; } = string.Empty;
    public int Age { get; set; }
    public string ZodiacSignTitle { get; set; } = string.Empty;
    public string EducationLevelTitle { get; set; } = string.Empty;
    public bool IsProfileComplete { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastActivityAtUtc { get; set; }
}
