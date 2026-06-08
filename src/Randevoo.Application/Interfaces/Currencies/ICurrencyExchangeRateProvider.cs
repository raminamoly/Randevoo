namespace Randevoo.Application.Interfaces.Currencies;

public interface ICurrencyExchangeRateProvider
{
    Task<CurrencyExchangeRateSnapshot> GetActiveRateToIrrAsync(string currencyCode, DateTime atUtc, CancellationToken cancellationToken = default);
}
