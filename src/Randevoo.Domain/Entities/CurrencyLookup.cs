using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class CurrencyLookup : BaseEntity, IAggregateRoot
{
    public string Code { get; private set; } = null!;
    public string DisplayNameFa { get; private set; } = null!;
    public string Symbol { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private CurrencyLookup() { }

    public CurrencyLookup(string code, string displayNameFa, string symbol, int displayOrder = 0)
    {
        Code = NormalizeCode(code);
        DisplayNameFa = GuardAgainst.String.InvalidLength(displayNameFa, nameof(displayNameFa), 2, 80);
        Symbol = GuardAgainst.String.InvalidLength(symbol, nameof(symbol), 1, 12);
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<CurrencyLookup>(this));
    }

    public static string NormalizeCode(string? code)
    {
        var normalized = (code ?? string.Empty).Trim().ToUpperInvariant();
        return GuardAgainst.String.InvalidLength(normalized, nameof(code), 3, 3);
    }
}
