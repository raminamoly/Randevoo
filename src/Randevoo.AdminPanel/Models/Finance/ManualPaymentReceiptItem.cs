using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class ManualPaymentReceiptItem
{
    public long Id { get; set; }
    public long? TicketOrderId { get; set; }
    public long? WalletCreditTransactionId { get; set; }
    public long EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public long PlannerUserId { get; set; }
    public string PlannerName { get; set; } = string.Empty;
    public long ParticipantUserId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string ParticipantMobile { get; set; } = string.Empty;
    public long BuyerUserId { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public string BuyerMobile { get; set; } = string.Empty;
    public EventPaymentCollectionMethod PaymentCollectionMethod { get; set; }
    public ManualPaymentDestinationType DestinationType { get; set; }
    public ManualPaymentReceiptStatus Status { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public decimal ReportingAmountIrr { get; set; }
    public decimal ExchangeRateToIrr { get; set; }
    public string? DiscountCode { get; set; }
    public string UploadedFilePath { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    public string? PayerNote { get; set; }
    public DateTime SubmittedAtUtc { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public string? RejectReason { get; set; }
    public long? EventTicketId { get; set; }
}
