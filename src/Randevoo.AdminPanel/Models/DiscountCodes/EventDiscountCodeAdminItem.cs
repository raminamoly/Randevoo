using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.DiscountCodes;

public sealed class EventDiscountCodeAdminItem
{
    public long Id { get; init; }

    public long? DatingEventId { get; init; }

    public string EventTitle { get; init; } = string.Empty;

    public string Code { get; init; } = string.Empty;

    public string? Title { get; init; }

    public string? Description { get; init; }

    public EventDiscountGenderScope GenderScope { get; init; }

    public EventDiscountType DiscountType { get; init; }

    public decimal Value { get; init; }

    public DateTime StartsAtUtc { get; init; }

    public DateTime EndsAtUtc { get; init; }

    public int MaxUsageCount { get; init; }

    public int UsedCount { get; init; }

    public bool IsActive { get; init; }

    public bool IsExpired => EndsAtUtc < DateTime.UtcNow;
}
