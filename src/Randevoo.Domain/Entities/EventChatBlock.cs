using Randevoo.Domain.Common;

namespace Randevoo.Domain.Entities;

public class EventChatBlock : BaseEntity
{
    public long EventConversationId { get; private set; }
    public EventConversation EventConversation { get; private set; } = null!;
    public long BlockerUserId { get; private set; }
    public User BlockerUser { get; private set; } = null!;
    public long BlockedUserId { get; private set; }
    public User BlockedUser { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private EventChatBlock() { }

    internal EventChatBlock(EventConversation conversation, long blockerUserId, long blockedUserId)
    {
        EventConversation = GuardAgainst.Object.Null(conversation, nameof(conversation));
        BlockerUserId = blockerUserId;
        BlockedUserId = blockedUserId;
        IsActive = true;
    }
}
