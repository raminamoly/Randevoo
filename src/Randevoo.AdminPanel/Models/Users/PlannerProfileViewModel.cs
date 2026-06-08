namespace Randevoo.AdminPanel.Models.Users;

public sealed class PlannerProfileViewModel
{
    public long UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string? PictureUrl { get; set; }

    public string Resume { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string SettlementCurrencyCode { get; set; } = "IRR";

    public bool IsSettlementCurrencyLocked { get; set; }

    public DateTime? SettlementCurrencyLockedAtUtc { get; set; }

    public string? SettlementCurrencyLockReason { get; set; }

    public bool HasPendingChanges { get; set; }

    public string? PendingFullName { get; set; }

    public string? PendingCity { get; set; }

    public string? PendingTitle { get; set; }

    public string? PendingPictureUrl { get; set; }

    public string? PendingResume { get; set; }

    public DateTimeOffset? PendingSubmittedAtUtc { get; set; }

    public string? PendingReviewNote { get; set; }

    public DateTimeOffset? PendingReviewedAtUtc { get; set; }

    public decimal AverageRating { get; set; }

    public int TotalSurveyCount { get; set; }

    public int HostedEventCount { get; set; }

    public int CancelledEventCount { get; set; }

    public int CompletedEventCount { get; set; }
}
