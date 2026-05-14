// Domain/Common/GuardAgainst.cs
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Common;

public static class GuardAgainst
{
    public static class String
    {
        public static string NullOrWhiteSpace(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleViolationException(
                    "Invalid input",
                    $"{parameterName} cannot be null or whitespace");
            return value;
        }

        public static string InvalidLength(string value, string parameterName, int min, int max)
        {
            NullOrWhiteSpace(value, parameterName);
            if (value.Length < min || value.Length > max)
                throw new BusinessRuleViolationException(
                    "Invalid length",
                    $"{parameterName} must be between {min} and {max} characters. Got: {value.Length}");
            return value;
        }

        public static string InvalidEmail(string value, string parameterName)
        {
            NullOrWhiteSpace(value, parameterName);
            if (!value.Contains('@') || !value.Contains('.') || value.Length < 5)
                throw new BusinessRuleViolationException(
                    "Invalid email format",
                    $"{parameterName} is not a valid email address");
            return value;
        }

        public static string MaxLength(string value, string parameterName, int maxLength)
        {
            NullOrWhiteSpace(value, parameterName);
            if (value.Length > maxLength)
                throw new BusinessRuleViolationException(
                    "Maximum length exceeded",
                    $"{parameterName} cannot exceed {maxLength} characters. Got: {value.Length}");
            return value;
        }

        public static string MinLength(string value, string parameterName, int minLength)
        {
            NullOrWhiteSpace(value, parameterName);
            if (value.Length < minLength)
                throw new BusinessRuleViolationException(
                    "Minimum length not met",
                    $"{parameterName} must be at least {minLength} characters. Got: {value.Length}");
            return value;
        }
    }

    public static class Number
    {
        public static int OutOfRange(int value, string parameterName, int min, int max)
        {
            if (value < min || value > max)
                throw new BusinessRuleViolationException(
                    "Value out of range",
                    $"{parameterName} must be between {min} and {max}. Got: {value}");
            return value;
        }

        public static decimal OutOfRange(decimal value, string parameterName, decimal min, decimal max)
        {
            if (value < min || value > max)
                throw new BusinessRuleViolationException(
                    "Value out of range",
                    $"{parameterName} must be between {min} and {max}. Got: {value}");
            return value;
        }

        public static int Negative(int value, string parameterName)
        {
            if (value < 0)
                throw new BusinessRuleViolationException(
                    "Negative value not allowed",
                    $"{parameterName} cannot be negative. Got: {value}");
            return value;
        }

        public static int Positive(int value, string parameterName)
        {
            if (value <= 0)
                throw new BusinessRuleViolationException(
                    "Non-positive value not allowed",
                    $"{parameterName} must be positive. Got: {value}");
            return value;
        }

        public static double OutOfRange(double value, string parameterName, double min, double max)
        {
            if (value < min || value > max)
                throw new BusinessRuleViolationException(
                    "Value out of range",
                    $"{parameterName} must be between {min} and {max}. Got: {value}");
            return value;
        }

        // ENUMS - CRITICAL for defensive programming!
        public static TEnum AgainstInvalidEnum<TEnum>(int value, string parameterName) where TEnum : Enum
        {
            if (!Enum.IsDefined(typeof(TEnum), value))
                throw new InvalidEntityStateException(
                    $"{parameterName} has invalid value {value}. Valid values: {string.Join(", ", Enum.GetNames(typeof(TEnum)))}");

            return (TEnum)(object)value;
        }

        public static TEnum AgainstInvalidEnum<TEnum>(string value, string parameterName) where TEnum : Enum
        {
            if (!Enum.TryParse(typeof(TEnum), value, true, out var result))
                throw new InvalidEntityStateException(
                    $"{parameterName} has invalid value '{value}'. Valid values: {string.Join(", ", Enum.GetNames(typeof(TEnum)))}");

            return (TEnum)result;
        }
    }

    public static class Object
    {
        public static T Null<T>(T value, string parameterName) where T : class
        {
            if (value == null)
                throw new BusinessRuleViolationException(
                    "Null value not allowed",
                    $"{parameterName} cannot be null");
            return value;
        }

        public static T Nullable<T>(T? value, string parameterName) where T : struct
        {
            if (!value.HasValue)
                throw new BusinessRuleViolationException(
                    "Null value not allowed",
                    $"{parameterName} cannot be null");
            return value.Value;
        }
    }

    public static class Collection
    {
        public static ICollection<T> Empty<T>(ICollection<T> collection, string parameterName)
        {
            if (collection == null || collection.Count == 0)
                throw new BusinessRuleViolationException(
                    "Empty collection not allowed",
                    $"{parameterName} cannot be empty");
            return collection;
        }

        public static ICollection<T> MaxSize<T>(ICollection<T> collection, string parameterName, int maxSize)
        {
            if (collection.Count > maxSize)
                throw new BusinessRuleViolationException(
                    "Collection size exceeded",
                    $"{parameterName} cannot exceed {maxSize} items. Got: {collection.Count}");
            return collection;
        }

        public static ICollection<T> MinSize<T>(ICollection<T> collection, string parameterName, int minSize)
        {
            if (collection.Count < minSize)
                throw new BusinessRuleViolationException(
                    "Insufficient items",
                    $"{parameterName} must have at least {minSize} items. Got: {collection.Count}");
            return collection;
        }
    }

    public static class Date
    {
        public static DateTime Future(DateTime date, string parameterName)
        {
            if (date > DateTime.UtcNow)
                throw new BusinessRuleViolationException(
                    "Future date not allowed",
                    $"{parameterName} cannot be in the future");
            return date;
        }

        public static DateOnly Future(DateOnly date, string parameterName)
        {
            if (date > DateOnly.FromDateTime(DateTime.UtcNow))
                throw new BusinessRuleViolationException(
                    "Future date not allowed",
                    $"{parameterName} cannot be in the future");
            return date;
        }

        public static DateOnly AgeRequirement(DateOnly birthDate, int minAge, string parameterName)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age)) age--;

            if (age < minAge)
                throw new BusinessRuleViolationException(
                    "Age requirement not met",
                    $"{parameterName}: Must be at least {minAge} years old. Current age: {age}");

            return birthDate;
        }

        public static DateOnly Past(DateOnly date, string parameterName)
        {
            if (date < DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-100)))
                throw new BusinessRuleViolationException(
                    "Date too old",
                    $"{parameterName} cannot be more than 100 years in the past");
            return date;
        }
    }

    public static class Entity
    {
        public static T NotFound<T>(T? entity, string entityName, object id) where T : class
        {
            if (entity == null)
                throw new NotFoundException(entityName, id);
            return entity;
        }

        public static T AlreadyExists<T>(T? entity, string entityName, string identifier) where T : class
        {
            if (entity != null)
                throw new BusinessRuleViolationException(
                    "Duplicate entity",
                    $"{entityName} with identifier '{identifier}' already exists");
            return entity;
        }
    }
}