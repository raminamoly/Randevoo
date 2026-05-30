using Randevoo.Domain.Common;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventConversation : BaseEntity, IAggregateRoot
{
    private readonly List<EventChatMessage> _messages = new();
    private readonly List<EventChatBlock> _blocks = new();

    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long StarterUserId { get; private set; }
    public User StarterUser { get; private set; } = null!;
    public long ParticipantUserId { get; private set; }
    public User ParticipantUser { get; private set; } = null!;
    public bool IsDisabled { get; private set; }
    public string? DisabledReason { get; private set; }
    public long? DisabledByUserId { get; private set; }
    public DateTime? DisabledAt { get; private set; }
    public IReadOnlyList<EventChatMessage> Messages => _messages.AsReadOnly();
    public IReadOnlyList<EventChatBlock> Blocks => _blocks.AsReadOnly();

    private EventConversation() { }

    public EventConversation(DatingEvent datingEvent, User starterUser, User participantUser)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        StarterUser = GuardAgainst.Object.Null(starterUser, nameof(starterUser));
        ParticipantUser = GuardAgainst.Object.Null(participantUser, nameof(participantUser));

        if (starterUser.Id == participantUser.Id)
            throw new BusinessRuleViolationException("Invalid conversation", "User cannot start a chat with themselves");

        DatingEventId = datingEvent.Id;
        StarterUserId = starterUser.Id;
        ParticipantUserId = participantUser.Id;
        IsDisabled = false;
        AddDomainEvent(new EntityCreatedEvent<EventConversation>(this));
    }

    public EventChatMessage SendMessage(long senderUserId, string body)
    {
        EnsureParticipant(senderUserId);
        if (IsDisabled)
            throw new BusinessRuleViolationException("Conversation disabled", "This conversation is no longer active");

        if (IsBlockedBetweenUsers())
            throw new BusinessRuleViolationException("Chat blocked", "Blocked users cannot send messages to each other");

        var message = new EventChatMessage(this, senderUserId, body);
        _messages.Add(message);
        UpdateTimestamp();
        return message;
    }

    public EventChatBlock Block(long blockerUserId, long blockedUserId)
    {
        EnsureParticipant(blockerUserId);
        EnsureParticipant(blockedUserId);
        if (blockerUserId == blockedUserId)
            throw new BusinessRuleViolationException("Invalid block", "User cannot block themselves");

        var existing = _blocks.FirstOrDefault(b => b.BlockerUserId == blockerUserId && b.BlockedUserId == blockedUserId);
        if (existing is not null)
            return existing;

        var block = new EventChatBlock(this, blockerUserId, blockedUserId);
        _blocks.Add(block);
        UpdateTimestamp();
        return block;
    }

    public bool HasParticipant(long userId) => StarterUserId == userId || ParticipantUserId == userId;

    public bool IsBlockedBetweenUsers() => _blocks.Any(b => b.IsActive);

    public void Disable(long disabledByUserId, string reason)
    {
        if (IsDisabled)
            return;

        IsDisabled = true;
        DisabledByUserId = disabledByUserId;
        DisabledReason = GuardAgainst.String.InvalidLength(reason, nameof(reason), 5, 500);
        DisabledAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    private void EnsureParticipant(long userId)
    {
        if (!HasParticipant(userId))
            throw new BusinessRuleViolationException("Access denied", "User is not part of this conversation");
    }
}
