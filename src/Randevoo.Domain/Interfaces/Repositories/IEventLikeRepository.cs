using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IEventLikeRepository
{
    Task<EventLike?> GetDirectedAsync(long eventId, long fromUserId, long toUserId, CancellationToken cancellationToken = default);
    Task<EventLike?> GetReverseAsync(long eventId, long fromUserId, long toUserId, CancellationToken cancellationToken = default);
    Task AddAsync(EventLike eventLike, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventLike eventLike, CancellationToken cancellationToken = default);
}
