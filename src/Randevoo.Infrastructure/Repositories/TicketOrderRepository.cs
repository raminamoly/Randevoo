using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public sealed class TicketOrderRepository : ITicketOrderRepository
{
    private readonly RandevooDbContext _db;

    public TicketOrderRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(TicketOrder order, CancellationToken cancellationToken = default)
    {
        _db.TicketOrders.Add(order);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<TicketOrder>> ListForBuyerOrParticipantAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _db.TicketOrders
            .Include(order => order.DatingEvent)
            .Include(order => order.BuyerUser)
            .ThenInclude(user => user.Profile)
            .Include(order => order.Tickets)
            .ThenInclude(ticket => ticket.User)
            .ThenInclude(user => user.Profile)
            .Where(order => order.BuyerUserId == userId || order.Tickets.Any(ticket => ticket.UserId == userId))
            .OrderByDescending(order => order.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
