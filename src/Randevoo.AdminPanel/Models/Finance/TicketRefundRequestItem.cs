using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class TicketRefundRequestItem
{
    public long Id { get; set; }
    public long EventTicketId { get; set; }
    public long TicketOrderId { get; set; }
    public long EventId { get; set; }
    public string EventTitle { get; set; } = "";
    public long BuyerUserId { get; set; }
    public string BuyerName { get; set; } = "";
    public string BuyerMobile { get; set; } = "";
    public long ParticipantUserId { get; set; }
    public string ParticipantName { get; set; } = "";
    public string ParticipantMobile { get; set; } = "";
    public TicketRefundRequestStatus Status { get; set; }
    public decimal RequestedAmount { get; set; }
    public decimal ApprovedAmount { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public decimal ReportingRequestedAmountIrr { get; set; }
    public decimal ReportingApprovedAmountIrr { get; set; }
    public string RequestReason { get; set; } = "";
    public DateTime RequestedAtUtc { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? ReviewNote { get; set; }
    public long? WalletCreditTransactionId { get; set; }
}
