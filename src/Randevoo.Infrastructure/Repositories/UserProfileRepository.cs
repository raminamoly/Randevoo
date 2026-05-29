
using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class UserProfileRepository : IUserProfileRepository
{
    private readonly RandevooDbContext _db;

    public UserProfileRepository(RandevooDbContext db) => _db = db;

    public async Task AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        _db.UserProfiles.Add(userProfile);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        _db.UserProfiles.Update(userProfile);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(UserProfile userProfile, CancellationToken cancellationToken = default)
    {
        userProfile.SoftDelete();
        _db.UserProfiles.Update(userProfile);
        await Task.CompletedTask;
    }

    public async Task<UserProfile?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<UserProfile?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles
            .Include(p => p.User)
            .Include(p => p.Interests)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<UserProfile?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles
            .Include(p => p.User)
            .Include(p => p.Interests)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<UserProfile?> GetByDisplayNameAsync(string displayName, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles.FirstOrDefaultAsync(p => p.DisplayName == displayName, cancellationToken);
    }

    public async Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.User.Email == email, cancellationToken);
    }

    public async Task<IReadOnlyList<UserProfile>> GetPotentialMatchesAsync(long userId, int limit = 20, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles
            .Where(p => p.UserId != userId && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserProfile>> GetNearbyUsersAsync(Coordinates location, int maxDistanceKm, int limit = 50, CancellationToken cancellationToken = default)
    {
        // Naive in-memory filter; for production use spatial queries / NetTopologySuite
        var all = await _db.UserProfiles.Include(p => p.Location).ToListAsync(cancellationToken);
        var nearby = all
            .Where(p => p.Location?.Coordinates != null && p.Location.Coordinates.DistanceTo(location) <= maxDistanceKm)
            .OrderBy(p => p.Location.Coordinates.DistanceTo(location))
            .Take(limit)
            .ToList();
        return nearby;
    }

    public async Task<IReadOnlyList<UserProfile>> GetUsersByInterestsAsync(IReadOnlyList<Interest> interests, int limit = 30, CancellationToken cancellationToken = default)
    {
        var names = interests.Select(i => i.Name).ToList();
        return await _db.UserProfiles
            .Include(p => p.Interests)
            .Where(p => p.Interests.Any(i => names.Contains(i.Name)))
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<UserProfile>> GetNewUsersSinceAsync(DateTime since, int limit = 100, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles
            .Where(p => p.CreatedAt >= since)
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetActiveUsersCountAsync(DateTime lastActiveSince, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles.CountAsync(p => (p.UpdatedAt >= lastActiveSince) || (p.CreatedAt >= lastActiveSince), cancellationToken);
    }

    public async Task<IReadOnlyList<UserProfile>> GetMultipleByIdsAsync(IReadOnlyList<long> userIds, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles.Where(p => userIds.Contains(p.UserId)).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByDisplayNameAsync(string displayName, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles.AnyAsync(p => p.DisplayName == displayName, cancellationToken);
    }

    public async Task<bool> ExistsByDisplayNameAsync(string displayName, long excludedProfileId, CancellationToken cancellationToken = default)
    {
        return await _db.UserProfiles.AnyAsync(p => p.DisplayName == displayName && p.Id != excludedProfileId, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _db.Users.AnyAsync(u => u.Email == email || u.PendingEmail == email, cancellationToken);
    }
}
