using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public sealed class UserRestrictionRepository : IUserRestrictionRepository
{
    private readonly RandevooDbContext _db;

    public UserRestrictionRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<bool> HasActiveRestrictionAsync(long userId, UserRestrictionType restrictionType, DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        return _db.UserRestrictions.AnyAsync(
            restriction =>
                restriction.UserId == userId
                && restriction.RestrictionType == restrictionType
                && restriction.IsActive
                && (restriction.ExpiresAtUtc == null || restriction.ExpiresAtUtc > nowUtc),
            cancellationToken);
    }

    public Task<UserRestriction?> GetActiveRestrictionAsync(long userId, UserRestrictionType restrictionType, DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        return _db.UserRestrictions
            .Include(restriction => restriction.User)
            .FirstOrDefaultAsync(
                restriction =>
                    restriction.UserId == userId
                    && restriction.RestrictionType == restrictionType
                    && restriction.IsActive
                    && (restriction.ExpiresAtUtc == null || restriction.ExpiresAtUtc > nowUtc),
                cancellationToken);
    }

    public Task AddAsync(UserRestriction restriction, CancellationToken cancellationToken = default)
    {
        _db.UserRestrictions.Add(restriction);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(UserRestriction restriction, CancellationToken cancellationToken = default)
    {
        _db.UserRestrictions.Update(restriction);
        return Task.CompletedTask;
    }
}
