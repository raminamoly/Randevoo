namespace Randevoo.AdminPanel.Models.Finance;

public sealed class AdminTicketTransactionItem
{
    public long TransactionId { get; set; }
    public long BuyerUserId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerMobile { get; set; } = string.Empty;
    public string? DiscountCode { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPaidAmount { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime PurchasedAtUtc { get; set; }
}
