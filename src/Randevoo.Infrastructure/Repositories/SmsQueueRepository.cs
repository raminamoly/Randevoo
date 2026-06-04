using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class SmsQueueRepository : ISmsQueueRepository
{
    private readonly RandevooDbContext _db;

    public SmsQueueRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task AddRangeAsync(IReadOnlyCollection<SmsQueueItem> items, CancellationToken cancellationToken = default)
    {
        _db.SmsQueueItems.AddRange(items);
        await Task.CompletedTask;
    }
}
