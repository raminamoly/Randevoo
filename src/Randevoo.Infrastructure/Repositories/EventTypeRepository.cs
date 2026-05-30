using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class EventTypeRepository : IEventTypeRepository
{
    private readonly RandevooDbContext _db;

    public EventTypeRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<EventType?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        _db.EventTypes.FirstOrDefaultAsync(type => type.Id == id, cancellationToken);

    public async Task<IReadOnlyList<EventType>> ListActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _db.EventTypes
            .Where(type => type.IsActive)
            .OrderBy(type => type.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(EventType eventType, CancellationToken cancellationToken = default)
    {
        _db.EventTypes.Add(eventType);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(EventType eventType, CancellationToken cancellationToken = default)
    {
        _db.EventTypes.Update(eventType);
        await Task.CompletedTask;
    }
}
