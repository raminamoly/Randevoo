using MediatR;

namespace Randevoo.Application.EndUsers.Events;

public sealed record ListEndUserEventsQuery(EndUserEventCatalogRequest Request) : IRequest<EndUserEventCatalogPageDto>;
