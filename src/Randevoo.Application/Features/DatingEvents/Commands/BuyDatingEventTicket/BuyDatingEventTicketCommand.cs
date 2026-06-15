using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.BuyDatingEventTicket;

public record BuyDatingEventTicketCommand(
    long BuyerUserId,
    long EventId,
    string? DiscountCode = null,
    long? ParticipantUserId = null) : IRequest<TicketOrderPurchaseResult>;

public record TicketOrderPurchaseResult(long TicketOrderId, IReadOnlyList<long> TicketIds)
{
    public long TicketId => TicketIds.FirstOrDefault();
}
