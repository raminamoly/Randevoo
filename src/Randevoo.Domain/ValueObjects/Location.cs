// Domain/ValueObjects/Location.cs
using Randevoo.Domain.Common;

namespace Randevoo.Domain.ValueObjects;

public class Location : BaseValueObject
{
    public string Country { get; private set; }
    public string City { get; private set; }
    public string? Region { get; private set; }

    private Location() { } // EF Core

    public Location(string country, string city, string? region = null)
    {
        // DEFENSIVE PROGRAMMING
        Country = GuardAgainst.String.InvalidLength(
            country,
            nameof(country),
            min: 2,
            max: 100);

        City = GuardAgainst.String.InvalidLength(
            city,
            nameof(city),
            min: 2,
            max: 100);

        if (region != null)
        {
            Region = GuardAgainst.String.InvalidLength(
                region,
                nameof(region),
                min: 0,
                max: 100);
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Country;
        yield return City;
        yield return Region;
    }

    public string GetFullAddress()
    {
        return Region == null
            ? $"{City}, {Country}"
            : $"{City}, {Region}, {Country}";
    }

    public override string ToString() => GetFullAddress();
}