using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventDraftInput
{
    public string Title { get; set; } = string.Empty;

    public string Country { get; set; } = "ایران";

    public string City { get; set; } = "تهران";

    public string Region { get; set; } = string.Empty;

    public string VenueName { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public decimal Latitude { get; set; } = 35.7219m;

    public decimal Longitude { get; set; } = 51.3347m;

    public long EventTypeId { get; set; }

    public string EventTypeName { get; set; } = string.Empty;

    public string AgeRangeForMale { get; set; } = "25-35";

    public string AgeRangeForFemale { get; set; } = "25-35";

    public bool IsOpenForSell { get; set; } = false;

    public decimal TicketPrice { get; set; } = 950000m;

    public EventEducationLevelRestriction EducationLevelRestriction { get; set; } = EventEducationLevelRestriction.WithoutLimit;

    public long? MinimumEducationLevelId { get; set; }

    public decimal OrganizerCommissionPercent { get; set; } = 12m;

    public int CapacityMale { get; set; } = 40;

    public int CapacityFemale { get; set; } = 40;

    public int ChatLimit { get; set; } = 80;

    public List<string> Tags { get; set; } = new();

    [ValidateNever]
    public List<long> TagIds { get; set; } = new();

    public string DescriptionHtml { get; set; } = string.Empty;

    public string? Image1 { get; set; }

    public string? Image2 { get; set; }

    public string? Image3 { get; set; }

    public DateTimeOffset StartAtUtc { get; set; } = DateTimeOffset.UtcNow.AddDays(14);

    public DateTimeOffset EndAtUtc { get; set; } = DateTimeOffset.UtcNow.AddDays(14).AddHours(3);
}
