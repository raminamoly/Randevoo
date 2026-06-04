namespace Randevoo.AdminPanel.Models.Finance;

public sealed class PlannerBankAccountItem
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string CardNumber { get; set; } = string.Empty;
    public string Iban { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
