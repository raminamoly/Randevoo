using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class PlannerWithdrawalRequest : BaseEntity
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public decimal Amount { get; private set; }
    public PlannerWithdrawalRequestStatus Status { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public long? ReviewedByAdminUserId { get; private set; }
    public User? ReviewedByAdminUser { get; private set; }
    public string? ReviewNote { get; private set; }

    private PlannerWithdrawalRequest() { }

    public PlannerWithdrawalRequest(User plannerUser, decimal amount)
    {
        if (plannerUser.Role != UserRole.EventPlanner)
            throw new BusinessRuleViolationException("Invalid withdrawal user", "Only event planner users can request payouts");

        ValidateAmount(amount);
        User = GuardAgainst.Object.Null(plannerUser, nameof(plannerUser));
        UserId = plannerUser.Id;
        Amount = amount;
        Status = PlannerWithdrawalRequestStatus.Pending;
        RequestedAtUtc = DateTime.UtcNow;
    }

    public void Confirm(User adminUser, string? reviewNote)
    {
        EnsurePending();
        EnsureAdmin(adminUser);
        Status = PlannerWithdrawalRequestStatus.Confirmed;
        ReviewedByAdminUser = adminUser;
        ReviewedByAdminUserId = adminUser.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = NormalizeNote(reviewNote);
        UpdateTimestamp();
    }

    public void Reject(User adminUser, string? reviewNote)
    {
        EnsurePending();
        EnsureAdmin(adminUser);
        Status = PlannerWithdrawalRequestStatus.Rejected;
        ReviewedByAdminUser = adminUser;
        ReviewedByAdminUserId = adminUser.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = NormalizeNote(reviewNote);
        UpdateTimestamp();
    }

    private void EnsurePending()
    {
        if (Status != PlannerWithdrawalRequestStatus.Pending)
            throw new BusinessRuleViolationException("Withdrawal already reviewed", "Only pending withdrawal requests can be reviewed");
    }

    private static void EnsureAdmin(User adminUser)
    {
        if (adminUser.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Invalid reviewer", "Only admin users can review withdrawal requests");
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new BusinessRuleViolationException("Invalid withdrawal amount", "Withdrawal amount must be greater than zero");
    }

    private static string? NormalizeNote(string? reviewNote)
    {
        return string.IsNullOrWhiteSpace(reviewNote)
            ? null
            : GuardAgainst.String.MaxLength(reviewNote.Trim(), nameof(reviewNote), 1000);
    }
}
