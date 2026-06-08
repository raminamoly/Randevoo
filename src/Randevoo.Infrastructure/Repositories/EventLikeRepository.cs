using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class EventLikeRepository : IEventLikeRepository
{
    private readonly RandevooDbContext _db;

    public EventLikeRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<EventLike?> GetDirectedAsync(long eventId, long fromUserId, long toUserId, CancellationToken cancellationToken = default)
    {
        return _db.EventLikes
            .Include(item => item.DatingEvent)
            .FirstOrDefaultAsync(item => item.DatingEventId == eventId && item.FromUserId == fromUserId && item.ToUserId == toUserId, cancellationToken);
    }

    public Task<EventLike?> GetReverseAsync(long eventId, long fromUserId, long toUserId, CancellationToken cancellationToken = default)
    {
        return GetDirectedAsync(eventId, toUserId, fromUserId, cancellationToken);
    }

    public Task AddAsync(EventLike eventLike, CancellationToken cancellationToken = default)
    {
        _db.EventLikes.Add(eventLike);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(EventLike eventLike, CancellationToken cancellationToken = default)
    {
        _db.EventLikes.Update(eventLike);
        return Task.CompletedTask;
    }
}
