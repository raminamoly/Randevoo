using MediatR;
using Randevoo.Application.Features.EventChats.Common;

namespace Randevoo.Application.Features.EventChats.Commands.StartEventConversation;

public record StartEventConversationCommand(long StarterUserId, long EventId, long ParticipantUserId) : IRequest<EventLikeResultDto>;
