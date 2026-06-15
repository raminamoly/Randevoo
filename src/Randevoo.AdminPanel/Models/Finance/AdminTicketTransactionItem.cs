namespace Randevoo.AdminPanel.Models.Finance;

public sealed class AdminTicketTransactionItem
{
    public long OrderId { get; set; }
    public long TransactionId { get; set; }
    public long BuyerUserId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerMobile { get; set; } = string.Empty;
    public int TicketCount { get; set; }
    public string? DiscountCode { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPaidAmount { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public decimal ReportingAmountIrr { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime PurchasedAtUtc { get; set; }
}
