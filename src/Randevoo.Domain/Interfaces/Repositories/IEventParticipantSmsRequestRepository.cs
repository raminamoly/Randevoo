using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IEventParticipantSmsRequestRepository
{
    Task<EventParticipantSmsRequest?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task AddAsync(EventParticipantSmsRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventParticipantSmsRequest request, CancellationToken cancellationToken = default);
}
