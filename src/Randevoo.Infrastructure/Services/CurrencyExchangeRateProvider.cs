using Microsoft.EntityFrameworkCore;
using Randevoo.Application.Interfaces.Currencies;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Services;

public sealed class CurrencyExchangeRateProvider : ICurrencyExchangeRateProvider
{
    private readonly RandevooDbContext _db;

    public CurrencyExchangeRateProvider(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<CurrencyExchangeRateSnapshot> GetActiveRateToIrrAsync(string currencyCode, DateTime atUtc, CancellationToken cancellationToken = default)
    {
        var normalizedCurrency = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode);
        var normalizedAt = atUtc.Kind == DateTimeKind.Utc
            ? atUtc
            : DateTime.SpecifyKind(atUtc, DateTimeKind.Utc);

        var rate = await _db.CurrencyExchangeRates
            .Where(item => item.FromCurrencyCode == normalizedCurrency
                && item.ToCurrencyCode == "IRR"
                && item.EffectiveFromUtc <= normalizedAt
                && (item.EffectiveToUtc == null || item.EffectiveToUtc > normalizedAt))
            .OrderByDescending(item => item.EffectiveFromUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (rate is null)
            throw new BusinessRuleViolationException("Exchange rate missing", $"No active exchange rate to IRR exists for {normalizedCurrency}.");

        return new CurrencyExchangeRateSnapshot(rate.Id, rate.FromCurrencyCode, rate.ToCurrencyCode, rate.Rate, normalizedAt);
    }
}
