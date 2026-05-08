// Domain/ValueObjects/Interest.cs
using Randevoo.Domain.Common;

namespace Randevoo.Domain.ValueObjects;

public class Interest : BaseValueObject
{
    public string Name { get; private set; }
    public string? Category { get; private set; }

    private Interest() { } // EF Core

    public Interest(string name, string? category = null)
    {
        // DEFENSIVE PROGRAMMING
        Name = GuardAgainst.String.InvalidLength(
            name,
            nameof(name),
            min: 2,
            max: 50);

        if (category != null)
        {
            Category = GuardAgainst.String.InvalidLength(
                category,
                nameof(category),
                min: 0,
                max: 30);
        }
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
        yield return Category;
    }

    // Factory methods for common interests
    public static class Presets
    {
        public static Interest Hiking() => new("Hiking", "Outdoor");
        public static Interest Photography() => new("Photography", "Arts");
        public static Interest Coffee() => new("Coffee", "Food");
        public static Interest Gym() => new("Gym", "Fitness");
        public static Interest Reading() => new("Reading", "Education");
        public static Interest Travel() => new("Travel", "Lifestyle");
    }

    public override string ToString() => Name;
}