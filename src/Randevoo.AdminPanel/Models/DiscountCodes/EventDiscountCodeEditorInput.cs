using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.DiscountCodes;

public sealed class EventDiscountCodeEditorInput
{
    public long? DatingEventId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public EventDiscountGenderScope GenderScope { get; set; } = EventDiscountGenderScope.All;

    public EventDiscountType DiscountType { get; set; } = EventDiscountType.Percentage;

    public decimal Value { get; set; } = 10m;

    public DateTime StartsAtUtc { get; set; } = DateTime.UtcNow.Date;

    public DateTime EndsAtUtc { get; set; } = DateTime.UtcNow.Date.AddDays(30).AddHours(23).AddMinutes(59);

    public int MaxUsageCount { get; set; } = 100;

    public bool IsActive { get; set; } = true;
}
