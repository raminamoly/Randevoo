using MediatR;

namespace Randevoo.Application.EndUsers.Events;

public sealed record GetEndUserEventDetailsQuery(long EventId, long? UserId) : IRequest<EndUserEventDetailsDto>;
