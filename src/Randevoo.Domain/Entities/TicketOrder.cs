using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class TicketOrder : BaseEntity, IAggregateRoot
{
    private readonly List<EventTicket> _tickets = new();

    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long BuyerUserId { get; private set; }
    public User BuyerUser { get; private set; } = null!;
    public string CurrencyCode { get; private set; } = "IRR";
    public decimal GrossAmount { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public decimal PlatformCommissionAmount { get; private set; }
    public decimal OrganizerIncomeAmount { get; private set; }
    public EventPaymentCollectionMethod PaymentCollectionMethod { get; private set; }
    public TicketOrderPaymentStatus PaymentStatus { get; private set; }
    public TicketOrderStatus OrderStatus { get; private set; }
    public long? EventDiscountCodeId { get; private set; }
    public EventDiscountCode? EventDiscountCode { get; private set; }
    public string? DiscountCode { get; private set; }
    public string ReportingCurrencyCode { get; private set; } = "IRR";
    public decimal ReportingGrossAmountIrr { get; private set; }
    public decimal ReportingDiscountAmountIrr { get; private set; }
    public decimal ReportingNetAmountIrr { get; private set; }
    public decimal ReportingPlatformCommissionIrr { get; private set; }
    public decimal ReportingOrganizerIncomeIrr { get; private set; }
    public decimal ExchangeRateToIrr { get; private set; } = 1m;
    public DateTime ExchangeRateCapturedAtUtc { get; private set; }
    public long? ExchangeRateId { get; private set; }
    public CurrencyExchangeRate? ExchangeRate { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }
    public long? ApprovedByUserId { get; private set; }
    public User? ApprovedByUser { get; private set; }
    public string? Notes { get; private set; }
    public IReadOnlyList<EventTicket> Tickets => _tickets.AsReadOnly();

    private TicketOrder() { }

    public TicketOrder(
        DatingEvent datingEvent,
        User buyerUser,
        decimal grossAmount,
        decimal discountAmount,
        decimal netAmount,
        decimal platformCommissionAmount,
        EventPaymentCollectionMethod paymentCollectionMethod,
        string currencyCode,
        decimal exchangeRateToIrr,
        DateTime exchangeRateCapturedAtUtc,
        long? exchangeRateId = null,
        EventDiscountCode? eventDiscountCode = null,
        TicketOrderPaymentStatus paymentStatus = TicketOrderPaymentStatus.Pending,
        TicketOrderStatus orderStatus = TicketOrderStatus.PendingPayment,
        string? notes = null)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        DatingEventId = datingEvent.Id;
        BuyerUser = GuardAgainst.Object.Null(buyerUser, nameof(buyerUser));
        BuyerUserId = buyerUser.Id;
        CurrencyCode = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? datingEvent.CurrencyCode : currencyCode);
        GrossAmount = GuardAgainst.Number.OutOfRange(grossAmount, nameof(grossAmount), 0.01m, 10_000_000_000m);
        DiscountAmount = GuardAgainst.Number.OutOfRange(discountAmount, nameof(discountAmount), 0m, GrossAmount);
        NetAmount = GuardAgainst.Number.OutOfRange(netAmount, nameof(netAmount), 0.01m, GrossAmount);

        if (Math.Round(GrossAmount - DiscountAmount, 2, MidpointRounding.AwayFromZero) != Math.Round(NetAmount, 2, MidpointRounding.AwayFromZero))
            throw new BusinessRuleViolationException("Invalid order amounts", "Order net amount must equal gross amount minus discount amount.");

        PlatformCommissionAmount = GuardAgainst.Number.OutOfRange(platformCommissionAmount, nameof(platformCommissionAmount), 0m, NetAmount);
        OrganizerIncomeAmount = NetAmount - PlatformCommissionAmount;
        PaymentCollectionMethod = GuardAgainst.Number.AgainstInvalidEnum<EventPaymentCollectionMethod>((int)paymentCollectionMethod, nameof(paymentCollectionMethod));
        PaymentStatus = GuardAgainst.Number.AgainstInvalidEnum<TicketOrderPaymentStatus>((int)paymentStatus, nameof(paymentStatus));
        OrderStatus = GuardAgainst.Number.AgainstInvalidEnum<TicketOrderStatus>((int)orderStatus, nameof(orderStatus));
        ExchangeRateToIrr = GuardAgainst.Number.OutOfRange(exchangeRateToIrr, nameof(exchangeRateToIrr), 0.000001m, 1_000_000_000_000m);
        ExchangeRateCapturedAtUtc = exchangeRateCapturedAtUtc.Kind == DateTimeKind.Utc
            ? exchangeRateCapturedAtUtc
            : DateTime.SpecifyKind(exchangeRateCapturedAtUtc, DateTimeKind.Utc);
        ExchangeRateId = exchangeRateId;
        EventDiscountCode = eventDiscountCode;
        EventDiscountCodeId = eventDiscountCode?.Id;
        DiscountCode = eventDiscountCode?.Code;
        Notes = NormalizeOptional(notes, nameof(notes), 500);
        RecalculateReportingAmounts();

        if (PaymentStatus == TicketOrderPaymentStatus.Paid)
            PaidAtUtc = DateTime.UtcNow;

        AddDomainEvent(new EntityCreatedEvent<TicketOrder>(this));
    }

    internal void AddTicket(EventTicket ticket)
    {
        var normalizedTicket = GuardAgainst.Object.Null(ticket, nameof(ticket));
        if (_tickets.Any(item => ReferenceEquals(item, normalizedTicket)))
            return;

        _tickets.Add(normalizedTicket);
        UpdateTimestamp();
    }

    public void MarkPaid(long? approvedByUserId = null)
    {
        if (PaymentStatus == TicketOrderPaymentStatus.Paid && OrderStatus == TicketOrderStatus.Confirmed)
            return;

        if (PaymentStatus is TicketOrderPaymentStatus.Rejected or TicketOrderPaymentStatus.Refunded)
            throw new BusinessRuleViolationException("Order cannot be paid", "Rejected or refunded orders cannot be marked as paid.");

        PaymentStatus = TicketOrderPaymentStatus.Paid;
        OrderStatus = TicketOrderStatus.Confirmed;
        PaidAtUtc ??= DateTime.UtcNow;
        ApprovedAtUtc = DateTime.UtcNow;
        ApprovedByUserId = approvedByUserId;
        UpdateTimestamp();
    }

    public void MarkRejected(long reviewedByUserId, string reason)
    {
        if (PaymentStatus != TicketOrderPaymentStatus.Pending)
            throw new BusinessRuleViolationException("Order already reviewed", "Only pending orders can be rejected.");

        PaymentStatus = TicketOrderPaymentStatus.Rejected;
        OrderStatus = TicketOrderStatus.Cancelled;
        ApprovedAtUtc = DateTime.UtcNow;
        ApprovedByUserId = reviewedByUserId;
        Notes = GuardAgainst.String.InvalidLength(reason.Trim(), nameof(reason), 3, 500);
        UpdateTimestamp();
    }

    public void MarkRefunded()
    {
        PaymentStatus = TicketOrderPaymentStatus.Refunded;
        OrderStatus = TicketOrderStatus.Refunded;
        UpdateTimestamp();
    }

    private void RecalculateReportingAmounts()
    {
        ReportingCurrencyCode = "IRR";
        ReportingGrossAmountIrr = ConvertToIrr(GrossAmount, ExchangeRateToIrr);
        ReportingDiscountAmountIrr = ConvertToIrr(DiscountAmount, ExchangeRateToIrr);
        ReportingNetAmountIrr = ConvertToIrr(NetAmount, ExchangeRateToIrr);
        ReportingPlatformCommissionIrr = ConvertToIrr(PlatformCommissionAmount, ExchangeRateToIrr);
        ReportingOrganizerIncomeIrr = ConvertToIrr(OrganizerIncomeAmount, ExchangeRateToIrr);
    }

    private static string? NormalizeOptional(string? value, string parameterName, int maxLength)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), parameterName, maxLength);

    private static decimal ConvertToIrr(decimal amount, decimal rate)
        => Math.Round(amount * rate, 0, MidpointRounding.AwayFromZero);
}
