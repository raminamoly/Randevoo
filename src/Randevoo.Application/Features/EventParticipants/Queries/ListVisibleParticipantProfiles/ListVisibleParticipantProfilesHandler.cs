using MediatR;
using Randevoo.Application.Features.DatingProfile.Common;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventParticipants.Queries.ListVisibleParticipantProfiles;

public class ListVisibleParticipantProfilesHandler : IRequestHandler<ListVisibleParticipantProfilesQuery, IReadOnlyList<DatingProfileDto>>
{
    private readonly IEventTicketRepository _tickets;

    public ListVisibleParticipantProfilesHandler(IEventTicketRepository tickets)
    {
        _tickets = tickets;
    }

    public async Task<IReadOnlyList<DatingProfileDto>> Handle(ListVisibleParticipantProfilesQuery request, CancellationToken cancellationToken)
    {
        var requesterTicket = await _tickets.GetByEventAndUserAsync(request.EventId, request.UserId, cancellationToken)
            ?? throw new BusinessRuleViolationException("Ticket required", "User must have a ticket for this event");

        if (!requesterTicket.IsValidForEventAccess)
            throw new BusinessRuleViolationException("Ticket is not valid", "Refunded or removed tickets cannot access event participants");

        if (requesterTicket.DatingEvent.DateTimeStart > DateTime.UtcNow)
            throw new BusinessRuleViolationException("Event has not started", "Participant profiles are visible after event start time");

        var tickets = await _tickets.ListByEventIdAsync(request.EventId, cancellationToken);
        return tickets
            .Where(ticket => ticket.UserId != request.UserId && ticket.IsValidForEventAccess && ticket.User.Profile is not null)
            .Select(ticket => DatingProfileDto.FromEntity(ticket.User.Profile!))
            .ToList();
    }
}
