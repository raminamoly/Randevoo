using MediatR;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Application.EndUsers.Events;

public sealed class GetEndUserEventDetailsHandler : IRequestHandler<GetEndUserEventDetailsQuery, EndUserEventDetailsDto>
{
    private readonly IEndUserEventCatalogReader _reader;

    public GetEndUserEventDetailsHandler(IEndUserEventCatalogReader reader)
    {
        _reader = reader;
    }

    public async Task<EndUserEventDetailsDto> Handle(GetEndUserEventDetailsQuery request, CancellationToken cancellationToken)
    {
        var details = await _reader.GetDetailsAsync(request.EventId, request.UserId, cancellationToken);
        if (details is null)
            throw new NotFoundException("Event", request.EventId);

        return details;
    }
}
