namespace Randevoo.AdminPanel.Models.Finance;

public sealed class AdminEventTicketTransactionGroup
{
    public long EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime StartAtUtc { get; set; }
    public string PlannerName { get; set; } = string.Empty;
    public int TicketCount { get; set; }
    public decimal TotalTicketAmount { get; set; }
    public IReadOnlyList<AdminTicketTransactionItem> Transactions { get; set; } = Array.Empty<AdminTicketTransactionItem>();
}
