namespace Randevoo.Application.EndUsers.Events;

public sealed record EndUserEventEligibilityResult(
    bool CanBuyTicket,
    string ReasonCode,
    string Message);
