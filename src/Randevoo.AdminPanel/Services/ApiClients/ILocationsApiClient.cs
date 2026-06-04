using Randevoo.AdminPanel.Models.Common;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface ILocationsApiClient
{
    Task<IReadOnlyList<CountryOption>> GetCountriesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CityOption>> GetCitiesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EducationLevelOption>> GetEducationLevelsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GenderOption>> GetGendersAsync(CancellationToken cancellationToken = default);
}
