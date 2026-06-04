using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Finance;

public sealed class UserFinanceTransactionItem
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public BalanceTransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? EventId { get; set; }
    public string EventTitle { get; set; } = "بدون رویداد";
    public DateTime CreatedAtUtc { get; set; }
}
