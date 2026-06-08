namespace Randevoo.Application.Interfaces.Currencies;

public sealed record CurrencyExchangeRateSnapshot(
    long ExchangeRateId,
    string FromCurrencyCode,
    string ToCurrencyCode,
    decimal Rate,
    DateTime CapturedAtUtc);
