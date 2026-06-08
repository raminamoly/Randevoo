using MediatR;

namespace Randevoo.Application.Features.EventChats.Commands.RejectEventLike;

public record RejectEventLikeCommand(long RejectingUserId, long EventId, long FromUserId) : IRequest;
