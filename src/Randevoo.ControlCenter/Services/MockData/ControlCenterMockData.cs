using MudBlazor;
using Randevoo.ControlCenter.Models.Common;
using Randevoo.ControlCenter.Models.EventPlanners;
using Randevoo.ControlCenter.Models.Events;

namespace Randevoo.ControlCenter.Services.MockData;

public sealed class ControlCenterMockData
{
    private readonly List<EventSummary> _events =
    [
        new(Guid.Parse("19733337-708f-45af-9c4d-01574dd8ac10"), "دورهمی عصرانه تهران", "Nava Events", "ایران", "تهران", "منطقه ۳", 35.7219m, 51.3347m, "باغ‌رووف جردن", "تهران، جردن، خیابان گلستان", "رویداد حضوری با پذیرش کنترل‌شده، ظرفیت تفکیک‌شده و نظرسنجی بعد از برنامه.", DateTimeOffset.Now.AddDays(3), DateTimeOffset.Now.AddDays(3).AddHours(3), 1, "دورهمی اجتماعی", EventStatus.OnSale, true, false, 10m, 40, 40, 5, 54, 135_000_000m, 2_500_000m, "25-35", "20-30", "/images/events/tehran-rooftop.svg", "/images/events/social-evening.svg", null, DateTimeOffset.Now.AddDays(-12), DateTimeOffset.Now.AddDays(-10)),
        new(Guid.Parse("8db13d6a-22d2-4d1e-b5ee-9d7b15c09ad2"), "شب گالری شیراز", "Orange Room", "ایران", "شیراز", "حافظیه", 29.625m, 52.545m, "گالری حافظ", "شیراز، محدوده حافظیه", "یک شب فرهنگی در فضای گالری با زمان‌بندی ورود و ظرفیت محدود.", DateTimeOffset.Now.AddDays(8), DateTimeOffset.Now.AddDays(8).AddHours(2), 2, "فرهنگی و هنری", EventStatus.Scheduled, false, false, 12m, 21, 21, 3, 19, 47_500_000m, 2_500_000m, "25-35", "25-35", "/images/events/gallery-night.svg", null, null, DateTimeOffset.Now.AddDays(-6), DateTimeOffset.Now.AddDays(-5)),
        new(Guid.Parse("b9376c3c-0ad2-4767-b074-3367a2cd9917"), "کافه‌نشینی اصفهان", "Nava Events", "ایران", "اصفهان", "چهارباغ", 32.6546m, 51.668m, "استودیو بلوکاپ", "اصفهان، چهارباغ عباسی", "رویداد کوچک برگزارکننده که منتظر تایید ادمین برای شروع فروش است.", DateTimeOffset.Now.AddDays(15), DateTimeOffset.Now.AddDays(15).AddHours(2), 3, "کافه و گفتگو", EventStatus.PendingAdminReview, false, false, 10m, 18, 18, 4, 0, 0m, 1_800_000m, "30-40", "25-35", "/images/events/coffee-circle.svg", null, null, DateTimeOffset.Now.AddDays(-1), null),
        new(Guid.Parse("ef5d0283-38d9-4b4c-9704-2d6f65d912b1"), "شام مدیران تهران", "North Star Gatherings", "ایران", "تهران", "ونک", 35.7575m, 51.409m, "سالن اطلس", "تهران، میدان ونک", "رویداد بسته‌شده که به عنوان سابقه عملیاتی نگهداری می‌شود.", DateTimeOffset.Now.AddDays(-2), DateTimeOffset.Now.AddDays(-2).AddHours(3), 4, "شام رسمی", EventStatus.Closed, false, false, 15m, 14, 14, 2, 28, 196_000_000m, 7_000_000m, "35-45", "30-40", "/images/events/dinner.svg", null, null, DateTimeOffset.Now.AddDays(-25), DateTimeOffset.Now.AddDays(-23))
    ];

    public IReadOnlyList<DashboardMetric> AdminMetrics { get; } =
    [
        new("Active events", "18", "5 selling this week", Icons.Material.Filled.EventAvailable, "#2563eb"),
        new("Ticket revenue", "$84.2k", "Mock financial snapshot", Icons.Material.Filled.Payments, "#059669"),
        new("Planner reviews", "12", "4 waiting for verification", Icons.Material.Filled.ManageSearch, "#7c3aed"),
        new("Moderation queue", "7", "2 high-priority reports", Icons.Material.Filled.Policy, "#dc2626")
    ];

    public IReadOnlyList<DashboardMetric> PlannerMetrics { get; } =
    [
        new("My events", "6", "3 upcoming", Icons.Material.Filled.EventNote, "#2563eb"),
        new("Tickets sold", "214", "Across upcoming events", Icons.Material.Filled.ConfirmationNumber, "#059669"),
        new("Available balance", "$12.6k", "Mock settlement data", Icons.Material.Filled.AccountBalanceWallet, "#0891b2"),
        new("Survey score", "4.7", "Average recent rating", Icons.Material.Filled.QueryStats, "#7c3aed")
    ];

    public IReadOnlyList<EventSummary> Events => _events;

    public IReadOnlyList<EventPlannerSummary> EventPlanners { get; } =
    [
        new(Guid.Parse("256d6de8-e275-4211-a903-34048ca9151d"), "Sara M.", "Nava Events", "Tehran", 12600m, 3, true),
        new(Guid.Parse("749de263-8bdb-482c-bcf1-f4c0e61548c0"), "Arman K.", "Orange Room", "Shiraz", 4800m, 1, false),
        new(Guid.Parse("67bf3ed1-ad2b-4ff9-80cd-f599a35f7537"), "Leila R.", "North Star Gatherings", "Tehran", 9100m, 2, true)
    ];

    public EventSummary AddPlannerEvent(EventDraftInput input)
    {
        var item = new EventSummary(
            Guid.NewGuid(),
            input.Title,
            "Nava Events",
            input.Country,
            input.City,
            input.Region,
            input.Latitude,
            input.Longitude,
            input.VenueName,
            input.Address,
            input.EventDescriptionHtml,
            input.StartsAt,
            input.EndsAt,
            input.EventTypeId,
            input.EventType,
            EventStatus.PendingAdminReview,
            input.IsOpenForSell,
            false,
            0m,
            input.MaleCapacity,
            input.FemaleCapacity,
            input.NumberOfChatAllowed,
            0,
            0m,
            input.TicketPrice,
            input.AgeRangeForMale,
            input.AgeRangeForFemale,
            input.EventImage1,
            input.EventImage2,
            input.EventImage3,
            DateTimeOffset.Now,
            null);

        _events.Insert(0, item);
        return item;
    }

    public EventSummary? GetEvent(Guid id) => _events.FirstOrDefault(item => item.Id == id);

    public EventSummary? UpdatePlannerEvent(Guid id, EventDraftInput input)
    {
        var index = _events.FindIndex(item => item.Id == id);
        if (index < 0)
        {
            return null;
        }

        var current = _events[index];
        var updated = current with
        {
            Title = input.Title,
            Country = input.Country,
            City = input.City,
            Region = input.Region,
            Latitude = input.Latitude,
            Longitude = input.Longitude,
            VenueName = input.VenueName,
            Address = input.Address,
            Description = input.EventDescriptionHtml,
            StartsAt = input.StartsAt,
            EndsAt = input.EndsAt,
            EventTypeId = input.EventTypeId,
            EventType = input.EventType,
            Status = EventStatus.PendingAdminReview,
            IsOpenForSell = false,
            IsCancelled = false,
            MaleCapacity = input.MaleCapacity,
            FemaleCapacity = input.FemaleCapacity,
            NumberOfChatAllowed = input.NumberOfChatAllowed,
            TicketPrice = input.TicketPrice,
            AgeRangeForMale = input.AgeRangeForMale,
            AgeRangeForFemale = input.AgeRangeForFemale,
            EventImage1 = input.EventImage1,
            EventImage2 = input.EventImage2,
            EventImage3 = input.EventImage3,
            ConfirmedAt = null
        };

        _events[index] = updated;
        return updated;
    }

    public EventSummary? ConfirmEvent(Guid id, decimal commissionPercent)
    {
        var index = _events.FindIndex(item => item.Id == id);
        if (index < 0)
        {
            return null;
        }

        var current = _events[index];
        var confirmed = current with
        {
            Status = EventStatus.Scheduled,
            EventPlannerCommissionPercent = commissionPercent,
            ConfirmedAt = DateTimeOffset.Now
        };
        _events[index] = confirmed;
        return confirmed;
    }

    public EventSummary? SetCommission(Guid id, decimal commissionPercent)
    {
        return UpdateEvent(id, item => item with { EventPlannerCommissionPercent = commissionPercent });
    }

    public EventSummary? OpenForSell(Guid id)
    {
        return UpdateEvent(id, item => item.IsCancelled ? item : item with { IsOpenForSell = true, Status = EventStatus.OnSale });
    }

    public EventSummary? CloseForSell(Guid id)
    {
        return UpdateEvent(id, item => item with { IsOpenForSell = false, Status = EventStatus.Scheduled });
    }

    public EventSummary? CancelEvent(Guid id)
    {
        return UpdateEvent(id, item => item with { IsCancelled = true, IsOpenForSell = false, Status = EventStatus.Cancelled });
    }

    private EventSummary? UpdateEvent(Guid id, Func<EventSummary, EventSummary> update)
    {
        var index = _events.FindIndex(item => item.Id == id);
        if (index < 0)
        {
            return null;
        }

        var updated = update(_events[index]);
        _events[index] = updated;
        return updated;
    }
}
