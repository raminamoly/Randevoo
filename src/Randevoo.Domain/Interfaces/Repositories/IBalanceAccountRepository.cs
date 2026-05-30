using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IBalanceAccountRepository
{
    Task<BalanceAccount?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default);
    Task AddAsync(BalanceAccount account, CancellationToken cancellationToken = default);
    Task UpdateAsync(BalanceAccount account, CancellationToken cancellationToken = default);
}
