using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class City : BaseEntity, IAggregateRoot
{
    public long CountryId { get; private set; }
    public Country Country { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    private City() { }

    public City(Country country, string name, decimal latitude, decimal longitude, int displayOrder = 0)
    {
        Country = GuardAgainst.Object.Null(country, nameof(country));
        CountryId = country.Id;
        Name = GuardAgainst.String.InvalidLength(name, nameof(name), 2, 100);
        Latitude = latitude;
        Longitude = longitude;
        DisplayOrder = displayOrder;
        IsActive = true;
        AddDomainEvent(new EntityCreatedEvent<City>(this));
    }
}
