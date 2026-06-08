using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.DiscountCodes;

public sealed class EventDiscountCodeUsageItem
{
    public long TicketId { get; init; }
    public string BuyerName { get; init; } = string.Empty;
    public string BuyerMobile { get; init; } = string.Empty;
    public Gender BuyerGender { get; init; }
    public string EventTitle { get; init; } = string.Empty;
    public DateTime PurchasedAtUtc { get; init; }
    public decimal OriginalPrice { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal FinalPaidAmount { get; init; }
    public string CurrencyCode { get; init; } = "IRR";
    public bool IsRefunded { get; init; }
    public bool IsRemoved { get; init; }
}
