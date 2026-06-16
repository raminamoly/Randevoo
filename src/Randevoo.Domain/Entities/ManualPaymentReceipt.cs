using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class ManualPaymentReceipt : BaseEntity, IAggregateRoot
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long ParticipantUserId { get; private set; }
    public User ParticipantUser { get; private set; } = null!;
    public long PlannerUserId { get; private set; }
    public User PlannerUser { get; private set; } = null!;
    public long? EventTicketId { get; private set; }
    public EventTicket? EventTicket { get; private set; }
    public long? TicketOrderId { get; private set; }
    public TicketOrder? TicketOrder { get; private set; }
    public long? WalletCreditTransactionId { get; private set; }
    public BalanceTransaction? WalletCreditTransaction { get; private set; }
    public long? EventDiscountCodeId { get; private set; }
    public EventDiscountCode? EventDiscountCode { get; private set; }
    public string? DiscountCode { get; private set; }
    public EventPaymentCollectionMethod PaymentCollectionMethod { get; private set; }
    public ManualPaymentDestinationType DestinationType { get; private set; }
    public decimal OriginalAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = "IRR";
    public string ReportingCurrencyCode { get; private set; } = "IRR";
    public decimal ReportingAmountIrr { get; private set; }
    public decimal ExchangeRateToIrr { get; private set; } = 1m;
    public DateTime ExchangeRateCapturedAtUtc { get; private set; }
    public long? ExchangeRateId { get; private set; }
    public CurrencyExchangeRate? ExchangeRate { get; private set; }
    public string UploadedFilePath { get; private set; } = null!;
    public string? TrackingNumber { get; private set; }
    public string? PayerNote { get; private set; }
    public ManualPaymentReceiptStatus Status { get; private set; }
    public DateTime SubmittedAtUtc { get; private set; }
    public long? ReviewedByUserId { get; private set; }
    public User? ReviewedByUser { get; private set; }
    public DateTime? ReviewedAtUtc { get; private set; }
    public string? RejectReason { get; private set; }

    private ManualPaymentReceipt() { }

    public ManualPaymentReceipt(
        DatingEvent datingEvent,
        User participantUser,
        decimal originalAmount,
        decimal amount,
        string currencyCode,
        EventPaymentCollectionMethod paymentCollectionMethod,
        string uploadedFilePath,
        string? trackingNumber,
        string? payerNote,
        decimal exchangeRateToIrr,
        DateTime exchangeRateCapturedAtUtc,
        long? exchangeRateId = null,
        EventDiscountCode? eventDiscountCode = null)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        DatingEventId = datingEvent.Id;
        ParticipantUser = GuardAgainst.Object.Null(participantUser, nameof(participantUser));
        ParticipantUserId = participantUser.Id;
        PlannerUser = datingEvent.EventPlannerUser;
        PlannerUserId = datingEvent.EventPlannerUserId;
        PaymentCollectionMethod = GuardAgainst.Number.AgainstInvalidEnum<EventPaymentCollectionMethod>((int)paymentCollectionMethod, nameof(paymentCollectionMethod));
        if (PaymentCollectionMethod == EventPaymentCollectionMethod.PlatformGateway)
            throw new BusinessRuleViolationException("Invalid manual receipt method", "Gateway payments do not use manual receipts.");

        DestinationType = PaymentCollectionMethod == EventPaymentCollectionMethod.OrganizerManualTransfer
            ? ManualPaymentDestinationType.Organizer
            : ManualPaymentDestinationType.Platform;
        OriginalAmount = GuardAgainst.Number.OutOfRange(originalAmount, nameof(originalAmount), 0.01m, 1_000_000_000m);
        Amount = GuardAgainst.Number.OutOfRange(amount, nameof(amount), 0.01m, OriginalAmount);
        DiscountAmount = OriginalAmount - Amount;
        CurrencyCode = CurrencyLookup.NormalizeCode(currencyCode);
        ReportingCurrencyCode = "IRR";
        ExchangeRateToIrr = GuardAgainst.Number.OutOfRange(exchangeRateToIrr, nameof(exchangeRateToIrr), 0.000001m, 1_000_000_000_000m);
        ExchangeRateCapturedAtUtc = exchangeRateCapturedAtUtc.Kind == DateTimeKind.Utc
            ? exchangeRateCapturedAtUtc
            : DateTime.SpecifyKind(exchangeRateCapturedAtUtc, DateTimeKind.Utc);
        ExchangeRateId = exchangeRateId;
        ReportingAmountIrr = ConvertToIrr(Amount, ExchangeRateToIrr);
        UploadedFilePath = GuardAgainst.String.InvalidLength(uploadedFilePath.Trim(), nameof(uploadedFilePath), 3, 1000);
        TrackingNumber = NormalizeOptional(trackingNumber, nameof(trackingNumber), 2, 120);
        PayerNote = NormalizeOptional(payerNote, nameof(payerNote), 2, 1000);
        EventDiscountCode = eventDiscountCode;
        EventDiscountCodeId = eventDiscountCode?.Id;
        DiscountCode = eventDiscountCode?.Code;
        Status = ManualPaymentReceiptStatus.Submitted;
        SubmittedAtUtc = DateTime.UtcNow;
        AddDomainEvent(new EntityCreatedEvent<ManualPaymentReceipt>(this));
    }

    public void LinkTicketOrder(TicketOrder ticketOrder)
    {
        TicketOrder = GuardAgainst.Object.Null(ticketOrder, nameof(ticketOrder));
        TicketOrderId = ticketOrder.Id > 0 ? ticketOrder.Id : null;
        UpdateTimestamp();
    }

    public void Approve(User reviewer, TicketOrder ticketOrder, EventTicket eventTicket)
    {
        if (Status != ManualPaymentReceiptStatus.Submitted)
            throw new BusinessRuleViolationException("Receipt already reviewed", "This manual payment receipt has already been reviewed.");

        var normalizedReviewer = GuardAgainst.Object.Null(reviewer, nameof(reviewer));
        TicketOrder = GuardAgainst.Object.Null(ticketOrder, nameof(ticketOrder));
        TicketOrderId = ticketOrder.Id;
        EventTicket = GuardAgainst.Object.Null(eventTicket, nameof(eventTicket));
        EventTicketId = eventTicket.Id;
        ReviewedByUserId = normalizedReviewer.Id;
        ReviewedByUser = normalizedReviewer;
        ReviewedAtUtc = DateTime.UtcNow;
        RejectReason = null;
        Status = ManualPaymentReceiptStatus.Approved;
        UpdateTimestamp();
    }

    public void ApproveAsWalletCredit(User reviewer, BalanceTransaction walletCreditTransaction)
    {
        if (Status != ManualPaymentReceiptStatus.Submitted)
            throw new BusinessRuleViolationException("Receipt already reviewed", "This manual payment receipt has already been reviewed.");

        var normalizedReviewer = GuardAgainst.Object.Null(reviewer, nameof(reviewer));
        WalletCreditTransaction = GuardAgainst.Object.Null(walletCreditTransaction, nameof(walletCreditTransaction));
        WalletCreditTransactionId = walletCreditTransaction.Id > 0 ? walletCreditTransaction.Id : null;
        ReviewedByUserId = normalizedReviewer.Id;
        ReviewedByUser = normalizedReviewer;
        ReviewedAtUtc = DateTime.UtcNow;
        RejectReason = null;
        Status = ManualPaymentReceiptStatus.Approved;
        UpdateTimestamp();
    }

    public void Reject(User reviewer, string reason)
    {
        if (Status != ManualPaymentReceiptStatus.Submitted)
            throw new BusinessRuleViolationException("Receipt already reviewed", "This manual payment receipt has already been reviewed.");

        var normalizedReviewer = GuardAgainst.Object.Null(reviewer, nameof(reviewer));
        ReviewedByUserId = normalizedReviewer.Id;
        ReviewedByUser = normalizedReviewer;
        ReviewedAtUtc = DateTime.UtcNow;
        RejectReason = GuardAgainst.String.InvalidLength(reason.Trim(), nameof(reason), 3, 1000);
        Status = ManualPaymentReceiptStatus.Rejected;
        UpdateTimestamp();
    }

    private static string? NormalizeOptional(string? value, string parameterName, int minLength, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return GuardAgainst.String.InvalidLength(value.Trim(), parameterName, minLength, maxLength);
    }

    private static decimal ConvertToIrr(decimal amount, decimal rate)
        => Math.Round(amount * rate, 0, MidpointRounding.AwayFromZero);
}
