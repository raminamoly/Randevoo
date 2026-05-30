using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class EventConversationRepository : IEventConversationRepository
{
    private readonly RandevooDbContext _db;

    public EventConversationRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<EventConversation?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return _db.EventConversations
            .Include(c => c.DatingEvent)
            .Include(c => c.Messages)
            .Include(c => c.Blocks)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public Task<EventConversation?> GetBetweenParticipantsAsync(long eventId, long firstUserId, long secondUserId, CancellationToken cancellationToken = default)
    {
        return _db.EventConversations
            .Include(c => c.DatingEvent)
            .Include(c => c.Messages)
            .Include(c => c.Blocks)
            .FirstOrDefaultAsync(c =>
                c.DatingEventId == eventId &&
                ((c.StarterUserId == firstUserId && c.ParticipantUserId == secondUserId) ||
                 (c.StarterUserId == secondUserId && c.ParticipantUserId == firstUserId)),
                cancellationToken);
    }

    public Task<int> CountActiveConnectionsForUserAsync(long eventId, long userId, CancellationToken cancellationToken = default)
    {
        return _db.EventConversations
            .CountAsync(c => !c.IsDisabled && c.DatingEventId == eventId && (c.StarterUserId == userId || c.ParticipantUserId == userId), cancellationToken);
    }

    public async Task<IReadOnlyList<EventConversation>> ListForUserAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _db.EventConversations
            .Include(c => c.DatingEvent)
            .Include(c => c.Messages)
            .Include(c => c.Blocks)
            .Where(c => c.StarterUserId == userId || c.ParticipantUserId == userId)
            .OrderByDescending(c => c.UpdatedAt ?? c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventConversation>> ListForEventUserAsync(long eventId, long userId, CancellationToken cancellationToken = default)
    {
        return await _db.EventConversations
            .Include(c => c.Messages)
            .Include(c => c.Blocks)
            .Where(c => c.DatingEventId == eventId && (c.StarterUserId == userId || c.ParticipantUserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(EventConversation conversation, CancellationToken cancellationToken = default)
    {
        _db.EventConversations.Add(conversation);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(EventConversation conversation, CancellationToken cancellationToken = default)
    {
        _db.EventConversations.Update(conversation);
        await Task.CompletedTask;
    }
}
