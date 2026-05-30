using MediatR;
using Randevoo.Application.Features.EventParticipants.Common;

namespace Randevoo.Application.Features.EventParticipants.Queries.ListEventParticipants;

public record ListEventParticipantsQuery(long ActorUserId, long EventId) : IRequest<IReadOnlyList<EventParticipantDto>>;
