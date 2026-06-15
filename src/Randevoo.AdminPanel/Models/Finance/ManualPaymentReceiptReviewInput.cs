using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class ManualPaymentReceiptReviewInput
{
    public long ReceiptId { get; set; }

    [StringLength(1000, MinimumLength = 3, ErrorMessage = "دلیل رد باید بین ۳ تا ۱۰۰۰ کاراکتر باشد.")]
    public string? RejectReason { get; set; }
}
