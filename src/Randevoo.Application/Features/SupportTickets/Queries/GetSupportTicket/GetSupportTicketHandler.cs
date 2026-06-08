using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.SupportTickets.Queries.GetSupportTicket;

public class GetSupportTicketHandler : IRequestHandler<GetSupportTicketQuery, SupportTicketDetailDto>
{
    private readonly IUserRepository _users;
    private readonly ISupportTicketRepository _tickets;

    public GetSupportTicketHandler(IUserRepository users, ISupportTicketRepository tickets)
    {
        _users = users;
        _tickets = tickets;
    }

    public async Task<SupportTicketDetailDto> Handle(GetSupportTicketQuery request, CancellationToken cancellationToken)
    {
        var requester = await _users.GetByIdAsync(request.RequesterUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.RequesterUserId);
        var ticket = await _tickets.GetByIdWithDetailsAsync(request.TicketId, cancellationToken)
            ?? throw new NotFoundException("SupportTicket", request.TicketId);
        if (!ticket.CanBeViewedBy(requester))
            throw new BusinessRuleViolationException("Access denied", "You cannot view this ticket");

        return SupportTicketDtoMapper.ToDetail(ticket);
    }
}
