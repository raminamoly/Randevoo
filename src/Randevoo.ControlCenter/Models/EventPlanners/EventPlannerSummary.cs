namespace Randevoo.ControlCenter.Models.EventPlanners;

public sealed record EventPlannerSummary(
    Guid Id,
    string DisplayName,
    string CompanyName,
    string City,
    decimal Balance,
    int UpcomingEvents,
    bool IsVerified);
