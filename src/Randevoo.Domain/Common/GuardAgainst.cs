// Domain/Common/GuardAgains.cs
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Common;

public static class GuardAgainst
{
    public static class String
    {
        public static string NullOrWhiteSpace(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"{parameterName} cannot be null or whitespace");
            return value;
        }

        public static string InvalidLength(string value, string parameterName, int min, int max)
        {
            NullOrWhiteSpace(value, parameterName);
            if (value.Length < min || value.Length > max)
                throw new DomainException($"{parameterName} must be {min}-{max} chars. Got: {value.Length}");
            return value;
        }

        public static string InvalidEmail(string value, string parameterName)
        {
            NullOrWhiteSpace(value, parameterName);
            if (!value.Contains('@') || value.Length < 5)
                throw new DomainException($"{parameterName} is not a valid email");
            return value;
        }
    }

    public static class Number
    {
        public static int OutOfRange(int value, string parameterName, int min, int max)
        {
            if (value < min || value > max)
                throw new DomainException($"{parameterName} must be {min}-{max}. Got: {value}");
            return value;
        }

        public static int Negative(int value, string parameterName)
        {
            if (value < 0)
                throw new DomainException($"{parameterName} cannot be negative. Got: {value}");
            return value;
        }

        public static double OutOfRange(double value, string parameterName, double min, double max)
        {
            if (value < min || value > max)
                throw new DomainException($"{parameterName} must be {min}-{max}. Got: {value}");
            return value;
        }


        // ENUMS - CRITICAL for defensive programming!
        public static TEnum AgainstInvalidEnum<TEnum>(int value, string parameterName) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new DomainException($"{parameterName} has invalid value {value}. Valid values: {string.Join(", ", Enum.GetNames(typeof(TEnum)))}");

            return (TEnum)(object)value;
        }

        public static TEnum AgainstInvalidEnum<TEnum>(string value, string parameterName) where TEnum : Enum
        {
            if (!Enum.TryParse(typeof(TEnum), value, true, out var result))
                throw new DomainException($"{parameterName} has invalid value '{value}'. Valid values: {string.Join(", ", Enum.GetNames(typeof(TEnum)))}");

            return (TEnum)result;
        }

    }
    public static class Object
    {
        public static T Null<T>(T value, string parameterName) where T : class
        {
            if (value == null)
                throw new DomainException($"{parameterName} cannot be null");
            return value;
        }

        public static T Nullable<T>(T? value, string parameterName) where T : struct
        {
            if (!value.HasValue)
                throw new DomainException($"{parameterName} cannot be null");
            return value.Value;
        }
    }

    public static class Collection
    {
        public static ICollection<T> Empty<T>(ICollection<T> collection, string parameterName)
        {
            if (collection == null || collection.Count == 0)
                throw new DomainException($"{parameterName} cannot be empty");
            return collection;
        }

        public static ICollection<T> MaxSize<T>(ICollection<T> collection, string parameterName, int maxSize)
        {
            if (collection.Count > maxSize)
                throw new DomainException($"{parameterName} cannot exceed {maxSize} items. Got: {collection.Count}");
            return collection;
        }
    }

    public static class Date
    {
        public static DateTime Future(DateTime date, string parameterName)
        {
            if (date > DateTime.UtcNow)
                throw new DomainException($"{parameterName} cannot be in the future");
            return date;
        }

        public static DateOnly Future(DateOnly date, string parameterName)
        {
            if (date > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new DomainException($"{parameterName} cannot be in the future");
            return date;
        }

        public static DateOnly AgeRequirement(DateOnly birthDate, int minAge, string parameterName)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;

            if (age < minAge)
                throw new DomainException($"{parameterName}: Must be at least {minAge} years old. Current age: {age}");

            return birthDate;
        }
    }
}