using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Buyers;

public sealed class TicketOrderListResult
{
    public int TotalCount { get; set; }
    public TicketOrderListSummary Summary { get; set; } = new();
    public IReadOnlyList<TicketOrderListItem> Items { get; set; } = Array.Empty<TicketOrderListItem>();
}

public sealed class TicketOrderListSummary
{
    public int TotalOrders { get; set; }
    public int PaidOrders { get; set; }
    public int PendingOrders { get; set; }
    public int TicketCount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal ReportingNetAmountIrr { get; set; }
}

public sealed class TicketOrderListItem
{
    public long OrderId { get; init; }
    public long EventId { get; init; }
    public string EventTitle { get; init; } = string.Empty;
    public long EventPlannerUserId { get; init; }
    public string EventPlannerName { get; init; } = string.Empty;
    public long BuyerUserId { get; init; }
    public string BuyerName { get; init; } = string.Empty;
    public string BuyerMobile { get; init; } = string.Empty;
    public int TicketCount { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal NetAmount { get; init; }
    public string CurrencyCode { get; init; } = "IRR";
    public decimal ReportingNetAmountIrr { get; init; }
    public string? DiscountCode { get; init; }
    public EventPaymentCollectionMethod PaymentCollectionMethod { get; init; }
    public TicketOrderPaymentStatus PaymentStatus { get; init; }
    public TicketOrderStatus OrderStatus { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? PaidAtUtc { get; init; }
}
