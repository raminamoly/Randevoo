using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IManualPaymentReceiptRepository
{
    Task AddAsync(ManualPaymentReceipt receipt, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ManualPaymentReceipt>> ListByOrderIdsAsync(IReadOnlyCollection<long> ticketOrderIds, CancellationToken cancellationToken = default);
    Task<bool> HasSubmittedReceiptAsync(long eventId, long participantUserId, CancellationToken cancellationToken = default);
}
