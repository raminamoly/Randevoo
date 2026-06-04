using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.RejectEventParticipantSmsRequest;

public record RejectEventParticipantSmsRequestCommand(long AdminUserId, long RequestId, string Note) : IRequest;
