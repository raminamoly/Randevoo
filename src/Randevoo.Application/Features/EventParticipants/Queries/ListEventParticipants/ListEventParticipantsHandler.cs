using MediatR;
using Randevoo.Application.Features.EventParticipants.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventParticipants.Queries.ListEventParticipants;

public class ListEventParticipantsHandler : IRequestHandler<ListEventParticipantsQuery, IReadOnlyList<EventParticipantDto>>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IEventTicketRepository _tickets;

    public ListEventParticipantsHandler(IUserRepository users, IDatingEventRepository ticketsEvents, IEventTicketRepository tickets)
    {
        _users = users;
        _events = ticketsEvents;
        _tickets = tickets;
    }

    public async Task<IReadOnlyList<EventParticipantDto>> Handle(ListEventParticipantsQuery request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can view event participants");

        var tickets = await _tickets.ListByEventIdAsync(request.EventId, cancellationToken);
        return tickets
            .Where(ticket => ticket.User.Profile is not null)
            .Select(EventParticipantDto.FromTicket)
            .ToList();
    }
}
