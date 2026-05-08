// Domain/ValueObjects/Height.cs
using Randevoo.Domain.Common;

namespace Randevoo.Domain.ValueObjects;

public class Height : BaseValueObject
{
    public int Centimeters { get; private set; }

    private Height() { } // EF Core

    public Height(int centimeters)
    {
        // DEFENSIVE PROGRAMMING with guards
        Centimeters = GuardAgainst.Number.OutOfRange(
            centimeters,
            nameof(centimeters),
            min: 50,
            max: 300);
    }

    public static Height FromCentimeters(int cm)
    {
        return new Height(cm);
    }

    public static Height FromMeters(double meters)
    {
        var cm = (int)Math.Round(meters * 100);
        return new Height(cm);
    }

    public static Height FromFeetInches(int feet, int inches)
    {
        GuardAgainst.Number.Negative(feet, nameof(feet));
        GuardAgainst.Number.OutOfRange(inches, nameof(inches), 0, 11);

        var totalInches = (feet * 12) + inches;
        var cm = (int)(totalInches * 2.54);
        return new Height(cm);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Centimeters;
    }

    public bool IsTallerThan(Height other) => Centimeters > other.Centimeters;
    public bool IsShorterThan(Height other) => Centimeters < other.Centimeters;

    public override string ToString() => $"{Centimeters}cm";
}