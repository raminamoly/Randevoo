using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface ISupportTicketRepository
{
    Task<SupportTicket?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupportTicket>> ListAsync(
        long requesterUserId,
        UserRole requesterRole,
        long? ticketStatusId = null,
        long? ticketTypeId = null,
        long? ticketRecipientTypeId = null,
        UserRole? submitterRole = null,
        long? assigneeUserId = null,
        DateTime? createdFromUtc = null,
        DateTime? createdToUtc = null,
        int limit = 100,
        CancellationToken cancellationToken = default);
    Task<User?> GetNextRoundRobinAssigneeAsync(CancellationToken cancellationToken = default);
    Task<bool> IsTicketTypeActiveAsync(long ticketTypeId, CancellationToken cancellationToken = default);
    Task<bool> IsTicketStatusActiveAsync(long ticketStatusId, CancellationToken cancellationToken = default);
    Task<bool> IsTicketRecipientTypeActiveAsync(long ticketRecipientTypeId, CancellationToken cancellationToken = default);
    Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
    Task UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken = default);
}
