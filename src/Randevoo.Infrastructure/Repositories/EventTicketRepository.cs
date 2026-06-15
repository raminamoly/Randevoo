using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class EventTicketRepository : IEventTicketRepository
{
    private readonly RandevooDbContext _db;

    public EventTicketRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<EventTicket?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _db.EventTickets
            .Include(t => t.DatingEvent)
            .Include(t => t.User)
            .ThenInclude(u => u.Profile)
            .Include(t => t.TicketOrder)
            .ThenInclude(order => order.BuyerUser)
            .ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public Task<EventTicket?> GetByEventAndUserAsync(long eventId, long userId, CancellationToken cancellationToken = default)
    {
        return _db.EventTickets
            .Include(t => t.DatingEvent)
            .Include(t => t.User)
            .ThenInclude(u => u.Profile)
            .Include(t => t.TicketOrder)
            .ThenInclude(order => order.BuyerUser)
            .ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(t => t.DatingEventId == eventId && t.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<EventTicket>> ListByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _db.EventTickets
            .Include(t => t.DatingEvent)
            .Include(t => t.TicketOrder)
            .ThenInclude(order => order.BuyerUser)
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.DatingEvent.DateTimeStart)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventTicket>> ListByEventIdAsync(long eventId, CancellationToken cancellationToken = default)
    {
        return await _db.EventTickets
            .Include(t => t.User)
            .ThenInclude(u => u.Profile)
            .Include(t => t.TicketOrder)
            .ThenInclude(order => order.BuyerUser)
            .ThenInclude(user => user.Profile)
            .Where(t => t.DatingEventId == eventId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(EventTicket ticket, CancellationToken cancellationToken = default)
    {
        _db.EventTickets.Update(ticket);
        await Task.CompletedTask;
    }
}
