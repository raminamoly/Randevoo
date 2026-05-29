using Randevoo.Domain.Interfaces;

namespace Randevoo.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly RandevooDbContext _db;

    public UnitOfWork(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _db.SaveChangesAsync(cancellationToken);
    }
}
