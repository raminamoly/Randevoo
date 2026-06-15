using Randevoo.Application.Features.DatingProfile.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.EventParticipants.Common;

public record EventParticipantDto(
    long TicketId,
    long UserId,
    string MobileNumber,
    Gender Gender,
    decimal Price,
    bool IsRefunded,
    bool IsRemoved,
    string? RemovalReason,
    DateTime TicketCreatedAt,
    DatingProfileDto Profile,
    long? TicketOrderId = null,
    long? BuyerUserId = null)
{
    public static EventParticipantDto FromTicket(EventTicket ticket) =>
        new(
            ticket.Id,
            ticket.UserId,
            ticket.User.MobileNumber,
            ticket.Gender,
            ticket.Price,
            ticket.IsRefunded,
            ticket.IsRemoved,
            ticket.RemovalReason,
            ticket.CreatedAt,
            DatingProfileDto.FromEntity(ticket.User.Profile!),
            ticket.TicketOrderId,
            ticket.TicketOrder?.BuyerUserId);
}
