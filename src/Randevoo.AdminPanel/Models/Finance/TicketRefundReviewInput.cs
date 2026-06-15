using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class TicketRefundReviewInput
{
    [Range(0.01, double.MaxValue, ErrorMessage = "مبلغ تایید شده باید بیشتر از صفر باشد.")]
    public decimal ApprovedAmount { get; set; }

    [StringLength(1000, MinimumLength = 3, ErrorMessage = "توضیح بررسی باید بین ۳ تا ۱۰۰۰ کاراکتر باشد.")]
    public string? ReviewNote { get; set; }
}
