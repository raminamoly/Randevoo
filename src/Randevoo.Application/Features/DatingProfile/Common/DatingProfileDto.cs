using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.DatingProfile.Common;

public record DatingProfileDto(
    long Id,
    long UserId,
    string DisplayName,
    Gender Gender,
    DateOnly DateOfBirth,
    int Age,
    int HeightCm,
    EducationLevel EducationLevel,
    bool Smoking,
    string Country,
    string City,
    string? Region,
    decimal Latitude,
    decimal Longitude,
    IReadOnlyList<string> Interests)
{
    public static DatingProfileDto FromEntity(UserProfile profile)
    {
        return new DatingProfileDto(
            profile.Id,
            profile.UserId,
            profile.DisplayName,
            profile.Gender,
            profile.DateOfBirth,
            profile.Age,
            profile.Height.Centimeters,
            profile.EducationLevel,
            profile.Smoking,
            profile.Location.Country,
            profile.Location.City,
            profile.Location.Region,
            profile.Location.Coordinates.Latitude,
            profile.Location.Coordinates.Longitude,
            profile.Interests.Select(i => i.Name).ToList());
    }
}
