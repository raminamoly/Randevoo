using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class OnlinePayment : BaseEntity, IAggregateRoot
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public long? DatingEventId { get; private set; }
    public DatingEvent? DatingEvent { get; private set; }
    public long? EventTicketId { get; private set; }
    public EventTicket? EventTicket { get; private set; }
    public long? TicketOrderId { get; private set; }
    public TicketOrder? TicketOrder { get; private set; }
    public long? BalanceTransactionId { get; private set; }
    public BalanceTransaction? BalanceTransaction { get; private set; }
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = "IRR";
    public decimal ReportingAmountIrr { get; private set; }
    public decimal ExchangeRateToIrr { get; private set; } = 1m;
    public DateTime ExchangeRateCapturedAtUtc { get; private set; }
    public long? ExchangeRateId { get; private set; }
    public CurrencyExchangeRate? ExchangeRate { get; private set; }
    public string GatewayName { get; private set; } = null!;
    public string TrackingCode { get; private set; } = null!;
    public OnlinePaymentStatus Status { get; private set; }
    public DateTime? PaidAtUtc { get; private set; }
    public string? FailureReason { get; private set; }

    private OnlinePayment() { }

    public OnlinePayment(
        User user,
        decimal amount,
        string gatewayName,
        string trackingCode,
        OnlinePaymentStatus status = OnlinePaymentStatus.Pending,
        DatingEvent? datingEvent = null,
        EventTicket? eventTicket = null,
        BalanceTransaction? balanceTransaction = null,
        string currencyCode = "IRR",
        decimal? reportingAmountIrr = null,
        decimal exchangeRateToIrr = 1m,
        DateTime? exchangeRateCapturedAtUtc = null,
        long? exchangeRateId = null,
        TicketOrder? ticketOrder = null)
    {
        User = GuardAgainst.Object.Null(user, nameof(user));
        UserId = user.Id;
        Amount = GuardAgainst.Number.OutOfRange(amount, nameof(amount), 0.01m, 10_000_000_000m);
        CurrencyCode = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        ReportingAmountIrr = reportingAmountIrr ?? Amount;
        ExchangeRateToIrr = GuardAgainst.Number.OutOfRange(exchangeRateToIrr, nameof(exchangeRateToIrr), 0.000001m, 1_000_000_000_000m);
        ExchangeRateCapturedAtUtc = (exchangeRateCapturedAtUtc ?? DateTime.UtcNow).Kind == DateTimeKind.Utc
            ? exchangeRateCapturedAtUtc ?? DateTime.UtcNow
            : DateTime.SpecifyKind(exchangeRateCapturedAtUtc!.Value, DateTimeKind.Utc);
        ExchangeRateId = exchangeRateId;
        GatewayName = GuardAgainst.String.InvalidLength(gatewayName.Trim(), nameof(gatewayName), 2, 80);
        TrackingCode = GuardAgainst.String.InvalidLength(trackingCode.Trim(), nameof(trackingCode), 2, 120);
        DatingEvent = datingEvent;
        DatingEventId = datingEvent?.Id;
        EventTicket = eventTicket;
        EventTicketId = eventTicket?.Id;
        TicketOrder = ticketOrder;
        TicketOrderId = ticketOrder is not null && ticketOrder.Id > 0 ? ticketOrder.Id : null;
        BalanceTransaction = balanceTransaction;
        BalanceTransactionId = balanceTransaction?.Id;
        Status = GuardAgainst.Number.AgainstInvalidEnum<OnlinePaymentStatus>((int)status, nameof(status));
        PaidAtUtc = status == OnlinePaymentStatus.Succeeded ? DateTime.UtcNow : null;
        AddDomainEvent(new EntityCreatedEvent<OnlinePayment>(this));
    }

    public void LinkBalanceTransaction(BalanceTransaction balanceTransaction)
    {
        BalanceTransaction = GuardAgainst.Object.Null(balanceTransaction, nameof(balanceTransaction));
        BalanceTransactionId = balanceTransaction.Id;
        UpdateTimestamp();
    }

    public void MarkSucceeded()
    {
        Status = OnlinePaymentStatus.Succeeded;
        PaidAtUtc = DateTime.UtcNow;
        FailureReason = null;
        UpdateTimestamp();
    }

    public void MarkFailed(string reason)
    {
        Status = OnlinePaymentStatus.Failed;
        FailureReason = GuardAgainst.String.InvalidLength(reason.Trim(), nameof(reason), 3, 500);
        UpdateTimestamp();
    }

    public void MarkRefunded()
    {
        Status = OnlinePaymentStatus.Refunded;
        UpdateTimestamp();
    }
}
