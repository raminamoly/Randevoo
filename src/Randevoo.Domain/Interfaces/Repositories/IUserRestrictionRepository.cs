using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IUserRestrictionRepository
{
    Task<bool> HasActiveRestrictionAsync(long userId, UserRestrictionType restrictionType, DateTime nowUtc, CancellationToken cancellationToken = default);
    Task<UserRestriction?> GetActiveRestrictionAsync(long userId, UserRestrictionType restrictionType, DateTime nowUtc, CancellationToken cancellationToken = default);
    Task AddAsync(UserRestriction restriction, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserRestriction restriction, CancellationToken cancellationToken = default);
}
