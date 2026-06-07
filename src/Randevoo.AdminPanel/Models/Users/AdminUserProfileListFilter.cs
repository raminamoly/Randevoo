namespace Randevoo.AdminPanel.Models.Users;

public sealed class AdminUserProfileListFilter
{
    public string? Search { get; set; }
    public long? CityId { get; set; }
    public long? GenderId { get; set; }
    public long? ZodiacSignId { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsProfileComplete { get; set; }
    public string Sort { get; set; } = "newest";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
}
