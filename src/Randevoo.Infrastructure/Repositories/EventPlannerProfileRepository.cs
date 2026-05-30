using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class EventPlannerProfileRepository : IEventPlannerProfileRepository
{
    private readonly RandevooDbContext _db;

    public EventPlannerProfileRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<EventPlannerProfile?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return _db.EventPlannerProfiles.Include(p => p.User).FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(EventPlannerProfile profile, CancellationToken cancellationToken = default)
    {
        _db.EventPlannerProfiles.Add(profile);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(EventPlannerProfile profile, CancellationToken cancellationToken = default)
    {
        _db.EventPlannerProfiles.Update(profile);
        await Task.CompletedTask;
    }
}
