using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.CancelDatingEvent;

public record CancelDatingEventCommand(long ActorUserId, long EventId) : IRequest;
