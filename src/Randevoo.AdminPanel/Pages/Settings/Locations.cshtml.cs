using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.Domain.Entities;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Pages.Settings;

[Authorize(Policy = Policies.AdminOnly)]
public class LocationsModel : PageModel
{
    private readonly RandevooDbContext _db;

    public LocationsModel(RandevooDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? CountryId { get; set; }

    [BindProperty]
    public CountryFormInput CountryInput { get; set; } = new();

    [BindProperty]
    public CityFormInput CityInput { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public IReadOnlyList<CountryRow> Countries { get; private set; } = [];
    public IReadOnlyList<CityRow> Cities { get; private set; } = [];
    public SelectList CountryOptions { get; private set; } = new(Array.Empty<object>());

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostSaveCountryAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var code = CountryInput.Code.Trim().ToUpperInvariant();
        var name = CountryInput.Name.Trim();
        var duplicateExists = await _db.Countries.AnyAsync(
            item => item.Id != CountryInput.Id && (item.Name == name || item.Code == code),
            cancellationToken);
        if (duplicateExists)
        {
            ModelState.AddModelError(string.Empty, "کشور دیگری با همین نام یا کد ثبت شده است.");
            await LoadAsync(cancellationToken);
            return Page();
        }

        if (CountryInput.Id is > 0)
        {
            var country = await _db.Countries.FirstOrDefaultAsync(item => item.Id == CountryInput.Id, cancellationToken);
            if (country is null)
                return NotFound();

            country.Update(name, code, CountryInput.DisplayOrder, CountryInput.IsActive);
        }
        else
        {
            var country = new Country(name, code, CountryInput.DisplayOrder);
            country.SetActive(CountryInput.IsActive);
            _db.Countries.Add(country);
        }

        await _db.SaveChangesAsync(cancellationToken);
        StatusMessage = "کشور ذخیره شد.";
        return RedirectToPage(new { CountryId = CountryInput.Id > 0 ? CountryInput.Id : (long?)null, Search });
    }

    public async Task<IActionResult> OnPostToggleCountryAsync(long id, CancellationToken cancellationToken)
    {
        var country = await _db.Countries.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (country is null)
            return NotFound();

        country.SetActive(!country.IsActive);
        await _db.SaveChangesAsync(cancellationToken);
        StatusMessage = country.IsActive ? "کشور فعال شد." : "کشور غیرفعال شد.";
        return RedirectToPage(new { CountryId = id, Search });
    }

    public async Task<IActionResult> OnPostSaveCityAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var country = await _db.Countries.FirstOrDefaultAsync(item => item.Id == CityInput.CountryId, cancellationToken);
        if (country is null)
        {
            ModelState.AddModelError(nameof(CityInput.CountryId), "کشور انتخاب شده معتبر نیست.");
            await LoadAsync(cancellationToken);
            return Page();
        }

        var name = CityInput.Name.Trim();
        var duplicateExists = await _db.Cities.AnyAsync(
            item => item.Id != CityInput.Id && item.CountryId == CityInput.CountryId && item.Name == name,
            cancellationToken);
        if (duplicateExists)
        {
            ModelState.AddModelError(string.Empty, "این شهر برای کشور انتخاب شده قبلاً ثبت شده است.");
            await LoadAsync(cancellationToken);
            return Page();
        }

        if (CityInput.Id is > 0)
        {
            var city = await _db.Cities.FirstOrDefaultAsync(item => item.Id == CityInput.Id, cancellationToken);
            if (city is null)
                return NotFound();

            city.Update(country, name, CityInput.Latitude, CityInput.Longitude, CityInput.DisplayOrder, CityInput.IsActive);
        }
        else
        {
            var city = new City(country, name, CityInput.Latitude, CityInput.Longitude, CityInput.DisplayOrder);
            city.SetActive(CityInput.IsActive);
            _db.Cities.Add(city);
        }

        await _db.SaveChangesAsync(cancellationToken);
        StatusMessage = "شهر ذخیره شد.";
        return RedirectToPage(new { CountryId = CityInput.CountryId, Search });
    }

    public async Task<IActionResult> OnPostToggleCityAsync(long id, CancellationToken cancellationToken)
    {
        var city = await _db.Cities.Include(item => item.Country).FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (city is null)
            return NotFound();

        city.SetActive(!city.IsActive);
        await _db.SaveChangesAsync(cancellationToken);
        StatusMessage = city.IsActive ? "شهر فعال شد." : "شهر غیرفعال شد.";
        return RedirectToPage(new { CountryId = city.CountryId, Search });
    }

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var countriesQuery = _db.Countries.AsNoTracking();
        var citiesQuery = _db.Cities.AsNoTracking().Include(item => item.Country).AsQueryable();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();
            countriesQuery = countriesQuery.Where(item => item.Name.Contains(search) || item.Code.Contains(search));
            citiesQuery = citiesQuery.Where(item => item.Name.Contains(search) || item.Country.Name.Contains(search));
        }

        var countries = await countriesQuery
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Name)
            .ToListAsync(cancellationToken);
        var cityCounts = await _db.Cities
            .AsNoTracking()
            .GroupBy(item => item.CountryId)
            .Select(group => new { CountryId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.CountryId, item => item.Count, cancellationToken);

        Countries = countries
            .Select(item => new CountryRow(item.Id, item.Name, item.Code, item.IsActive, item.DisplayOrder, cityCounts.GetValueOrDefault(item.Id)))
            .ToList();

        if (CountryId is null && Countries.Count > 0)
            CountryId = Countries[0].Id;

        var countryOptions = await _db.Countries
            .AsNoTracking()
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Name)
            .Select(item => new { item.Id, Title = item.IsActive ? item.Name : item.Name + " (غیرفعال)" })
            .ToListAsync(cancellationToken);

        CountryOptions = new SelectList(countryOptions, "Id", "Title", CountryId);

        Cities = await citiesQuery
            .Where(item => CountryId == null || item.CountryId == CountryId)
            .OrderBy(item => item.Country.DisplayOrder)
            .ThenBy(item => item.DisplayOrder)
            .ThenBy(item => item.Name)
            .Select(item => new CityRow(
                item.Id,
                item.CountryId,
                item.Country.Name,
                item.Name,
                item.IsActive,
                item.Country.IsActive,
                item.DisplayOrder,
                item.Latitude,
                item.Longitude))
            .ToListAsync(cancellationToken);

        CountryInput = new CountryFormInput { IsActive = true };
        CityInput = new CityFormInput
        {
            CountryId = CountryId ?? countryOptions.FirstOrDefault()?.Id ?? 0,
            IsActive = true,
            Latitude = 35.6892m,
            Longitude = 51.3890m
        };
    }

    public sealed class CountryFormInput
    {
        public long? Id { get; set; }

        [Required(ErrorMessage = "نام کشور را وارد کنید.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "نام کشور باید بین ۲ تا ۱۰۰ کاراکتر باشد.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "کد کشور را وارد کنید.")]
        [StringLength(10, MinimumLength = 2, ErrorMessage = "کد کشور باید بین ۲ تا ۱۰ کاراکتر باشد.")]
        public string Code { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public sealed class CityFormInput
    {
        public long? Id { get; set; }

        [Range(1, long.MaxValue, ErrorMessage = "کشور را انتخاب کنید.")]
        public long CountryId { get; set; }

        [Required(ErrorMessage = "نام شهر را وارد کنید.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "نام شهر باید بین ۲ تا ۱۰۰ کاراکتر باشد.")]
        public string Name { get; set; } = string.Empty;

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        [Range(typeof(decimal), "-90", "90", ErrorMessage = "عرض جغرافیایی معتبر نیست.")]
        public decimal Latitude { get; set; }

        [Range(typeof(decimal), "-180", "180", ErrorMessage = "طول جغرافیایی معتبر نیست.")]
        public decimal Longitude { get; set; }
    }

    public sealed record CountryRow(long Id, string Name, string Code, bool IsActive, int DisplayOrder, int CityCount);
    public sealed record CityRow(long Id, long CountryId, string CountryName, string Name, bool IsActive, bool CountryIsActive, int DisplayOrder, decimal Latitude, decimal Longitude);
}
