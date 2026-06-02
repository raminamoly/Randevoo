namespace Randevoo.ControlCenter.Models.Events;

public sealed record EventSummary(
    Guid Id,
    string Title,
    string PlannerName,
    string City,
    DateTimeOffset StartsAt,
    EventStatus Status,
    int Capacity,
    int TicketsSold,
    decimal GrossSales);
