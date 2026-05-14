// Domain/ValueObjects/Coordinates.cs
using Randevoo.Domain.Common;

namespace Randevoo.Domain.ValueObjects;

public class Coordinates : BaseValueObject
{
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }

    public Coordinates(decimal latitude, decimal longitude)
    {
        Latitude = GuardAgainst.Number.OutOfRange(latitude, nameof(latitude), -90, 90);
        Longitude = GuardAgainst.Number.OutOfRange(longitude, nameof(longitude), -180, 180);
    }

    public double DistanceTo(Coordinates other)
    {
        var lat1 = (double)Latitude;
        var lon1 = (double)Longitude;
        var lat2 = (double)other.Latitude;
        var lon2 = (double)other.Longitude;

        var R = 6371; // Earth radius in kilometers

        var dLat = (lat2 - lat1).ToRadians();
        var dLon = (lon2 - lon1).ToRadians();

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1.ToRadians()) * Math.Cos(lat2.ToRadians()) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Distance in kilometers
    }

    // Helper method to get distance in miles
    public double DistanceToInMiles(Coordinates other)
    {
        return DistanceTo(other) * 0.621371;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }

    public override string ToString()
    {
        return $"{Latitude:F6}, {Longitude:F6}";
    }
}