using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class Country : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string Code { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private readonly List<City> _cities = new();
    public IReadOnlyList<City> Cities => _cities.AsReadOnly();

    private Country() { }

    public Country(string name, string code, int displayOrder = 0)
    {
        Name = GuardAgainst.String.InvalidLength(name, nameof(name), 2, 100);
        Code = GuardAgainst.String.InvalidLength(code, nameof(code), 2, 10).ToUpperInvariant();
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<Country>(this));
    }
}
