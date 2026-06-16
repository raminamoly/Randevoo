using MediatR;

namespace Randevoo.Application.EndUsers.Events;

public sealed class ListEndUserEventsHandler : IRequestHandler<ListEndUserEventsQuery, EndUserEventCatalogPageDto>
{
    private readonly IEndUserEventCatalogReader _reader;

    public ListEndUserEventsHandler(IEndUserEventCatalogReader reader)
    {
        _reader = reader;
    }

    public Task<EndUserEventCatalogPageDto> Handle(ListEndUserEventsQuery request, CancellationToken cancellationToken)
    {
        if (request.Request.OnlyEligibleForMe && request.Request.UserId is null)
            throw new UnauthorizedAccessException("Login is required to filter events by your profile.");

        return _reader.ListAsync(request.Request, cancellationToken);
    }
}
