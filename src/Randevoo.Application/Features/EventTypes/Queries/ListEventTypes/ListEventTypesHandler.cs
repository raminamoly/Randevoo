using MediatR;
using Randevoo.Application.Features.EventTypes.Common;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventTypes.Queries.ListEventTypes;

public class ListEventTypesHandler : IRequestHandler<ListEventTypesQuery, IReadOnlyList<EventTypeDto>>
{
    private readonly IEventTypeRepository _eventTypes;

    public ListEventTypesHandler(IEventTypeRepository eventTypes)
    {
        _eventTypes = eventTypes;
    }

    public async Task<IReadOnlyList<EventTypeDto>> Handle(ListEventTypesQuery request, CancellationToken cancellationToken)
    {
        var eventTypes = await _eventTypes.ListActiveAsync(cancellationToken);
        return eventTypes.Select(EventTypeDto.FromEntity).ToList();
    }
}
