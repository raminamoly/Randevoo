using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseUserProfilesApiClient : IUserProfilesApiClient
{
    private readonly RandevooDbContext _db;

    public DatabaseUserProfilesApiClient(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<UserProfileDetailsViewModel?> GetProfileAsync(long userId, MockUser viewer, CancellationToken cancellationToken = default)
    {
        var profile = await _db.UserProfiles
            .Include(item => item.User)
            .Include(item => item.GenderLookup)
            .Include(item => item.EducationLevelLookup)
            .Include(item => item.ZodiacSignLookup)
            .Include(item => item.Country)
            .Include(item => item.City)
            .Include(item => item.Images)
            .Include(item => item.Interests)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);
        if (profile is null)
            return null;

        var tickets = await _db.EventTickets
            .Include(item => item.DatingEvent)
            .Where(item => item.UserId == userId)
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new UserProfileTicketItem
            {
                EventId = item.DatingEventId,
                EventTitle = item.DatingEvent.Title,
                StartAtUtc = DateTime.SpecifyKind(item.DatingEvent.DateTimeStart, DateTimeKind.Utc),
                Price = item.Price,
                CurrencyCode = item.CurrencyCode,
                IsRefunded = item.IsRefunded,
                IsRemoved = item.IsRemoved
            })
            .ToListAsync(cancellationToken);

        return new UserProfileDetailsViewModel
        {
            UserId = profile.UserId,
            ProfileId = profile.Id,
            DisplayName = profile.DisplayName,
            MobileNumber = viewer.Role == AdminRole.Admin ? profile.User.MobileNumber : null,
            GenderTitle = profile.GenderLookup?.Title ?? DisplayFormatter.Gender(profile.Gender),
            Age = profile.Age,
            BirthMonth = DisplayFormatter.Count(profile.BirthMonth),
            ZodiacSign = profile.ZodiacSignLookup?.Title ?? profile.ZodiacSign,
            EducationLevelTitle = profile.EducationLevelLookup?.Title ?? profile.EducationLevel.ToString(),
            Country = profile.Country?.Name ?? "ثبت نشده",
            City = profile.City?.Name ?? "ثبت نشده",
            Region = profile.Location.Region,
            HeightCentimeters = profile.Height.Centimeters,
            Smoking = profile.Smoking,
            Interests = profile.Interests.OrderBy(item => item.Name).Select(item => item.Name).ToList(),
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
            Tickets = tickets
        };
    }
}
