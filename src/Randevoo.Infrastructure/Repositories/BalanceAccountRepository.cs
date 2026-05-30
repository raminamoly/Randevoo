using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class BalanceAccountRepository : IBalanceAccountRepository
{
    private readonly RandevooDbContext _db;

    public BalanceAccountRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<BalanceAccount?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return _db.BalanceAccounts
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(BalanceAccount account, CancellationToken cancellationToken = default)
    {
        _db.BalanceAccounts.Add(account);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(BalanceAccount account, CancellationToken cancellationToken = default)
    {
        _db.BalanceAccounts.Update(account);
        await Task.CompletedTask;
    }
}
