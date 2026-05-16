// Domain/Interfaces/IUserProfileRepository.cs
using Randevoo.Domain.Entities;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Domain.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default);

    Task<UserProfile?> GetByDisplayNameAsync(string displayName, CancellationToken cancellationToken = default);
    Task<UserProfile?> GetByEmailAsync(string email, CancellationToken cancellationToken = default); // For auth

    Task<IReadOnlyList<UserProfile>> GetPotentialMatchesAsync(
        long userId,
        int limit = 20,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserProfile>> GetNearbyUsersAsync(
        Coordinates location,
        int maxDistanceKm,
        int limit = 50,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserProfile>> GetUsersByInterestsAsync(
        IReadOnlyList<Interest> interests,
        int limit = 30,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserProfile>> GetNewUsersSinceAsync(
        DateTime since,
        int limit = 100,
        CancellationToken cancellationToken = default);

    // Existence checks
    Task<bool> ExistsByDisplayNameAsync(string displayName, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);

    // Statistics
    Task<int> GetActiveUsersCountAsync(DateTime lastActiveSince, CancellationToken cancellationToken = default);

    // Write operations
    Task AddAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserProfile userProfile, CancellationToken cancellationToken = default);
    Task DeleteAsync(UserProfile userProfile, CancellationToken cancellationToken = default);

    // Batch operations
    Task<IReadOnlyList<UserProfile>> GetMultipleByIdsAsync(
        IReadOnlyList<long> userIds,
        CancellationToken cancellationToken = default);
}