using MediatR;
using Randevoo.Application.Features.EventChats.Common;

namespace Randevoo.Application.Features.EventChats.Commands.SendEventChatMessage;

public record SendEventChatMessageCommand(long SenderUserId, long ConversationId, string Body) : IRequest<EventConversationDto>;
