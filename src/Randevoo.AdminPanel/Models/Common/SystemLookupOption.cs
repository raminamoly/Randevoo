namespace Randevoo.AdminPanel.Models.Common;

public sealed class SystemLookupOption
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayNameFa { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal? ExchangeRateToIrr { get; set; }
    public DateTime? ExchangeRateEffectiveFromUtc { get; set; }
    public string Value => Name;
}
