using MediatR;
using Randevoo.Application.Features.EventTypes.Common;

namespace Randevoo.Application.Features.EventTypes.Queries.ListEventTypes;

public record ListEventTypesQuery : IRequest<IReadOnlyList<EventTypeDto>>;
