using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public sealed class InterestRepository : IInterestRepository
{
    private readonly RandevooDbContext _db;

    public InterestRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Interest>> GetByNamesAsync(
        IReadOnlyCollection<string> names,
        CancellationToken cancellationToken = default)
    {
        if (names.Count == 0)
            return [];

        return await _db.Interests
            .Where(interest => names.Contains(interest.Name))
            .ToListAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Interest> interests, CancellationToken cancellationToken = default)
    {
        await _db.Interests.AddRangeAsync(interests, cancellationToken);
    }
}
