using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventDiscountCode : BaseEntity
{
    public long? DatingEventId { get; private set; }
    public DatingEvent? DatingEvent { get; private set; }
    public string Code { get; private set; } = null!;
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public EventDiscountGenderScope GenderScope { get; private set; }
    public EventDiscountType DiscountType { get; private set; }
    public decimal Value { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime EndsAtUtc { get; private set; }
    public int MaxUsageCount { get; private set; }
    public int UsedCount { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime? LastUsedAtUtc { get; private set; }

    private EventDiscountCode() { }

    public EventDiscountCode(
        DatingEvent? datingEvent,
        string code,
        EventDiscountGenderScope genderScope,
        EventDiscountType discountType,
        decimal value,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        int maxUsageCount,
        bool isActive,
        string? title = null,
        string? description = null)
    {
        DatingEvent = datingEvent;
        DatingEventId = datingEvent?.Id;
        UpdateDetails(code, genderScope, discountType, value, startsAtUtc, endsAtUtc, maxUsageCount, title, description);
        IsActive = isActive;
    }

    public void UpdateDetails(
        string code,
        EventDiscountGenderScope genderScope,
        EventDiscountType discountType,
        decimal value,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        int maxUsageCount,
        string? title = null,
        string? description = null)
    {
        if (endsAtUtc <= startsAtUtc)
            throw new BusinessRuleViolationException("Invalid discount period", "Discount code end time must be after start time.");

        Code = NormalizeCode(code);
        GenderScope = GuardAgainst.Number.AgainstInvalidEnum<EventDiscountGenderScope>((int)genderScope, nameof(genderScope));
        DiscountType = GuardAgainst.Number.AgainstInvalidEnum<EventDiscountType>((int)discountType, nameof(discountType));
        Value = discountType switch
        {
            EventDiscountType.FixedAmount => GuardAgainst.Number.OutOfRange(value, nameof(value), 1m, 1_000_000m),
            EventDiscountType.Percentage => GuardAgainst.Number.OutOfRange(value, nameof(value), 1m, 100m),
            _ => throw new BusinessRuleViolationException("Invalid discount type", "Discount type is not supported.")
        };
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
        MaxUsageCount = GuardAgainst.Number.Positive(maxUsageCount, nameof(maxUsageCount));
        Title = NormalizeOptionalText(title, nameof(title), 120);
        Description = NormalizeOptionalText(description, nameof(description), 500);
        UpdateTimestamp();
    }

    public decimal CalculateDiscountedPrice(decimal originalPrice)
    {
        var normalizedOriginalPrice = GuardAgainst.Number.OutOfRange(originalPrice, nameof(originalPrice), 0.01m, 1_000_000m);
        var discounted = DiscountType switch
        {
            EventDiscountType.FixedAmount => normalizedOriginalPrice - Value,
            EventDiscountType.Percentage => normalizedOriginalPrice - (normalizedOriginalPrice * Value / 100m),
            _ => normalizedOriginalPrice
        };

        return Math.Max(0m, Math.Round(discounted, 2, MidpointRounding.AwayFromZero));
    }

    public void EnsureCanUse(long eventId, Gender gender, DateTime nowUtc, decimal originalPrice)
    {
        if (DatingEventId is not null && DatingEventId != eventId)
            throw new BusinessRuleViolationException("Discount code mismatch", "Discount code is not valid for this event.");

        if (!IsActive)
            throw new BusinessRuleViolationException("Discount code inactive", "Discount code is inactive.");

        if (StartsAtUtc > nowUtc || EndsAtUtc < nowUtc)
            throw new BusinessRuleViolationException("Discount code expired", "Discount code is not valid at this time.");

        if (UsedCount >= MaxUsageCount)
            throw new BusinessRuleViolationException("Discount code limit reached", "Discount code usage limit has been reached.");

        if (!AllowsGender(gender))
            throw new BusinessRuleViolationException("Discount code gender mismatch", "Discount code is not valid for this user.");

        if (CalculateDiscountedPrice(originalPrice) <= 0m)
            throw new BusinessRuleViolationException("Discount code invalid amount", "Discount code cannot reduce the ticket price below zero.");
    }

    public void RegisterUsage(DateTime usedAtUtc)
    {
        if (UsedCount >= MaxUsageCount)
            throw new BusinessRuleViolationException("Discount code limit reached", "Discount code usage limit has been reached.");

        UsedCount++;
        LastUsedAtUtc = usedAtUtc;
        UpdateTimestamp();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdateTimestamp();
    }

    public bool AllowsGender(Gender gender) => GenderScope switch
    {
        EventDiscountGenderScope.All => true,
        EventDiscountGenderScope.Male => gender == Gender.Male,
        EventDiscountGenderScope.Female => gender == Gender.Female,
        _ => false
    };

    private static string NormalizeCode(string code)
    {
        var candidate = code is null ? string.Empty : code.Trim().ToUpperInvariant();
        var normalized = GuardAgainst.String.InvalidLength(candidate, nameof(code), 3, 50);
        if (normalized.Any(char.IsWhiteSpace))
            throw new BusinessRuleViolationException("Invalid discount code", "Discount code cannot contain spaces.");

        return normalized;
    }

    private static string? NormalizeOptionalText(string? value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        return GuardAgainst.String.MaxLength(value.Trim(), parameterName, maxLength);
    }
}
