using Randevoo.Domain.Entities;

namespace Randevoo.Application.Features.EventChats.Common;

public record EventConversationDto(
    long Id,
    long EventId,
    long StarterUserId,
    long ParticipantUserId,
    bool IsBlocked,
    IReadOnlyList<EventChatMessageDto> Messages)
{
    public static EventConversationDto FromEntity(EventConversation conversation) =>
        new(
            conversation.Id,
            conversation.DatingEventId,
            conversation.StarterUserId,
            conversation.ParticipantUserId,
            conversation.IsBlockedBetweenUsers(),
            conversation.Messages.OrderBy(message => message.CreatedAt).Select(EventChatMessageDto.FromEntity).ToList());
}

public record EventChatMessageDto(long Id, long SenderUserId, string Body, DateTime CreatedAt)
{
    public static EventChatMessageDto FromEntity(EventChatMessage message) =>
        new(message.Id, message.SenderUserId, message.Body, message.CreatedAt);
}
