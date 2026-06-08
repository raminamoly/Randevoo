using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.SupportTickets.Queries.ListSupportTickets;

public class ListSupportTicketsHandler : IRequestHandler<ListSupportTicketsQuery, IReadOnlyList<SupportTicketListItemDto>>
{
    private readonly IUserRepository _users;
    private readonly ISupportTicketRepository _tickets;

    public ListSupportTicketsHandler(IUserRepository users, ISupportTicketRepository tickets)
    {
        _users = users;
        _tickets = tickets;
    }

    public async Task<IReadOnlyList<SupportTicketListItemDto>> Handle(ListSupportTicketsQuery request, CancellationToken cancellationToken)
    {
        var requester = await _users.GetByIdAsync(request.RequesterUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.RequesterUserId);
        var tickets = await _tickets.ListAsync(requester.Id, requester.Role, request.Status, request.Category, request.SubmitterRole, request.AssigneeUserId, request.CreatedFromUtc, request.CreatedToUtc, request.Limit, cancellationToken);
        return tickets.Select(SupportTicketDtoMapper.ToListItem).ToList();
    }
}
