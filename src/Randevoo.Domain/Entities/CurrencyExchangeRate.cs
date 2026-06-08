using Randevoo.Domain.Common;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class CurrencyExchangeRate : BaseEntity, IAggregateRoot
{
    public string FromCurrencyCode { get; private set; } = null!;
    public string ToCurrencyCode { get; private set; } = "IRR";
    public decimal Rate { get; private set; }
    public DateTime EffectiveFromUtc { get; private set; }
    public DateTime? EffectiveToUtc { get; private set; }
    public string Source { get; private set; } = "Manual";
    public long? CreatedByUserId { get; private set; }

    public bool IsActive => EffectiveToUtc is null;

    private CurrencyExchangeRate() { }

    public CurrencyExchangeRate(
        string fromCurrencyCode,
        string toCurrencyCode,
        decimal rate,
        DateTime effectiveFromUtc,
        string source = "Manual",
        long? createdByUserId = null)
    {
        FromCurrencyCode = CurrencyLookup.NormalizeCode(fromCurrencyCode);
        ToCurrencyCode = CurrencyLookup.NormalizeCode(string.IsNullOrWhiteSpace(toCurrencyCode) ? "IRR" : toCurrencyCode);
        Rate = GuardAgainst.Number.OutOfRange(rate, nameof(rate), 0.000001m, 1_000_000_000_000m);
        EffectiveFromUtc = effectiveFromUtc.Kind == DateTimeKind.Utc
            ? effectiveFromUtc
            : DateTime.SpecifyKind(effectiveFromUtc, DateTimeKind.Utc);
        Source = string.IsNullOrWhiteSpace(source) ? "Manual" : GuardAgainst.String.MaxLength(source.Trim(), nameof(source), 80);
        CreatedByUserId = createdByUserId;

        if (FromCurrencyCode == "IRR" && ToCurrencyCode == "IRR" && Rate != 1m)
            throw new BusinessRuleViolationException("Invalid IRR rate", "IRR to IRR exchange rate must be 1.");

        AddDomainEvent(new EntityCreatedEvent<CurrencyExchangeRate>(this));
    }

    public void Close(DateTime effectiveToUtc)
    {
        var normalized = effectiveToUtc.Kind == DateTimeKind.Utc
            ? effectiveToUtc
            : DateTime.SpecifyKind(effectiveToUtc, DateTimeKind.Utc);

        if (normalized <= EffectiveFromUtc)
            throw new BusinessRuleViolationException("Invalid exchange rate period", "Exchange rate end date must be after the start date.");

        EffectiveToUtc = normalized;
        UpdateTimestamp();
    }

    public bool Covers(DateTime atUtc)
    {
        var normalized = atUtc.Kind == DateTimeKind.Utc
            ? atUtc
            : DateTime.SpecifyKind(atUtc, DateTimeKind.Utc);

        return EffectiveFromUtc <= normalized && (EffectiveToUtc is null || EffectiveToUtc > normalized);
    }
}
