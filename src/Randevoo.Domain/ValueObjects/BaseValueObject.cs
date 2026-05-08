// Domain/ValueObjects/BaseValueObject.cs
namespace Randevoo.Domain.ValueObjects;

public abstract class BaseValueObject
{
    // Equality based on all properties
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (BaseValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    protected abstract IEnumerable<object?> GetEqualityComponents();

    public static bool operator ==(BaseValueObject? left, BaseValueObject? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseValueObject? left, BaseValueObject? right)
    {
        return !Equals(left, right);
    }
}