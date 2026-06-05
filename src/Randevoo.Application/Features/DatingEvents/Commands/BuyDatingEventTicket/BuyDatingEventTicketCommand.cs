using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.BuyDatingEventTicket;

public record BuyDatingEventTicketCommand(long BuyerUserId, long EventId, string? DiscountCode = null) : IRequest<long>;
