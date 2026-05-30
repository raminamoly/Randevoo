using Randevoo.Domain.Entities;

namespace Randevoo.Application.Features.EventParticipants.Common;

public record EventArchiveItemDto(
    long TicketId,
    long EventId,
    string EventTitle,
    DateTime DateTimeStart,
    DateTime DateTimeEnd,
    decimal Price,
    bool IsRefunded,
    bool IsRemoved,
    string? RemovalReason)
{
    public static EventArchiveItemDto FromTicket(EventTicket ticket) =>
        new(
            ticket.Id,
            ticket.DatingEventId,
            ticket.DatingEvent.Title,
            ticket.DatingEvent.DateTimeStart,
            ticket.DatingEvent.DateTimeEnd,
            ticket.Price,
            ticket.IsRefunded,
            ticket.IsRemoved,
            ticket.RemovalReason);
}
