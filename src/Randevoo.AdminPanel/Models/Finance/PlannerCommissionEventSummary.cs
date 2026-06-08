namespace Randevoo.AdminPanel.Models.Finance;

public sealed class PlannerCommissionEventSummary
{
    public long EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime StartAtUtc { get; set; }
    public int TicketsSold { get; set; }
    public decimal GrossTicketSales { get; set; }
    public string CurrencyCode { get; set; } = "IRR";
    public decimal GrossTicketSalesIrr { get; set; }
    public decimal PlannerIncome { get; set; }
    public decimal PlannerIncomeIrr { get; set; }
    public decimal PlannerCommissionPercent { get; set; }
}
