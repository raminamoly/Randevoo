using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IEventTicketRepository
{
    Task<EventTicket?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<EventTicket?> GetByEventAndUserAsync(long eventId, long userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventTicket>> ListByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventTicket>> ListByEventIdAsync(long eventId, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventTicket ticket, CancellationToken cancellationToken = default);
}
