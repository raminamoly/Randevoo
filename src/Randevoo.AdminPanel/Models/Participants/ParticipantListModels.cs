namespace Randevoo.AdminPanel.Models.Participants;

public sealed class ParticipantListResult
{
    public int TotalCount { get; set; }
    public ParticipantListSummary Summary { get; set; } = new();
    public IReadOnlyList<ParticipantListItem> Items { get; set; } = Array.Empty<ParticipantListItem>();
}

public sealed class ParticipantListSummary
{
    public int TotalParticipants { get; set; }
    public int TotalOrders { get; set; }
    public int TotalBuyers { get; set; }
    public int? AvailableCapacity { get; set; }
    public int CompletedProfiles { get; set; }
    public int PendingProfiles { get; set; }
    public int ActiveTickets { get; set; }
    public int CancelledTickets { get; set; }
}

public sealed class ParticipantListItem
{
    public bool IsDirectoryRow { get; init; }
    public long TicketId { get; init; }
    public long? TicketOrderId { get; init; }
    public long? EventId { get; init; }
    public string EventTitle { get; init; } = string.Empty;
    public long? EventPlannerUserId { get; init; }
    public string EventPlannerName { get; init; } = string.Empty;
    public long ParticipantUserId { get; init; }
    public string ParticipantName { get; init; } = string.Empty;
    public string? ParticipantMobile { get; init; }
    public string? ProfileImageUrl { get; init; }
    public string GenderTitle { get; init; } = string.Empty;
    public int Age { get; init; }
    public string EducationLevelTitle { get; init; } = string.Empty;
    public string CityTitle { get; init; } = string.Empty;
    public bool HasProfile { get; init; }
    public bool IsProfileComplete { get; init; }
    public bool IsActive { get; init; } = true;
    public string ProfileStatusTitle => IsProfileComplete ? "پروفایل کامل" : "در انتظار تکمیل";
    public decimal TicketPrice { get; init; }
    public string TicketCurrencyCode { get; init; } = "IRR";
    public int TicketCount { get; init; }
    public int ActiveTicketCount { get; init; }
    public int CancelledTicketCount { get; init; }
    public int EventCount { get; init; }
    public int SupportTicketCount { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? LastActivityAtUtc { get; init; }
    public DateTime? LastTicketAtUtc { get; init; }
    public bool IsRefunded { get; init; }
    public bool IsRemoved { get; init; }
    public string TicketStatus { get; init; } = string.Empty;
    public DateTime PurchasedAtUtc { get; init; }
    public string? RemovalReason { get; init; }
    public long? BuyerUserId { get; init; }
    public string BuyerName { get; init; } = string.Empty;
    public string? BuyerMobile { get; init; }
}
