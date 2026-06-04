using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface ISmsQueueRepository
{
    Task AddRangeAsync(IReadOnlyCollection<SmsQueueItem> items, CancellationToken cancellationToken = default);
}
