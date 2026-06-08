using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.EventChats.Common;

public record EventLikeResultDto(
    long LikeId,
    long EventId,
    long FromUserId,
    long ToUserId,
    EventLikeStatus Status,
    EventConversationDto? Conversation)
{
    public static EventLikeResultDto FromEntity(EventLike eventLike, EventConversation? conversation = null) =>
        new(
            eventLike.Id,
            eventLike.DatingEventId,
            eventLike.FromUserId,
            eventLike.ToUserId,
            eventLike.Status,
            conversation is null ? null : EventConversationDto.FromEntity(conversation));
}
