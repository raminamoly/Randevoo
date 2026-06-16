using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public sealed class ManualPaymentReceiptRepository : IManualPaymentReceiptRepository
{
    private readonly RandevooDbContext _db;

    public ManualPaymentReceiptRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(ManualPaymentReceipt receipt, CancellationToken cancellationToken = default)
    {
        _db.ManualPaymentReceipts.Add(receipt);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<ManualPaymentReceipt>> ListByOrderIdsAsync(IReadOnlyCollection<long> ticketOrderIds, CancellationToken cancellationToken = default)
    {
        if (ticketOrderIds.Count == 0)
            return Array.Empty<ManualPaymentReceipt>();

        return await _db.ManualPaymentReceipts
            .Include(receipt => receipt.ParticipantUser)
            .ThenInclude(user => user.Profile)
            .Where(receipt => receipt.TicketOrderId.HasValue && ticketOrderIds.Contains(receipt.TicketOrderId.Value))
            .OrderByDescending(receipt => receipt.SubmittedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> HasSubmittedReceiptAsync(long eventId, long participantUserId, CancellationToken cancellationToken = default)
    {
        return _db.ManualPaymentReceipts.AnyAsync(receipt =>
            receipt.DatingEventId == eventId
            && receipt.ParticipantUserId == participantUserId
            && receipt.Status == ManualPaymentReceiptStatus.Submitted, cancellationToken);
    }
}
