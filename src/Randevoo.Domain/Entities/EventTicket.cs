using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventTicket : BaseEntity
{
    public long TicketOrderId { get; private set; }
    public TicketOrder TicketOrder { get; private set; } = null!;
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    // UserId is kept for backward compatibility and represents the participant user.
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public long ParticipantUserId => UserId;
    public User ParticipantUser => User;
    public Gender Gender { get; private set; }
    public decimal OriginalPrice { get; private set; }
    public string CurrencyCode { get; private set; } = "IRR";
    public decimal ReportingOriginalPriceIrr { get; private set; }
    public decimal ReportingPriceIrr { get; private set; }
    public decimal ExchangeRateToIrr { get; private set; } = 1m;
    public DateTime ExchangeRateCapturedAtUtc { get; private set; }
    public long? ExchangeRateId { get; private set; }
    public CurrencyExchangeRate? ExchangeRate { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public long? EventDiscountCodeId { get; private set; }
    public EventDiscountCode? EventDiscountCode { get; private set; }
    public string? DiscountCode { get; private set; }
    public decimal Price { get; private set; }
    public bool IsRefunded { get; private set; }
    public bool IsRemoved { get; private set; }
    public string? RemovalReason { get; private set; }
    public long? RemovedByUserId { get; private set; }
    public DateTime? RemovedAt { get; private set; }
    public bool IsValidForEventAccess => !IsRefunded && !IsRemoved;

    private EventTicket() { }

    internal EventTicket(
        TicketOrder ticketOrder,
        DatingEvent datingEvent,
        User participantUser,
        Gender gender,
        decimal originalPrice,
        decimal finalPrice,
        string currencyCode,
        EventDiscountCode? discountCode = null)
    {
        TicketOrder = GuardAgainst.Object.Null(ticketOrder, nameof(ticketOrder));
        DatingEvent = datingEvent;
        User = participantUser;
        UserId = participantUser.Id;
        Gender = gender;
        OriginalPrice = GuardAgainst.Number.OutOfRange(originalPrice, nameof(originalPrice), 0.01m, 1_000_000_000m);
        Price = GuardAgainst.Number.OutOfRange(finalPrice, nameof(finalPrice), 0.01m, OriginalPrice);
        CurrencyCode = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        DiscountAmount = OriginalPrice - Price;
        ReportingOriginalPriceIrr = OriginalPrice;
        ReportingPriceIrr = Price;
        ExchangeRateToIrr = 1m;
        ExchangeRateCapturedAtUtc = DateTime.UtcNow;
        EventDiscountCode = discountCode;
        EventDiscountCodeId = discountCode?.Id;
        DiscountCode = discountCode?.Code;
        IsRefunded = false;
        IsRemoved = false;
        ticketOrder.AddTicket(this);
    }

    public void CaptureExchangeRate(decimal exchangeRateToIrr, DateTime capturedAtUtc, long? exchangeRateId = null)
    {
        ExchangeRateToIrr = GuardAgainst.Number.OutOfRange(exchangeRateToIrr, nameof(exchangeRateToIrr), 0.000001m, 1_000_000_000_000m);
        ExchangeRateCapturedAtUtc = capturedAtUtc.Kind == DateTimeKind.Utc
            ? capturedAtUtc
            : DateTime.SpecifyKind(capturedAtUtc, DateTimeKind.Utc);
        ExchangeRateId = exchangeRateId;
        ReportingOriginalPriceIrr = ConvertToIrr(OriginalPrice, ExchangeRateToIrr);
        ReportingPriceIrr = ConvertToIrr(Price, ExchangeRateToIrr);
        UpdateTimestamp();
    }

    public void MarkRefunded()
    {
        IsRefunded = true;
        UpdateTimestamp();
    }

    public void RemoveWithRefund(long removedByUserId, string reason)
    {
        if (IsRemoved)
            throw new BusinessRuleViolationException("Participant already removed", "This ticket was already removed from the event");

        IsRemoved = true;
        RemovalReason = GuardAgainst.String.InvalidLength(reason, nameof(reason), 5, 500);
        RemovedByUserId = removedByUserId;
        RemovedAt = DateTime.UtcNow;
        MarkRefunded();
    }

    private static decimal ConvertToIrr(decimal amount, decimal rate)
        => Math.Round(amount * rate, 0, MidpointRounding.AwayFromZero);
}
