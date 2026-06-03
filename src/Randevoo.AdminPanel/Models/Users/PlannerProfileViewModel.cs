namespace Randevoo.AdminPanel.Models.Users;

public sealed class PlannerProfileViewModel
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? PictureUrl { get; set; }

    public string Resume { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public decimal AverageRating { get; set; }

    public int TotalSurveyCount { get; set; }

    public int HostedEventCount { get; set; }

    public int CancelledEventCount { get; set; }

    public int CompletedEventCount { get; set; }
}
