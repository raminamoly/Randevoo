namespace Randevoo.AdminPanel.Models.Finance;

public sealed class PlannerBankAccountItem
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public string PayoutMethod { get; set; } = string.Empty;
    public string AccountHolderName { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? CardNumber { get; set; }
    public string? Iban { get; set; }
    public string? BankName { get; set; }
    public string? AccountNumber { get; set; }
    public string? SwiftCode { get; set; }
    public string? AccountIdentifier { get; set; }
    public string? PublicPaymentInstructions { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
