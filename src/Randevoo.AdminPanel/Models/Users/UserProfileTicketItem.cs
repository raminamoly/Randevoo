namespace Randevoo.AdminPanel.Models.Users;

public sealed class UserProfileTicketItem
{
    public long EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime StartAtUtc { get; set; }
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public bool IsRefunded { get; set; }
    public bool IsRemoved { get; set; }
}
