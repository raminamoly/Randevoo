using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class PlannerBankAccountInput
{
    public long? Id { get; set; }

    [Required(ErrorMessage = "شماره کارت الزامی است.")]
    [StringLength(19, MinimumLength = 12, ErrorMessage = "شماره کارت معتبر نیست.")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "شماره شبا الزامی است.")]
    [StringLength(34, MinimumLength = 18, ErrorMessage = "شماره شبا معتبر نیست.")]
    public string Iban { get; set; } = string.Empty;

    [Required(ErrorMessage = "نام بانک الزامی است.")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "نام بانک معتبر نیست.")]
    public string BankName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
