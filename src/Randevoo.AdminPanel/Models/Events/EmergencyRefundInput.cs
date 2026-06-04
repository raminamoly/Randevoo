using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Models.Events;

public sealed class EmergencyRefundInput
{
    [Required]
    public long TicketId { get; set; }

    [Required(ErrorMessage = "ثبت دلیل بازگشت وجه الزامی است.")]
    [StringLength(500, MinimumLength = 5, ErrorMessage = "دلیل باید بین ۵ تا ۵۰۰ کاراکتر باشد.")]
    public string Reason { get; set; } = string.Empty;
}
