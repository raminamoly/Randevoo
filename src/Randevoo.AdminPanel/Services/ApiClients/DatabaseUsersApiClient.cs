using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseUsersApiClient : IUsersApiClient
{
    private readonly RandevooDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    public DatabaseUsersApiClient(RandevooDbContext db, IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<MockUser>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _db.Users
            .Include(item => item.Profile)
            .Where(item => item.Role != UserRole.EndUser)
            .OrderBy(item => item.Role)
            .ThenBy(item => item.Profile != null ? item.Profile.DisplayName : item.MobileNumber)
            .ToListAsync(cancellationToken);

        return users.Select(DatabaseModelMapper.ToAdminUser).ToList();
    }

    public async Task<MockUser?> GetUserAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == id && item.Role != UserRole.EndUser, cancellationToken);

        return user is null ? null : DatabaseModelMapper.ToAdminUser(user);
    }

    public async Task<MockUser> UpsertUserAsync(UserUpsertInput input, long? existingUserId = null, CancellationToken cancellationToken = default)
    {
        var normalizedMobile = (input.Mobile ?? string.Empty).Trim();
        var normalizedFullName = string.IsNullOrWhiteSpace(input.FullName) ? normalizedMobile : input.FullName.Trim();
        var domainRole = DatabaseModelMapper.ToDomainRole(input.Role);

        User user;
        if (existingUserId is long userId)
        {
            user = await _db.Users
                .Include(item => item.Profile)
                .FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
                ?? throw new InvalidOperationException("کاربر مورد نظر پیدا نشد.");

            if (!string.Equals(user.MobileNumber, normalizedMobile, StringComparison.Ordinal))
            {
                var mobileTaken = await _db.Users.AnyAsync(item => item.Id != user.Id && item.MobileNumber == normalizedMobile, cancellationToken);
                if (mobileTaken)
                    throw new InvalidOperationException("این شماره موبایل قبلاً ثبت شده است.");

                user.UpdateMobileNumber(normalizedMobile);
            }
        }
        else
        {
            var existingByMobile = await _db.Users
                .Include(item => item.Profile)
                .FirstOrDefaultAsync(item => item.MobileNumber == normalizedMobile, cancellationToken);

            user = existingByMobile ?? new User(normalizedMobile);
            if (existingByMobile is null)
            {
                _db.Users.Add(user);
            }
        }

        if (user.Role != domainRole)
        {
            user.ChangeUserRole(domainRole);
        }

        if (input.IsActive && !user.IsActive)
            user.Activate();
        else if (!input.IsActive && user.IsActive)
            user.Deactivate();

        EnsureProfile(user, normalizedFullName);

        if (domainRole == UserRole.EventPlanner)
        {
            await EnsurePlannerProfileAsync(user, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return DatabaseModelMapper.ToAdminUser(user);
    }

    private async Task EnsurePlannerProfileAsync(User user, CancellationToken cancellationToken)
    {
        var existingProfile = await _db.EventPlannerProfiles.FirstOrDefaultAsync(item => item.UserId == user.Id, cancellationToken);
        if (existingProfile is not null)
            return;

        _db.EventPlannerProfiles.Add(new EventPlannerProfile(
            user,
            "برگزارکننده رندوو",
            "/images/logo.png",
            "این پروفایل توسط مدیر ساخته شده و جزئیات کامل تر آن بعداً تکمیل می شود."));
    }

    private static void EnsureProfile(User user, string fullName)
    {
        var location = new Location("ایران", "تهران", new Coordinates(35.7219m, 51.3347m), "تهران");

        if (user.Profile is null)
        {
            user.CreateProfile(fullName, new DateOnly(1990, 1, 1), Gender.Unknown, location, new Height(170));
            return;
        }

        user.Profile.UpdateDisplayName(fullName);
    }
}
