using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class TicketRefundRequest : BaseEntity, IAggregateRoot
{
    public long EventTicketId { get; private set; }
    public EventTicket EventTicket { get; private set; } = null!;
    public long TicketOrderId { get; private set; }
    public TicketOrder TicketOrder { get; private set; } = null!;
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long BuyerUserId { get; private set; }
    public User BuyerUser { get; private set; } = null!;
    public long ParticipantUserId { get; private set; }
    public User ParticipantUser { get; private set; } = null!;
    public long RequestedByUserId { get; private set; }
    public User RequestedByUser { get; private set; } = null!;
    public TicketRefundRequestStatus Status { get; private set; }
    public decimal RequestedAmount { get; private set; }
    public decimal ApprovedAmount { get; private set; }
    public string CurrencyCode { get; private set; } = "IRR";
    public decimal ReportingRequestedAmountIrr { get; private set; }
    public decimal ReportingApprovedAmountIrr { get; private set; }
    public decimal ExchangeRateToIrr { get; private set; } = 1m;
    public DateTime ExchangeRateCapturedAtUtc { get; private set; }
    public long? ExchangeRateId { get; private set; }
    public CurrencyExchangeRate? ExchangeRate { get; private set; }
    public string RequestReason { get; private set; } = null!;
    public DateTime RequestedAtUtc { get; private set; }
    public long? ReviewedByUserId { get; private set; }
    public User? ReviewedByUser { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public string? ReviewNote { get; private set; }
    public long? WalletCreditTransactionId { get; private set; }
    public BalanceTransaction? WalletCreditTransaction { get; private set; }

    private TicketRefundRequest() { }

    public TicketRefundRequest(EventTicket eventTicket, User requestedByUser, decimal requestedAmount, string reason)
    {
        EventTicket = GuardAgainst.Object.Null(eventTicket, nameof(eventTicket));
        EventTicketId = eventTicket.Id;
        TicketOrder = eventTicket.TicketOrder;
        TicketOrderId = eventTicket.TicketOrderId;
        DatingEvent = eventTicket.DatingEvent;
        DatingEventId = eventTicket.DatingEventId;
        BuyerUser = eventTicket.TicketOrder.BuyerUser;
        BuyerUserId = eventTicket.TicketOrder.BuyerUserId;
        ParticipantUser = eventTicket.ParticipantUser;
        ParticipantUserId = eventTicket.ParticipantUserId;
        RequestedByUser = GuardAgainst.Object.Null(requestedByUser, nameof(requestedByUser));
        RequestedByUserId = requestedByUser.Id;
        Status = TicketRefundRequestStatus.Pending;
        RequestedAmount = GuardAgainst.Number.OutOfRange(requestedAmount, nameof(requestedAmount), 0.01m, eventTicket.Price);
        ApprovedAmount = 0m;
        CurrencyCode = CurrencyLookup.NormalizeCode(eventTicket.CurrencyCode);
        ExchangeRateToIrr = GuardAgainst.Number.OutOfRange(eventTicket.ExchangeRateToIrr, nameof(eventTicket.ExchangeRateToIrr), 0.000001m, 1_000_000_000_000m);
        ExchangeRateCapturedAtUtc = eventTicket.ExchangeRateCapturedAtUtc;
        ExchangeRateId = eventTicket.ExchangeRateId;
        ReportingRequestedAmountIrr = ConvertToIrr(RequestedAmount, ExchangeRateToIrr);
        ReportingApprovedAmountIrr = 0m;
        RequestReason = GuardAgainst.String.InvalidLength(reason.Trim(), nameof(reason), 3, 1000);
        RequestedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new EntityCreatedEvent<TicketRefundRequest>(this));
    }

    public void Approve(User reviewer, decimal approvedAmount, BalanceTransaction walletCreditTransaction, string? reviewNote)
    {
        EnsurePending();
        ReviewedByUser = GuardAgainst.Object.Null(reviewer, nameof(reviewer));
        ReviewedByUserId = reviewer.Id;
        ApprovedAmount = GuardAgainst.Number.OutOfRange(approvedAmount, nameof(approvedAmount), 0.01m, RequestedAmount);
        ReportingApprovedAmountIrr = ConvertToIrr(ApprovedAmount, ExchangeRateToIrr);
        WalletCreditTransaction = GuardAgainst.Object.Null(walletCreditTransaction, nameof(walletCreditTransaction));
        WalletCreditTransactionId = walletCreditTransaction.Id > 0 ? walletCreditTransaction.Id : null;
        ReviewNote = NormalizeOptional(reviewNote, 1000);
        ReviewedAtUtc = DateTime.UtcNow;
        Status = TicketRefundRequestStatus.Approved;
        EventTicket.MarkRefunded();
        UpdateTimestamp();
    }

    public void Reject(User reviewer, string reviewNote)
    {
        EnsurePending();
        ReviewedByUser = GuardAgainst.Object.Null(reviewer, nameof(reviewer));
        ReviewedByUserId = reviewer.Id;
        ReviewNote = GuardAgainst.String.InvalidLength(reviewNote.Trim(), nameof(reviewNote), 3, 1000);
        ReviewedAtUtc = DateTime.UtcNow;
        Status = TicketRefundRequestStatus.Rejected;
        UpdateTimestamp();
    }

    public void Cancel(User actor, string? note)
    {
        EnsurePending();
        ReviewedByUser = GuardAgainst.Object.Null(actor, nameof(actor));
        ReviewedByUserId = actor.Id;
        ReviewNote = NormalizeOptional(note, 1000);
        ReviewedAtUtc = DateTime.UtcNow;
        Status = TicketRefundRequestStatus.Cancelled;
        UpdateTimestamp();
    }

    private void EnsurePending()
    {
        if (Status != TicketRefundRequestStatus.Pending)
            throw new BusinessRuleViolationException("Refund already reviewed", "This refund request has already been reviewed.");
    }

    private static string? NormalizeOptional(string? value, int maxLength)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), nameof(value), maxLength);

    private static decimal ConvertToIrr(decimal amount, decimal rate)
        => Math.Round(amount * rate, 0, MidpointRounding.AwayFromZero);
}
