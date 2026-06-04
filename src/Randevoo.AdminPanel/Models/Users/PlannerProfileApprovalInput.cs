namespace Randevoo.AdminPanel.Models.Users;

public sealed class PlannerProfileApprovalInput
{
    public string FullName { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? PictureUrl { get; set; }

    public string Resume { get; set; } = string.Empty;

    public string? ReviewNote { get; set; }
}
