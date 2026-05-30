using MediatR;
using Randevoo.Application.Features.DatingEvents.Common;

namespace Randevoo.Application.Features.DatingEvents.Queries.ListOpenDatingEvents;

public record ListOpenDatingEventsQuery(int Limit = 50) : IRequest<IReadOnlyList<DatingEventDto>>;
