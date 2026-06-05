using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IEventDiscountCodeRepository
{
    Task<EventDiscountCode?> GetApplicableByCodeAsync(long eventId, string code, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventDiscountCode discountCode, CancellationToken cancellationToken = default);
}
