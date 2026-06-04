using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.RequestEventParticipantSms;

public record RequestEventParticipantSmsCommand(long ActorUserId, long EventId, string Message, DateTime? PlannedSendAtUtc) : IRequest<long>;
