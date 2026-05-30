using MediatR;

namespace Randevoo.Application.Features.EventParticipants.Commands.RemoveEventParticipant;

public record RemoveEventParticipantCommand(long ActorUserId, long EventId, long ParticipantUserId, string Reason) : IRequest;
