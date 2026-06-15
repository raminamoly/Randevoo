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
    public int PlatformSupportTickets { get; init; }
    public int OrganizerTickets { get; init; }
    public int UnassignedTickets { get; init; }
    public IReadOnlyList<SupportTicketChartPoint> StatusChart { get; init; } = Array.Empty<SupportTicketChartPoint>();
    public IReadOnlyList<SupportTicketChartPoint> CategoryChart { get; init; } = Array.Empty<SupportTicketChartPoint>();
    public IReadOnlyList<SupportTicketChartPoint> RecipientChart { get; init; } = Array.Empty<SupportTicketChartPoint>();
    public IReadOnlyList<SupportTicketChartPoint> DailyCreatedChart { get; init; } = Array.Empty<SupportTicketChartPoint>();
}

public sealed record SupportTicketDashboardFilters(
    long? TicketStatusId,
    long? TicketTypeId,
    long? TicketRecipientTypeId,
    UserRole? SubmitterRole,
    long? AssigneeUserId,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToUtc);

public sealed record SupportTicketChartPoint(string Label, int Value);
