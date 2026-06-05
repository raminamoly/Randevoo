using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public sealed class EventDiscountCodeRepository : IEventDiscountCodeRepository
{
    private readonly RandevooDbContext _db;

    public EventDiscountCodeRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<EventDiscountCode?> GetApplicableByCodeAsync(long eventId, string code, CancellationToken cancellationToken = default)
    {
        var normalizedCode = (code ?? string.Empty).Trim().ToUpperInvariant();
        return _db.EventDiscountCodes
            .Where(item => item.Code == normalizedCode && (item.DatingEventId == eventId || item.DatingEventId == null))
            .OrderByDescending(item => item.DatingEventId == eventId)
            .ThenByDescending(item => item.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(EventDiscountCode discountCode, CancellationToken cancellationToken = default)
    {
        _db.EventDiscountCodes.Update(discountCode);
        await Task.CompletedTask;
    }
}
