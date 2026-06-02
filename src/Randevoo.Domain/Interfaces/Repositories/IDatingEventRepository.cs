using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IDatingEventRepository
{
    Task<DatingEvent?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<DatingEvent?> GetByIdWithTicketsAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DatingEvent>> ListOpenAsync(
        int limit = 50,
        long? afterId = null,
        string? city = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        long? eventTypeId = null,
        decimal? priceMin = null,
        decimal? priceMax = null,
        string? genderCapacityAvailable = null,
        CancellationToken cancellationToken = default);
    Task<int> CountByPlannerAsync(long plannerUserId, CancellationToken cancellationToken = default);
    Task<int> CountCancelledByPlannerAsync(long plannerUserId, CancellationToken cancellationToken = default);
    Task<int> CountCompletedByPlannerAsync(long plannerUserId, DateTime nowUtc, CancellationToken cancellationToken = default);
    Task AddAsync(DatingEvent datingEvent, CancellationToken cancellationToken = default);
    Task UpdateAsync(DatingEvent datingEvent, CancellationToken cancellationToken = default);
}
