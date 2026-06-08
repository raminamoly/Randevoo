using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.UserProfiles;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IAdminUserProfilesApiClient _profilesApi;
    private readonly ILocationsApiClient _locationsApi;
    private readonly CurrentSessionState _session;

    public IndexModel(IAdminUserProfilesApiClient profilesApi, ILocationsApiClient locationsApi, CurrentSessionState session)
    {
        _profilesApi = profilesApi;
        _locationsApi = locationsApi;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? CityId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? GenderId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? ZodiacSignId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsProfileComplete { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "newest";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; } = 25;
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(Result.TotalCount / (double)PageSize));
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search)
        || CityId.HasValue
        || GenderId.HasValue
        || ZodiacSignId.HasValue
        || IsActive.HasValue
        || IsProfileComplete.HasValue
        || !string.Equals(Sort, "newest", StringComparison.OrdinalIgnoreCase);

    public AdminUserProfileListResult Result { get; private set; } = new();
    public SelectList CityOptions { get; private set; } = new(Array.Empty<SelectListItem>());
    public SelectList GenderOptions { get; private set; } = new(Array.Empty<SelectListItem>());
    public SelectList ZodiacSignOptions { get; private set; } = new(Array.Empty<SelectListItem>());

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        await LoadOptionsAsync();
        Result = await _profilesApi.GetProfilesAsync(current, new AdminUserProfileListFilter
        {
            Search = Search,
            CityId = CityId,
            GenderId = GenderId,
            ZodiacSignId = ZodiacSignId,
            IsActive = IsActive,
            IsProfileComplete = IsProfileComplete,
            Sort = Sort,
            PageNumber = PageNumber,
            PageSize = PageSize
        });
    }

    private async Task LoadOptionsAsync()
    {
        var cities = await _locationsApi.GetCitiesAsync();
        var genders = await _locationsApi.GetGendersAsync();
        var zodiacSigns = await _locationsApi.GetZodiacSignsAsync();

        CityOptions = new SelectList(cities, nameof(CityOption.Id), nameof(CityOption.Name), CityId);
        GenderOptions = new SelectList(genders, nameof(GenderOption.Id), nameof(GenderOption.Title), GenderId);
        ZodiacSignOptions = new SelectList(zodiacSigns, nameof(ZodiacSignOption.Id), nameof(ZodiacSignOption.Title), ZodiacSignId);
    }
}
