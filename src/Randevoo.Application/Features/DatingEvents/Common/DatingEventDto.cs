using Randevoo.Domain.Entities;

namespace Randevoo.Application.Features.DatingEvents.Common;

public record DatingEventDto(
    long Id,
    string Title,
    string EventType,
    string Country,
    string City,
    string Address,
    DateTime DateTimeStart,
    DateTime DateTimeEnd,
    bool IsOpenForSell,
    bool IsCancelled,
    long EventPlannerUserId,
    decimal EventPlannerCommissionPercent,
    int MaleCapacity,
    int FemaleCapacity,
    int NumberOfChatAllowed,
    decimal TicketPrice,
    string EventDescriptionHtml)
{
    public static DatingEventDto FromEntity(DatingEvent datingEvent) =>
        new(
            datingEvent.Id,
            datingEvent.Title,
            datingEvent.EventType,
            datingEvent.Location.Country,
            datingEvent.Location.City,
            datingEvent.Address,
            datingEvent.DateTimeStart,
            datingEvent.DateTimeEnd,
            datingEvent.IsOpenForSell,
            datingEvent.IsCancelled,
            datingEvent.EventPlannerUserId,
            datingEvent.EventPlannerCommissionPercent,
            datingEvent.MaleCapacity,
            datingEvent.FemaleCapacity,
            datingEvent.NumberOfChatAllowed,
            datingEvent.TicketPrice,
            datingEvent.EventDescriptionHtml);
}
