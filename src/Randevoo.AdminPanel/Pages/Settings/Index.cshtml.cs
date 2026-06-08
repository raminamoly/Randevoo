using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Pages.Settings;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly RandevooDbContext _db;
    private readonly CurrentSessionState _session;

    public IndexModel(RandevooDbContext db, CurrentSessionState session)
    {
        _db = db;
        _session = session;
    }

    [BindProperty]
    public CurrencyRateInput RateInput { get; set; } = new();

    public SelectList CurrencyOptions { get; private set; } = new(Array.Empty<object>());

    public IReadOnlyList<CurrencyRateRow> ActiveRates { get; private set; } = Array.Empty<CurrencyRateRow>();

    public IReadOnlyList<CurrencyRateRow> RateHistory { get; private set; } = Array.Empty<CurrencyRateRow>();

    public bool IsRtl => _session.IsRtl;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostSaveRateAsync()
    {
        await LoadAsync();
        var currencyCode = CurrencyLookup.NormalizeCode(RateInput.CurrencyCode);
        var currencyExists = await _db.Currencies.AnyAsync(item => item.Code == currencyCode && item.IsActive);
        if (!currencyExists)
            ModelState.AddModelError(nameof(RateInput.CurrencyCode), "ارز انتخاب شده معتبر نیست.");

        if (RateInput.Rate <= 0)
            ModelState.AddModelError(nameof(RateInput.Rate), "نرخ تبدیل باید بزرگتر از صفر باشد.");

        if (currencyCode == "IRR" && RateInput.Rate != 1m)
            ModelState.AddModelError(nameof(RateInput.Rate), "نرخ ریال به ریال باید دقیقاً ۱ باشد.");

        if (!ModelState.IsValid)
            return Page();

        var nowUtc = DateTime.UtcNow;
        var activeRates = await _db.CurrencyExchangeRates
            .Where(item => item.FromCurrencyCode == currencyCode
                && item.ToCurrencyCode == "IRR"
                && item.EffectiveToUtc == null)
            .ToListAsync();

        foreach (var activeRate in activeRates)
        {
            activeRate.Close(nowUtc);
        }

        _db.CurrencyExchangeRates.Add(new CurrencyExchangeRate(
            currencyCode,
            "IRR",
            RateInput.Rate,
            nowUtc,
            "AdminPanel",
            _session.CurrentUser?.Id));

        await _db.SaveChangesAsync();
        StatusMessage = "نرخ تبدیل جدید ذخیره شد و نرخ قبلی به تاریخچه منتقل شد.";
        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var currencies = await _db.Currencies
            .Where(item => item.IsActive)
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Code)
            .Select(item => new
            {
                item.Code,
                Title = $"{item.DisplayNameFa} ({item.Code})"
            })
            .ToListAsync();

        CurrencyOptions = new SelectList(currencies, "Code", "Title", string.IsNullOrWhiteSpace(RateInput.CurrencyCode) ? "IRR" : RateInput.CurrencyCode);

        var rows = await (
                from rate in _db.CurrencyExchangeRates.AsNoTracking()
                join currency in _db.Currencies.AsNoTracking() on rate.FromCurrencyCode equals currency.Code
                where rate.ToCurrencyCode == "IRR"
                orderby rate.EffectiveFromUtc descending
                select new CurrencyRateRow
                {
                    Id = rate.Id,
                    CurrencyCode = rate.FromCurrencyCode,
                    CurrencyTitle = currency.DisplayNameFa,
                    CurrencySymbol = currency.Symbol,
                    Rate = rate.Rate,
                    EffectiveFromUtc = rate.EffectiveFromUtc,
                    EffectiveToUtc = rate.EffectiveToUtc,
                    Source = rate.Source
                })
            .ToListAsync();

        ActiveRates = rows
            .Where(item => item.EffectiveToUtc is null)
            .OrderBy(item => currencies.FindIndex(currency => currency.Code == item.CurrencyCode))
            .ToList();

        RateHistory = rows
            .OrderByDescending(item => item.EffectiveFromUtc)
            .Take(80)
            .ToList();
    }

    public sealed class CurrencyRateInput
    {
        [Required(ErrorMessage = "ارز را انتخاب کنید.")]
        public string CurrencyCode { get; set; } = "IRR";

        [Range(typeof(decimal), "0.000001", "1000000000000", ErrorMessage = "نرخ تبدیل معتبر نیست.")]
        public decimal Rate { get; set; } = 1m;
    }

    public sealed class CurrencyRateRow
    {
        public long Id { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencyTitle { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public DateTime EffectiveFromUtc { get; set; }
        public DateTime? EffectiveToUtc { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
