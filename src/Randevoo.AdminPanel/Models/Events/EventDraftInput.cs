namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventDraftInput
{
    public string Title { get; set; } = string.Empty;

    public string Country { get; set; } = "Iran";

    public string City { get; set; } = "Tehran";

    public string Region { get; set; } = string.Empty;

    public string VenueName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal Latitude { get; set; } = 35.7219m;

    public decimal Longitude { get; set; } = 51.3347m;

    public EventType EventType { get; set; } = EventType.SocialEvening;

    public string AgeRangeForMale { get; set; } = "25-35";

    public string AgeRangeForFemale { get; set; } = "25-35";

    public bool IsOpenForSell { get; set; } = false;

    public decimal TicketPrice { get; set; } = 1800000m;

    public decimal OrganizerCommissionPercent { get; set; } = 12m;

    public int CapacityMale { get; set; } = 40;

    public int CapacityFemale { get; set; } = 40;

    public int ChatLimit { get; set; } = 180;

    public string DescriptionHtml { get; set; } = string.Empty;

    public string? Image1 { get; set; }

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public DateTimeOffset StartAtUtc { get; set; } = DateTimeOffset.UtcNow.AddDays(14);

    public DateTimeOffset EndAtUtc { get; set; } = DateTimeOffset.UtcNow.AddDays(14).AddHours(3);
}

