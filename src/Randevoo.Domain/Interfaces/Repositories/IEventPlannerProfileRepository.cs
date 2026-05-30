using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IEventPlannerProfileRepository
{
    Task<EventPlannerProfile?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task AddAsync(EventPlannerProfile profile, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventPlannerProfile profile, CancellationToken cancellationToken = default);
}
