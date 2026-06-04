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
            profile.Country?.Name ?? LookupCountryName(profile.CountryId) ?? profile.Location.Country,
            profile.City?.Name ?? LookupCityName(profile.CityId) ?? profile.Location.City,
            profile.Location.Region,
            profile.Location.Coordinates.Latitude,
            profile.Location.Coordinates.Longitude,
            profile.Interests.Select(i => i.Name).ToList());
    }

    private static string? LookupCountryName(long? countryId) => countryId switch
    {
        1 => "Iran",
        2 => "United Arab Emirates",
        3 => "Turkey",
        _ => null
    };

    private static string? LookupCityName(long? cityId) => cityId switch
    {
        1 => "Tehran",
        2 => "Mashhad",
        3 => "Shiraz",
        4 => "Isfahan",
        5 => "Tabriz",
        6 => "Dubai",
        7 => "Abu Dhabi",
        8 => "Istanbul",
        9 => "Ankara",
        _ => null
    };
}
