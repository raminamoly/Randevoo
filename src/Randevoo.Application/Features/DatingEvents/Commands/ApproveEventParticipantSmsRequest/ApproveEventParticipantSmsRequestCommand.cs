using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.ApproveEventParticipantSmsRequest;

public record ApproveEventParticipantSmsRequestCommand(long AdminUserId, long RequestId, string ApprovedMessage, DateTime? PlannedSendAtUtc, string? Note) : IRequest<int>;
