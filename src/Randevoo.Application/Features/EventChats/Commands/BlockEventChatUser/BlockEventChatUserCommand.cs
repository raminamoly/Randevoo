using MediatR;

namespace Randevoo.Application.Features.EventChats.Commands.BlockEventChatUser;

public record BlockEventChatUserCommand(long BlockerUserId, long ConversationId, long BlockedUserId) : IRequest;
