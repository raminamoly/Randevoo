using System.ComponentModel.DataAnnotations;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class PlannerBankAccountInput
{
    public long? Id { get; set; }

    public string CurrencyCode { get; set; } = "IRR";

    public PlannerPayoutMethod PayoutMethod { get; set; } = PlannerPayoutMethod.IranianBankCard;

    [StringLength(120, MinimumLength = 2, ErrorMessage = "نام صاحب حساب معتبر نیست.")]
    public string AccountHolderName { get; set; } = string.Empty;

    [StringLength(80, MinimumLength = 2, ErrorMessage = "کشور معتبر نیست.")]
    public string? Country { get; set; }

    [StringLength(19, MinimumLength = 12, ErrorMessage = "شماره کارت معتبر نیست.")]
    public string? CardNumber { get; set; }

    [StringLength(34, MinimumLength = 18, ErrorMessage = "شماره شبا معتبر نیست.")]
    public string? Iban { get; set; }

    [StringLength(80, MinimumLength = 2, ErrorMessage = "نام بانک معتبر نیست.")]
    public string? BankName { get; set; }

    [StringLength(80, MinimumLength = 3, ErrorMessage = "شماره حساب معتبر نیست.")]
    public string? AccountNumber { get; set; }

    [StringLength(20, MinimumLength = 6, ErrorMessage = "کد SWIFT/BIC معتبر نیست.")]
    public string? SwiftCode { get; set; }

    [StringLength(160, MinimumLength = 3, ErrorMessage = "شناسه حساب معتبر نیست.")]
    public string? AccountIdentifier { get; set; }

    [StringLength(1200, MinimumLength = 10, ErrorMessage = "توضیحات پرداخت باید بین ۱۰ تا ۱۲۰۰ کاراکتر باشد.")]
    public string? PublicPaymentInstructions { get; set; }

    public bool IsActive { get; set; } = true;
}
