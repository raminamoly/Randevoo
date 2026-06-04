using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseLocationsApiClient : ILocationsApiClient
{
    private readonly RandevooDbContext _db;

    public DatabaseLocationsApiClient(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CountryOption>> GetCountriesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Countries
            .Where(country => country.IsActive)
            .OrderBy(country => country.DisplayOrder)
            .ThenBy(country => country.Name)
            .Select(country => new CountryOption
            {
                Id = country.Id,
                Name = country.Name
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CityOption>> GetCitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Cities
            .Where(city => city.IsActive && city.Country.IsActive)
            .OrderBy(city => city.Country.DisplayOrder)
            .ThenBy(city => city.DisplayOrder)
            .ThenBy(city => city.Name)
            .Select(city => new CityOption
            {
                Id = city.Id,
                CountryId = city.CountryId,
                CountryName = city.Country.Name,
                Name = city.Name,
                Latitude = city.Latitude,
                Longitude = city.Longitude
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EducationLevelOption>> GetEducationLevelsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.EducationLevels
            .Where(level => level.IsActive)
            .OrderBy(level => level.DisplayOrder)
            .ThenBy(level => level.Rank)
            .Select(level => new EducationLevelOption
            {
                Id = level.Id,
                Title = level.Title,
                Rank = level.Rank
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GenderOption>> GetGendersAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Genders
            .Where(gender => gender.IsActive)
            .OrderBy(gender => gender.DisplayOrder)
            .Select(gender => new GenderOption
            {
                Id = gender.Id,
                Title = gender.Title
            })
            .ToListAsync(cancellationToken);
    }
}
