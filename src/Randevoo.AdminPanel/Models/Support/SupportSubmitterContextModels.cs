using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Support;

public sealed record SupportSubmitterFinanceContext(
    decimal Balance,
    string ReportingCurrencyCode,
    IReadOnlyList<SupportSubmitterTransactionItem> Transactions,
    IReadOnlyList<SupportSubmitterPaymentItem> Payments);

public sealed record SupportSubmitterTransactionItem(
    long Id,
    decimal Amount,
    string CurrencyCode,
    decimal ReportingAmountIrr,
    BalanceTransactionType Type,
    string Description,
    long? EventId,
    string EventTitle,
    DateTime CreatedAtUtc);

public sealed record SupportSubmitterPaymentItem(
    long Id,
    decimal Amount,
    string CurrencyCode,
    decimal ReportingAmountIrr,
    string GatewayName,
    string TrackingCode,
    OnlinePaymentStatus Status,
    long? EventId,
    string EventTitle,
    long? TicketId,
    DateTime CreatedAtUtc,
    DateTime? PaidAtUtc);

public sealed record SupportSubmitterEventBookingItem(
    long TicketId,
    long EventId,
    string EventTitle,
    string PlannerDisplayName,
    DateTime StartsAtUtc,
    decimal Price,
    string CurrencyCode,
    string Status,
    DateTime PurchasedAtUtc);
