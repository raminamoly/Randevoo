using Randevoo.AdminPanel.Models.Common;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface ILocationsApiClient
{
    Task<IReadOnlyList<CountryOption>> GetCountriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CityOption>> GetCitiesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EducationLevelOption>> GetEducationLevelsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GenderOption>> GetGendersAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ZodiacSignOption>> GetZodiacSignsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SystemLookupOption>> GetUserRolesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SystemLookupOption>> GetReviewStatusesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SystemLookupOption>> GetDiscountTypesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SystemLookupOption>> GetBalanceTransactionTypesAsync(CancellationToken cancellationToken = default);
}
