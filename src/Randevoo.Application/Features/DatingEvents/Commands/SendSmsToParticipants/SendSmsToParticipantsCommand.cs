using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.SendSmsToParticipants;

public record SendSmsToParticipantsCommand(long ActorUserId, long EventId, string Message) : IRequest;
