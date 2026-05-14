// Domain/ValueObjects/AgeRange.cs
using Randevoo.Domain.Common;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.ValueObjects;

public class AgeRange : BaseValueObject
{
    public int Min { get; private set; }
    public int Max { get; private set; }

    public AgeRange(int min, int max)
    {
        Min = GuardAgainst.Number.OutOfRange(min, nameof(min), 18, 100);
        Max = GuardAgainst.Number.OutOfRange(max, nameof(max), 18, 100);

        if (min > max)
            throw new BusinessRuleViolationException("Range Problem", "Min age cannot be greater than max age");
    }

    public bool IsWithinRange(int age)
    {
        return age >= Min && age <= Max;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Min;
        yield return Max;
    }
}