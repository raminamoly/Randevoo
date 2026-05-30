using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IEventConversationRepository
{
    Task<EventConversation?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default);
    Task<EventConversation?> GetBetweenParticipantsAsync(long eventId, long firstUserId, long secondUserId, CancellationToken cancellationToken = default);
    Task<int> CountActiveConnectionsForUserAsync(long eventId, long userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventConversation>> ListForUserAsync(long userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventConversation>> ListForEventUserAsync(long eventId, long userId, CancellationToken cancellationToken = default);
    Task AddAsync(EventConversation conversation, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventConversation conversation, CancellationToken cancellationToken = default);
}
