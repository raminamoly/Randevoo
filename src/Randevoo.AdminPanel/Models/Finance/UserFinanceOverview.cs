namespace Randevoo.AdminPanel.Models.Finance;

public sealed class UserFinanceOverview
{
    public long UserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public decimal Balance { get; set; }
    public IReadOnlyList<UserFinanceTransactionItem> Transactions { get; set; } = Array.Empty<UserFinanceTransactionItem>();
    public IReadOnlyList<UserOnlinePaymentItem> OnlinePayments { get; set; } = Array.Empty<UserOnlinePaymentItem>();
}
