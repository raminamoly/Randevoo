using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class PlannerWithdrawalRequestItem
{
    public long Id { get; set; }
    public long PlannerUserId { get; set; }
    public string PlannerName { get; set; } = string.Empty;
    public string PlannerMobile { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PlannerWithdrawalRequestStatus Status { get; set; }
    public DateTime RequestedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewNote { get; set; }
}
