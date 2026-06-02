using MediatR;
using Randevoo.Application.Features.DatingEvents.Common;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Queries.ListOpenDatingEvents;

public class ListOpenDatingEventsHandler : IRequestHandler<ListOpenDatingEventsQuery, IReadOnlyList<DatingEventDto>>
{
    private readonly IDatingEventRepository _events;

    public ListOpenDatingEventsHandler(IDatingEventRepository events)
    {
        _events = events;
    }

    public async Task<IReadOnlyList<DatingEventDto>> Handle(ListOpenDatingEventsQuery request, CancellationToken cancellationToken)
    {
        var events = await _events.ListOpenAsync(
            request.Limit,
            request.AfterId,
            request.City,
            request.DateFrom,
            request.DateTo,
            request.EventTypeId,
            request.PriceMin,
            request.PriceMax,
            request.GenderCapacityAvailable,
            cancellationToken);
        return events.Select(DatingEventDto.FromEntity).ToList();
    }
}
