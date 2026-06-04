namespace Randevoo.AdminPanel.Models.Users;

public sealed class PlannerProfileApprovalItem
{
    public long UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public bool HasPendingChanges { get; set; }

    public string? PendingTitle { get; set; }

    public DateTimeOffset? PendingSubmittedAtUtc { get; set; }

    public int HostedEventCount { get; set; }
}
