namespace Randevoo.ControlCenter.Models.Events;

public sealed class EventDraftInput
{
    public string Title { get; set; } = "شب آشنایی تهران";

    public string Country { get; set; } = "ایران";

    public string City { get; set; } = "تهران";

    public string? Region { get; set; } = "منطقه ۳";

    public decimal Latitude { get; set; } = 35.7219m;

    public decimal Longitude { get; set; } = 51.3347m;

    public string VenueName { get; set; } = "سالن شمالی";

    public string Address { get; set; } = "تهران، خیابان ولیعصر";

    public DateTimeOffset StartsAt { get; set; } = DateTimeOffset.Now.AddDays(10);

    public DateTimeOffset EndsAt { get; set; } = DateTimeOffset.Now.AddDays(10).AddHours(3);

    public long EventTypeId { get; set; } = 1;

    public string EventType { get; set; } = "دورهمی اجتماعی";

    public string AgeRangeForMale { get; set; } = "25-35";

    public string AgeRangeForFemale { get; set; } = "20-30";

    public bool IsOpenForSell { get; set; }

    public int MaleCapacity { get; set; } = 24;

    public int FemaleCapacity { get; set; } = 24;

    public int NumberOfChatAllowed { get; set; } = 5;

    public decimal TicketPrice { get; set; } = 2_500_000m;

    public string EducationLevelRestriction { get; set; } = "WithoutLimit";

    public decimal EventPlannerCommissionPercent { get; set; } = 10m;

    public string? EventImage1 { get; set; }

    public string? EventImage2 { get; set; }

    public string? EventImage3 { get; set; }

    public string EventDescriptionHtml { get; set; } = "یک رویداد حضوری مدیریت‌شده برای آشنایی محترمانه، با ظرفیت کنترل‌شده و بررسی نهایی توسط ادمین.";

    public static EventDraftInput FromEvent(EventSummary item)
    {
        return new EventDraftInput
        {
            Title = item.Title,
            Country = item.Country,
            City = item.City,
            Region = item.Region,
            Latitude = item.Latitude,
            Longitude = item.Longitude,
            VenueName = item.VenueName,
            Address = item.Address,
            StartsAt = item.StartsAt,
            EndsAt = item.EndsAt,
            EventTypeId = item.EventTypeId,
            EventType = item.EventType,
            AgeRangeForMale = item.AgeRangeForMale,
            AgeRangeForFemale = item.AgeRangeForFemale,
            IsOpenForSell = item.IsOpenForSell,
            MaleCapacity = item.MaleCapacity,
            FemaleCapacity = item.FemaleCapacity,
            NumberOfChatAllowed = item.NumberOfChatAllowed,
            TicketPrice = item.TicketPrice,
            EducationLevelRestriction = item.EducationLevelRestriction,
            EventPlannerCommissionPercent = item.EventPlannerCommissionPercent,
            EventImage1 = item.EventImage1,
            EventImage2 = item.EventImage2,
            EventImage3 = item.EventImage3,
            EventDescriptionHtml = item.Description
        };
    }
}
