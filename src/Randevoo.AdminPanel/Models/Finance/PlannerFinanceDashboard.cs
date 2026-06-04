namespace Randevoo.AdminPanel.Models.Finance;

public sealed class PlannerFinanceDashboard
{
    public decimal CurrentBalance { get; set; }
    public decimal TotalCommissionIncome { get; set; }
    public decimal PendingWithdrawalAmount { get; set; }
    public decimal PaidWithdrawalAmount { get; set; }
    public decimal AvailableWithdrawalAmount { get; set; }
    public IReadOnlyList<PlannerCommissionEventSummary> Events { get; set; } = Array.Empty<PlannerCommissionEventSummary>();
    public IReadOnlyList<PlannerCommissionTransactionItem> Transactions { get; set; } = Array.Empty<PlannerCommissionTransactionItem>();
    public IReadOnlyList<PlannerWithdrawalRequestItem> Withdrawals { get; set; } = Array.Empty<PlannerWithdrawalRequestItem>();
}
