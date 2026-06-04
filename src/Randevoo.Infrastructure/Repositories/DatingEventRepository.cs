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
            .Include(e => e.EventType)
            .Include(e => e.Country)
            .Include(e => e.City)
            .Include(e => e.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<DatingEvent?> GetByIdWithTicketsAsync(long id, CancellationToken cancellationToken = default)
    {
        return _db.DatingEvents
            .Include(e => e.EventPlannerUser)
            .Include(e => e.EventType)
            .Include(e => e.Country)
            .Include(e => e.City)
            .Include(e => e.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .Include(e => e.Tickets)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<DatingEvent>> ListOpenAsync(
        int limit = 50,
        long? afterId = null,
        string? city = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        long? eventTypeId = null,
        decimal? priceMin = null,
        decimal? priceMax = null,
        string? genderCapacityAvailable = null,
        CancellationToken cancellationToken = default)
    {
        var query = _db.DatingEvents
            .Include(e => e.EventType)
            .Include(e => e.Country)
            .Include(e => e.City)
            .Include(e => e.EventTags)
            .ThenInclude(eventTag => eventTag.Tag)
            .Include(e => e.Tickets)
            .Where(e => e.IsOpenForSell && !e.IsCancelled)
            .AsQueryable();

        if (afterId is not null)
            query = query.Where(e => e.Id > afterId);
        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(e => e.City != null && e.City.Name == city);
        if (dateFrom is not null)
            query = query.Where(e => e.DateTimeStart >= dateFrom);
        if (dateTo is not null)
            query = query.Where(e => e.DateTimeStart <= dateTo);
        if (eventTypeId is not null)
            query = query.Where(e => e.EventTypeId == eventTypeId);
        if (priceMin is not null)
            query = query.Where(e => e.TicketPrice >= priceMin);
        if (priceMax is not null)
            query = query.Where(e => e.TicketPrice <= priceMax);

        var events = await query
            .OrderBy(e => e.Id)
            .Take(Math.Clamp(limit, 1, 100))
            .ToListAsync(cancellationToken);

        return genderCapacityAvailable?.Trim().ToLowerInvariant() switch
        {
            "male" => events.Where(e => e.Tickets.Count(t => !t.IsRefunded && t.Gender == Randevoo.Domain.Enums.Gender.Male) < e.MaleCapacity).ToList(),
            "female" => events.Where(e => e.Tickets.Count(t => !t.IsRefunded && t.Gender == Randevoo.Domain.Enums.Gender.Female) < e.FemaleCapacity).ToList(),
            _ => events
        };
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
