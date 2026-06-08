using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Support;

public sealed class SupportTicketDashboardViewModel
{
    public int TotalTickets { get; init; }
    public int OpenTickets { get; init; }
    public int InProgressTickets { get; init; }
    public int WaitingForUserTickets { get; init; }
    public int ClosedTickets { get; init; }
    public int ReopenedTickets { get; init; }
    public int FinancialTickets { get; init; }
    public int EventTickets { get; init; }
    public int QuestionTickets { get; init; }
    public int UnassignedTickets { get; init; }
    public IReadOnlyList<SupportTicketChartPoint> StatusChart { get; init; } = Array.Empty<SupportTicketChartPoint>();
    public IReadOnlyList<SupportTicketChartPoint> CategoryChart { get; init; } = Array.Empty<SupportTicketChartPoint>();
    public IReadOnlyList<SupportTicketChartPoint> DailyCreatedChart { get; init; } = Array.Empty<SupportTicketChartPoint>();
}

public sealed record SupportTicketDashboardFilters(
    SupportTicketStatus? Status,
    SupportTicketCategory? Category,
    UserRole? SubmitterRole,
    long? AssigneeUserId,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToUtc);

public sealed record SupportTicketChartPoint(string Label, int Value);
