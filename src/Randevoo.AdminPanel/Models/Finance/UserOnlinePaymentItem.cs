using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class UserOnlinePaymentItem
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public string GatewayName { get; set; } = string.Empty;
    public string TrackingCode { get; set; } = string.Empty;
    public OnlinePaymentStatus Status { get; set; }
    public long? EventId { get; set; }
    public string EventTitle { get; set; } = "بدون رویداد";
    public long? TicketId { get; set; }
    public long? BalanceTransactionId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? PaidAtUtc { get; set; }
}
