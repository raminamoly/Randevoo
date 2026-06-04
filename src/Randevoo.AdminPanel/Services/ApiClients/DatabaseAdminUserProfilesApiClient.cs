using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;
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

    public async Task<AdminUserProfileEditor> GetEditorAsync(long userId, MockUser admin, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var user = await QueryUser(userId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("کاربر پیدا نشد.");
        var profile = user.Profile ?? throw new InvalidOperationException("پروفایل کاربر پیدا نشد.");

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
            ?? throw new InvalidOperationException("کاربر پیدا نشد.");
        var profile = user.Profile ?? throw new InvalidOperationException("پروفایل کاربر پیدا نشد.");

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
        profile.UpdateLookupReferences(input.CountryId, input.CityId, input.EducationLevelId, MapGenderId(input.Gender));

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddImageAsync(long userId, MockUser admin, AdminUserProfileImageInput input, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var profile = await _db.UserProfiles
            .Include(item => item.Images)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("پروفایل کاربر پیدا نشد.");

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
            ?? throw new InvalidOperationException("پروفایل کاربر پیدا نشد.");

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
            ?? throw new InvalidOperationException("پروفایل کاربر پیدا نشد.");

        var interest = profile.Interests.FirstOrDefault(item => item.Name == interestName)
            ?? throw new InvalidOperationException("علاقه پیدا نشد.");
        profile.RemoveInterest(interest);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task SendInstantSmsAsync(long userId, MockUser admin, AdminInstantSmsInput input, CancellationToken cancellationToken = default)
    {
        EnsureAdmin(admin);
        var recipient = await _db.Users.FirstOrDefaultAsync(item => item.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException("کاربر پیدا نشد.");
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

    private static void EnsureAdmin(MockUser admin)
    {
        if (admin.Role != AdminRole.Admin)
            throw new InvalidOperationException("این عملیات فقط برای مدیر فعال است.");
    }
}
