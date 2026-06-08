using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Support;

public sealed record SupportSubmitterFinanceContext(
    decimal Balance,
    IReadOnlyList<SupportSubmitterTransactionItem> Transactions,
    IReadOnlyList<SupportSubmitterPaymentItem> Payments);

public sealed record SupportSubmitterTransactionItem(
    long Id,
    decimal Amount,
    BalanceTransactionType Type,
    string Description,
    long? EventId,
    string EventTitle,
    DateTime CreatedAtUtc);

public sealed record SupportSubmitterPaymentItem(
    long Id,
    decimal Amount,
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
