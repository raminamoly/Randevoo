using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventSettlementRequest : BaseEntity
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public EventSettlementRequestStatus Status { get; private set; }
    public long RequestedByUserId { get; private set; }
    public User RequestedByUser { get; private set; } = null!;
    public DateTime RequestedAtUtc { get; private set; }
    public long? ReviewedByUserId { get; private set; }
    public User? ReviewedByUser { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public int ValidTicketCount { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal PlatformCommissionAmount { get; private set; }
    public decimal OrganizerIncomeAmount { get; private set; }
    public decimal ReportingOrganizerIncomeIrr { get; private set; }
    public long? OrganizerCreditTransactionId { get; private set; }
    public BalanceTransaction? OrganizerCreditTransaction { get; private set; }
    public string? RequestNote { get; private set; }
    public string? ReviewNote { get; private set; }

    private EventSettlementRequest() { }

    public EventSettlementRequest(
        DatingEvent datingEvent,
        User requester,
        int validTicketCount,
        decimal grossAmount,
        decimal platformCommissionAmount,
        decimal organizerIncomeAmount,
        decimal reportingOrganizerIncomeIrr,
        string? requestNote)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        DatingEventId = datingEvent.Id;
        RequestedByUser = GuardAgainst.Object.Null(requester, nameof(requester));
        RequestedByUserId = requester.Id;
        ValidTicketCount = validTicketCount;
        GrossAmount = GuardAgainst.Number.OutOfRange(grossAmount, nameof(grossAmount), 0m, 100_000_000_000m);
        PlatformCommissionAmount = GuardAgainst.Number.OutOfRange(platformCommissionAmount, nameof(platformCommissionAmount), 0m, 100_000_000_000m);
        OrganizerIncomeAmount = GuardAgainst.Number.OutOfRange(organizerIncomeAmount, nameof(organizerIncomeAmount), 0m, 100_000_000_000m);
        ReportingOrganizerIncomeIrr = GuardAgainst.Number.OutOfRange(reportingOrganizerIncomeIrr, nameof(reportingOrganizerIncomeIrr), 0m, 100_000_000_000_000m);
        RequestNote = Normalize(requestNote);
        Status = EventSettlementRequestStatus.Pending;
        RequestedAtUtc = DateTime.UtcNow;
    }

    public void Approve(User reviewer, BalanceTransaction organizerCreditTransaction, string? note)
    {
        EnsurePending();
        ReviewedByUser = reviewer;
        ReviewedByUserId = reviewer.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = Normalize(note);
        OrganizerCreditTransaction = organizerCreditTransaction;
        OrganizerCreditTransactionId = organizerCreditTransaction.Id > 0 ? organizerCreditTransaction.Id : null;
        Status = EventSettlementRequestStatus.Approved;
        UpdateTimestamp();
    }

    public void Reject(User reviewer, string? note)
    {
        EnsurePending();
        ReviewedByUser = reviewer;
        ReviewedByUserId = reviewer.Id;
        ReviewedAtUtc = DateTime.UtcNow;
        ReviewNote = Normalize(note);
        Status = EventSettlementRequestStatus.Rejected;
        UpdateTimestamp();
    }

    private void EnsurePending()
    {
        if (Status != EventSettlementRequestStatus.Pending)
            throw new BusinessRuleViolationException("Settlement request reviewed", "Only pending settlement requests can be reviewed.");
    }

    private static string? Normalize(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), nameof(value), 1000);
}
