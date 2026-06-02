using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly RandevooDbContext _db;

    public RefreshTokenRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken = default)
    {
        return await _db.RefreshTokens.FirstOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> ListByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _db.RefreshTokens
            .Where(token => token.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        await _db.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        _db.RefreshTokens.Update(refreshToken);
        await Task.CompletedTask;
    }
}
