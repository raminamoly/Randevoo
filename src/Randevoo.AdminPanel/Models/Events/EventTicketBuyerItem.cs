using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventTicketBuyerItem
{
    public long TicketId { get; init; }
    public long EventId { get; init; }
    public long UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? MobileNumber { get; init; }
    public Gender Gender { get; init; }
    public string GenderTitle { get; init; } = string.Empty;
    public int Age { get; init; }
    public string EducationLevelTitle { get; init; } = string.Empty;
    public string Country { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public decimal TicketPrice { get; init; }
    public bool IsRefunded { get; init; }
    public bool IsRemoved { get; init; }
    public string TicketStatus { get; init; } = string.Empty;
    public DateTime PurchasedAtUtc { get; init; }
    public string? RemovalReason { get; init; }
}
