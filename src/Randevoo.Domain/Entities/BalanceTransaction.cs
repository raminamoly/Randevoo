using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Entities;

public class BalanceTransaction : BaseEntity
{
    public long BalanceAccountId { get; private set; }
    public BalanceAccount BalanceAccount { get; private set; } = null!;
    public long UserId { get; private set; }
    public decimal Amount { get; private set; }
    public string CurrencyCode { get; private set; } = "IRR";
    public string ReportingCurrencyCode { get; private set; } = "IRR";
    public decimal ReportingAmountIrr { get; private set; }
    public decimal ExchangeRateToIrr { get; private set; } = 1m;
    public DateTime ExchangeRateCapturedAtUtc { get; private set; }
    public long? ExchangeRateId { get; private set; }
    public CurrencyExchangeRate? ExchangeRate { get; private set; }
    public BalanceTransactionType Type { get; private set; }
    public string Description { get; private set; } = null!;
    public long? DatingEventId { get; private set; }
    public long? TicketOrderId { get; private set; }
    public TicketOrder? TicketOrder { get; private set; }
    public string? ReferenceType { get; private set; }
    public long? ReferenceId { get; private set; }
    public long? CreatedByUserId { get; private set; }

    private BalanceTransaction() { }

    internal BalanceTransaction(
        BalanceAccount account,
        decimal amount,
        BalanceTransactionType type,
        string description,
        long? datingEventId,
        string currencyCode,
        decimal reportingAmountIrr,
        decimal exchangeRateToIrr,
        DateTime exchangeRateCapturedAtUtc,
        long? exchangeRateId = null)
    {
        BalanceAccount = account;
        UserId = account.UserId;
        Amount = amount;
        CurrencyCode = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        ReportingCurrencyCode = "IRR";
        ReportingAmountIrr = reportingAmountIrr;
        ExchangeRateToIrr = GuardAgainst.Number.OutOfRange(exchangeRateToIrr, nameof(exchangeRateToIrr), 0.000001m, 1_000_000_000_000m);
        ExchangeRateCapturedAtUtc = exchangeRateCapturedAtUtc.Kind == DateTimeKind.Utc
            ? exchangeRateCapturedAtUtc
            : DateTime.SpecifyKind(exchangeRateCapturedAtUtc, DateTimeKind.Utc);
        ExchangeRateId = exchangeRateId;
        Type = type;
        Description = GuardAgainst.String.InvalidLength(description, nameof(description), 2, 300);
        DatingEventId = datingEventId;
    }

    public void SetReference(string? referenceType, long? referenceId, long? createdByUserId)
    {
        ReferenceType = string.IsNullOrWhiteSpace(referenceType) ? null : GuardAgainst.String.MaxLength(referenceType, nameof(referenceType), 100);
        ReferenceId = referenceId;
        CreatedByUserId = createdByUserId;
    }

    public void SetTicketOrder(TicketOrder? ticketOrder)
    {
        TicketOrder = ticketOrder;
        TicketOrderId = ticketOrder is not null && ticketOrder.Id > 0 ? ticketOrder.Id : null;
    }
}
