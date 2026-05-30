using MediatR;
using Randevoo.Application.Features.EventParticipants.Common;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventParticipants.Queries.ListMyEventArchive;

public class ListMyEventArchiveHandler : IRequestHandler<ListMyEventArchiveQuery, IReadOnlyList<EventArchiveItemDto>>
{
    private readonly IEventTicketRepository _tickets;

    public ListMyEventArchiveHandler(IEventTicketRepository tickets)
    {
        _tickets = tickets;
    }

    public async Task<IReadOnlyList<EventArchiveItemDto>> Handle(ListMyEventArchiveQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _tickets.ListByUserIdAsync(request.UserId, cancellationToken);
        return tickets.Select(EventArchiveItemDto.FromTicket).ToList();
    }
}
