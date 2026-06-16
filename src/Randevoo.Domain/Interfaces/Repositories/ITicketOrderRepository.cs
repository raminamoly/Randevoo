using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface ITicketOrderRepository
{
    Task AddAsync(TicketOrder order, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TicketOrder>> ListForBuyerOrParticipantAsync(long userId, CancellationToken cancellationToken = default);
}
