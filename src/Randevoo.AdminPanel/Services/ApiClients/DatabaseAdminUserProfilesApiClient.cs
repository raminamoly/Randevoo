using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseAdminUserProfilesApiClient : IAdminUserProfilesApiClient
{
    private readonly RandevooDbContext _db;

    public DatabaseAdminUserProfilesApiClient(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<AdminUserProfileListResult> GetProfilesAsync(MockUser admin, AdminUserProfileListFilter filter, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);

        var pageNumber = Math.Max(filter.PageNumber, 1);
        var pageSize = Math.Clamp(filter.PageSize, 10, 100);

        var baseQuery = _db.UserProfiles
            .AsNoTracking()
            .Include(item => item.User)
            .Include(item => item.City)
            .Include(item => item.GenderLookup)
            .Include(item => item.EducationLevelLookup)
            .Include(item => item.ZodiacSignLookup)
            .Include(item => item.Images)
            .Include(item => item.Interests)
            .Where(item => item.User != null);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            baseQuery = baseQuery.Where(item =>
                item.DisplayName.Contains(search)
                || item.User.MobileNumber.Contains(search)
                || (item.User.Email != null && item.User.Email.Contains(search)));
        }

        if (filter.CityId.HasValue)
            baseQuery = baseQuery.Where(item => item.CityId == filter.CityId.Value);
        if (filter.GenderId.HasValue)
            baseQuery = baseQuery.Where(item => item.GenderId == filter.GenderId.Value);
        if (filter.ZodiacSignId.HasValue)
            baseQuery = baseQuery.Where(item => item.ZodiacSignId == filter.ZodiacSignId.Value);
        if (filter.IsActive.HasValue)
            baseQuery = baseQuery.Where(item => item.User.IsActive == filter.IsActive.Value);

        if (filter.IsProfileComplete.HasValue)
        {
            if (filter.IsProfileComplete.Value)
            {
                baseQuery = baseQuery.Where(IsProfileCompleteExpression());
            }
            else
            {
                baseQuery = baseQuery.Where(IsProfileIncompleteExpression());
            }
        }

        var lastActivityQuery = _db.AuditLogs
            .AsNoTracking()
            .Where(log => log.ActorUserId != null)
            .GroupBy(log => log.ActorUserId!.Value)
            .Select(group => new
            {
                UserId = group.Key,
                LastActivityAtUtc = group.Max(item => item.CreatedAt)
            });

        var query =
            from profile in baseQuery
            join lastActivity in lastActivityQuery on profile.UserId equals lastActivity.UserId into lastActivityGroup
            from lastActivity in lastActivityGroup.DefaultIfEmpty()
            select new
            {
                Profile = profile,
                LastActivityAtUtc = lastActivity != null ? lastActivity.LastActivityAtUtc : (DateTime?)null
            };

        query = filter.Sort switch
        {
            "name" => query.OrderBy(item => item.Profile.DisplayName),
            "last-activity" => query.OrderByDescending(item => item.LastActivityAtUtc).ThenByDescending(item => item.Profile.CreatedAt),
            "oldest" => query.OrderBy(item => item.Profile.CreatedAt),
            _ => query.OrderByDescending(item => item.Profile.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var rows = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new
            {
                UserId = item.Profile.UserId,
                ProfileId = item.Profile.Id,
                DisplayName = item.Profile.DisplayName,
                MobileNumber = item.Profile.User.MobileNumber,
                UserRole = item.Profile.User.Role,
                GenderLookupTitle = item.Profile.GenderLookup != null ? item.Profile.GenderLookup.Title : null,
                Gender = item.Profile.Gender,
                CityName = item.Profile.City != null ? item.Profile.City.Name : null,
                LocationCity = item.Profile.Location.City,
                Age = item.Profile.Age,
                ZodiacSignLookupTitle = item.Profile.ZodiacSignLookup != null ? item.Profile.ZodiacSignLookup.Title : null,
                ZodiacSign = item.Profile.ZodiacSign,
                EducationLevelLookupTitle = item.Profile.EducationLevelLookup != null ? item.Profile.EducationLevelLookup.Title : null,
                EducationLevel = item.Profile.EducationLevel,
                IsProfileComplete = item.Profile.EducationLevel != EducationLevel.NotSpecified
                    && item.Profile.Images.Count > 0
                    && item.Profile.Interests.Count > 0
                    && !string.IsNullOrWhiteSpace(item.Profile.Location.City)
                    && !string.IsNullOrWhiteSpace(item.Profile.Location.Country),
                IsActive = item.Profile.User.IsActive,
                CreatedAtUtc = item.Profile.CreatedAt,
                LastActivityAtUtc = item.LastActivityAtUtc
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(item => new AdminUserProfileListItem
        {
            UserId = item.UserId,
            ProfileId = item.ProfileId,
            DisplayName = item.DisplayName,
            MobileNumber = item.MobileNumber,
            RoleTitle = item.UserRole == UserRole.EventPlanner ? "برگزارکننده" : item.UserRole == UserRole.Admin ? "مدیر" : "شرکت‌کننده",
            GenderTitle = item.GenderLookupTitle ?? DisplayFormatter.Gender(item.Gender),
            CityTitle = item.CityName ?? (string.IsNullOrWhiteSpace(item.LocationCity) ? "ثبت نشده" : item.LocationCity),
            Age = item.Age,
            ZodiacSignTitle = item.ZodiacSignLookupTitle ?? item.ZodiacSign,
            EducationLevelTitle = item.EducationLevelLookupTitle ?? item.EducationLevel.ToString(),
            IsProfileComplete = item.IsProfileComplete,
            IsActive = item.IsActive,
            CreatedAtUtc = item.CreatedAtUtc,
            LastActivityAtUtc = item.LastActivityAtUtc
        }).ToList();

        return new AdminUserProfileListResult
        {
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<AdminUserProfileEditor> GetEditorAsync(long userId, MockUser admin, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var user = await QueryUser(userId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("شرکت‌کننده پیدا نشد.");
        var profile = user.Profile ?? throw new InvalidOperationException("پروفایل شرکت‌کننده پیدا نشد.");

        var currentInterestIds = profile.Interests.Select(item => item.Id).ToList();
        var interests = await _db.Interests
            .Where(item => !currentInterestIds.Contains(item.Id))
            .OrderBy(item => item.Name)
            .Select(item => item.Name)
            .ToListAsync(cancellationToken);

        return new AdminUserProfileEditor
        {
            UserId = user.Id,
            ProfileId = profile.Id,
            MobileNumber = user.MobileNumber,
            IsActive = user.IsActive,
            Input = new AdminUserProfileEditorInput
            {
                DisplayName = profile.DisplayName,
                MobileNumber = user.MobileNumber,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                CountryId = profile.CountryId,
                CityId = profile.CityId,
                EducationLevelId = profile.EducationLevelId,
                ZodiacSignId = profile.ZodiacSignId,
                HeightCentimeters = profile.Height.Centimeters,
                Smoking = profile.Smoking,
                IsActive = user.IsActive
            },
            Images = profile.Images
                .OrderByDescending(item => item.IsPrimary)
                .ThenBy(item => item.DisplayOrder)
                .Select(item => new UserProfileImageItem
                {
                    ImageUrl = item.ImageUrl,
                    DisplayOrder = item.DisplayOrder,
                    IsPrimary = item.IsPrimary
                })
                .ToList(),
            Interests = profile.Interests.OrderBy(item => item.Name).Select(item => item.Name).ToList(),
            AvailableInterests = interests
        };
    }

    public async Task SaveProfileAsync(long userId, MockUser admin, AdminUserProfileEditorInput input, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var user = await QueryUser(userId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("شرکت‌کننده پیدا نشد.");
        var profile = user.Profile ?? throw new InvalidOperationException("پروفایل شرکت‌کننده پیدا نشد.");

        var country = input.CountryId is long countryId
            ? await _db.Countries.FirstOrDefaultAsync(item => item.Id == countryId, cancellationToken)
            : null;
        var city = input.CityId is long cityId
            ? await _db.Cities.FirstOrDefaultAsync(item => item.Id == cityId, cancellationToken)
            : null;

        user.UpdateMobileNumber(input.MobileNumber);
        if (input.IsActive)
            user.Activate();
        else
            user.Deactivate();

        var location = new Location(
            country?.Name ?? profile.Location.Country,
            city?.Name ?? profile.Location.City,
            new Coordinates(city?.Latitude ?? profile.Location.Coordinates.Latitude, city?.Longitude ?? profile.Location.Coordinates.Longitude),
            profile.Location.Region);

        profile.UpdateDateOfBirth(input.DateOfBirth);
        profile.UpdateProfile(
            input.DisplayName,
            input.Gender,
            location,
            new Height(input.HeightCentimeters),
            MapEducation(input.EducationLevelId),
            input.Smoking);
        profile.UpdateLookupReferences(input.CountryId, input.CityId, input.EducationLevelId, MapGenderId(input.Gender), input.ZodiacSignId);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddImageAsync(long userId, MockUser admin, AdminUserProfileImageInput input, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var profile = await _db.UserProfiles
            .Include(item => item.Images)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("پروفایل شرکت‌کننده پیدا نشد.");

        var nextOrder = profile.Images.Count == 0 ? 1 : profile.Images.Max(item => item.DisplayOrder) + 1;
        profile.AddImage(input.ImageUrl.Trim(), nextOrder, profile.Images.Count == 0);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveImageAsync(long userId, MockUser admin, string imageUrl, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var image = await _db.UserProfileImages
            .FirstOrDefaultAsync(item => item.UserProfile.UserId == userId && item.ImageUrl == imageUrl, cancellationToken)
            ?? throw new InvalidOperationException("تصویر پیدا نشد.");

        _db.UserProfileImages.Remove(image);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddInterestAsync(long userId, MockUser admin, AdminUserProfileInterestInput input, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var profile = await _db.UserProfiles
            .Include(item => item.Interests)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("پروفایل شرکت‌کننده پیدا نشد.");

        var name = input.InterestName.Trim();
        var interest = await _db.Interests.FirstOrDefaultAsync(item => item.Name == name, cancellationToken);
        if (interest is null)
        {
            interest = new Interest(name, "مدیریتی");
            _db.Interests.Add(interest);
            await _db.SaveChangesAsync(cancellationToken);
        }

        if (!profile.Interests.Any(item => item.Id == interest.Id))
            profile.AddInterest(interest);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveInterestAsync(long userId, MockUser admin, string interestName, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var profile = await _db.UserProfiles
            .Include(item => item.Interests)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("پروفایل شرکت‌کننده پیدا نشد.");

        var interest = profile.Interests.FirstOrDefault(item => item.Name == interestName)
            ?? throw new InvalidOperationException("علاقه پیدا نشد.");
        profile.RemoveInterest(interest);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SendInstantSmsAsync(long userId, MockUser admin, AdminInstantSmsInput input, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var recipient = await _db.Users.FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("شرکت‌کننده پیدا نشد.");
        var adminEvent = await _db.DatingEvents.OrderBy(item => item.Id).FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("برای ثبت پیام فوری حداقل یک رویداد لازم است.");

        _db.SmsQueueItems.Add(new SmsQueueItem(recipient, adminEvent, input.Message.Trim()));
        await _db.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<User> QueryUser(long userId)
    {
        return _db.Users
            .Include(item => item.Profile)!.ThenInclude(profile => profile!.Images)
            .Include(item => item.Profile)!.ThenInclude(profile => profile!.Interests)
            .Where(item => item.Id == userId);
    }

    private static EducationLevel MapEducation(long? educationLevelId) => educationLevelId switch
    {
        1 => EducationLevel.NotSpecified,
        2 => EducationLevel.Diploma,
        3 => EducationLevel.Graduated,
        4 => EducationLevel.Postgraduate,
        5 => EducationLevel.PhD,
        _ => EducationLevel.NotSpecified
    };

    private static long? MapGenderId(Gender gender) => gender switch
    {
        Gender.Unknown => 1,
        Gender.Male => 2,
        Gender.Female => 3,
        _ => null
    };

    private static System.Linq.Expressions.Expression<Func<UserProfile, bool>> IsProfileCompleteExpression()
        => item => item.EducationLevel != EducationLevel.NotSpecified
            && item.Images.Count > 0
            && item.Interests.Count > 0
            && !string.IsNullOrWhiteSpace(item.Location.City)
            && !string.IsNullOrWhiteSpace(item.Location.Country);

    private static System.Linq.Expressions.Expression<Func<UserProfile, bool>> IsProfileIncompleteExpression()
        => item => item.EducationLevel == EducationLevel.NotSpecified
            || item.Images.Count == 0
            || item.Interests.Count == 0
            || string.IsNullOrWhiteSpace(item.Location.City)
            || string.IsNullOrWhiteSpace(item.Location.Country);

    private static void EnsureAdmin(MockUser admin)
    {
        if (admin.Role != AdminRole.Admin)
            throw new InvalidOperationException("این عملیات فقط برای مدیر فعال است.");
    }
}
