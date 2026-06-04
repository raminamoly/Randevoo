namespace Randevoo.AdminPanel.Models.Users;

public sealed class UserProfileDetailsViewModel
{
    public long UserId { get; set; }
    public long ProfileId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? MobileNumber { get; set; }
    public string GenderTitle { get; set; } = string.Empty;
    public int Age { get; set; }
    public string BirthMonth { get; set; } = string.Empty;
    public string ZodiacSign { get; set; } = string.Empty;
    public string EducationLevelTitle { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? Region { get; set; }
    public int HeightCentimeters { get; set; }
    public bool Smoking { get; set; }
    public IReadOnlyList<string> Interests { get; set; } = Array.Empty<string>();
    public IReadOnlyList<UserProfileImageItem> Images { get; set; } = Array.Empty<UserProfileImageItem>();
    public IReadOnlyList<UserProfileTicketItem> Tickets { get; set; } = Array.Empty<UserProfileTicketItem>();
}
