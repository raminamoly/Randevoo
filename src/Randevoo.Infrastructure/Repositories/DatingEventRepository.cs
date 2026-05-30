using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class DatingEventRepository : IDatingEventRepository
{
    private readonly RandevooDbContext _db;

    public DatingEventRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<DatingEvent?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _db.DatingEvents
            .Include(e => e.EventPlannerUser)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<DatingEvent?> GetByIdWithTicketsAsync(long id, CancellationToken cancellationToken = default)
    {
        return _db.DatingEvents
            .Include(e => e.EventPlannerUser)
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DatingEvent>> ListOpenAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        return await _db.DatingEvents
            .Where(e => e.IsOpenForSell && !e.IsCancelled)
            .OrderBy(e => e.DateTimeStart)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByPlannerAsync(long plannerUserId, CancellationToken cancellationToken = default)
    {
        return _db.DatingEvents.CountAsync(e => e.EventPlannerUserId == plannerUserId, cancellationToken);
    }

    public Task<int> CountCancelledByPlannerAsync(long plannerUserId, CancellationToken cancellationToken = default)
    {
        return _db.DatingEvents.CountAsync(e => e.EventPlannerUserId == plannerUserId && e.IsCancelled, cancellationToken);
    }

    public Task<int> CountCompletedByPlannerAsync(long plannerUserId, DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        return _db.DatingEvents.CountAsync(e => e.EventPlannerUserId == plannerUserId && !e.IsCancelled && e.DateTimeEnd <= nowUtc, cancellationToken);
    }

    public async Task AddAsync(DatingEvent datingEvent, CancellationToken cancellationToken = default)
    {
        _db.DatingEvents.Add(datingEvent);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(DatingEvent datingEvent, CancellationToken cancellationToken = default)
    {
        _db.DatingEvents.Update(datingEvent);
        await Task.CompletedTask;
    }
}
