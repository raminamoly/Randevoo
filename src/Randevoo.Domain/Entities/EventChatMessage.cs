using Randevoo.Domain.Common;

namespace Randevoo.Domain.Entities;

public class EventChatMessage : BaseEntity
{
    public long EventConversationId { get; private set; }
    public EventConversation EventConversation { get; private set; } = null!;
    public long SenderUserId { get; private set; }
    public User SenderUser { get; private set; } = null!;
    public string Body { get; private set; } = null!;

    private EventChatMessage() { }

    internal EventChatMessage(EventConversation conversation, long senderUserId, string body)
    {
        EventConversation = GuardAgainst.Object.Null(conversation, nameof(conversation));
        SenderUserId = senderUserId;
        Body = GuardAgainst.String.InvalidLength(body, nameof(body), 1, 2000);
    }
}
