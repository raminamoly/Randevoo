using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IEventTypeRepository
{
    Task<EventType?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventType>> ListActiveAsync(CancellationToken cancellationToken = default);
    Task AddAsync(EventType eventType, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventType eventType, CancellationToken cancellationToken = default);
}
