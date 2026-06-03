using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class DetailsModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;
    private readonly IPlannerProfilesApiClient _plannerProfilesApi;

    public DetailsModel(IEventsApiClient eventsApi, IPlannerProfilesApiClient plannerProfilesApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _plannerProfilesApi = plannerProfilesApi;
        _session = session;
    }

    public DatingEvent Event { get; private set; } = new();

    public IReadOnlyList<PendingEventChangeItem> PendingChanges { get; private set; } = Array.Empty<PendingEventChangeItem>();

    public PlannerProfileViewModel? PlannerProfile { get; private set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public bool IsAdmin => _session.CurrentUser?.Role is AdminRole.Admin or AdminRole.SupportTeam;

    public bool IsRtl => _session.IsRtl;

    public string StatusClass => EditModel.GetStatusClass(Event.Status);

    public IReadOnlyList<string> EventImages => BuildImageList(Event.ActiveDraft);

    public EventImageCarouselModel EventImageCarousel => new()
    {
        CarouselId = $"event-slider-{Event.Id}",
        Images = EventImages
    };

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var @event = await _eventsApi.GetEventAsync(id);
        if (@event is null)
        {
            return NotFound();
        }

        Event = @event;
        PendingChanges = BuildChanges(@event);
        if (Guid.TryParse(@event.PlannerId, out var plannerId))
        {
            PlannerProfile = await _plannerProfilesApi.GetByUserIdAsync(plannerId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(Guid id, decimal commissionPercent, string? note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ApproveAsync(id, current, commissionPercent, note);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRejectAsync(Guid id, string note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.RejectAsync(id, current, note);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostOpenSaleAsync(Guid id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ToggleSaleAsync(id, current, true);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCloseSaleAsync(Guid id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ToggleSaleAsync(id, current, false);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelAsync(Guid id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.CancelAsync(id, current);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostSendSmsAsync(Guid id, string message)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.SendSmsAsync(id, current, message);
        StatusMessage = "پیامک برای شرکت کنندگان رویداد ثبت شد.";
        return RedirectToPage(new { id });
    }

    private static IReadOnlyList<PendingEventChangeItem> BuildChanges(DatingEvent @event)
    {
        if (@event.Pending is null)
        {
            return Array.Empty<PendingEventChangeItem>();
        }

        var current = @event.Live;
        var next = @event.Pending.Draft;
        var changes = new List<PendingEventChangeItem>();

        AddTextChange(changes, "عنوان", current.Title, next.Title);
        AddTextChange(changes, "کشور", current.Country, next.Country);
        AddTextChange(changes, "شهر", current.City, next.City);
        AddTextChange(changes, "منطقه", current.Region, next.Region);
        AddTextChange(changes, "محل برگزاری", current.VenueName, next.VenueName);
        AddTextChange(changes, "آدرس", current.Address, next.Address);
        AddTextChange(changes, "نوع رویداد", DisplayFormatter.EventTypeLabel(current.EventType), DisplayFormatter.EventTypeLabel(next.EventType));
        AddTagChange(changes, "تگ های رویداد", current.Tags, next.Tags);
        AddTextChange(changes, "بازه سنی آقایان", current.AgeRangeForMale, next.AgeRangeForMale);
        AddTextChange(changes, "بازه سنی بانوان", current.AgeRangeForFemale, next.AgeRangeForFemale);
        AddTextChange(changes, "قیمت بلیت", DisplayFormatter.Number(current.TicketPrice), DisplayFormatter.Number(next.TicketPrice));
        AddTextChange(changes, "ظرفیت آقایان", DisplayFormatter.Count(current.CapacityMale), DisplayFormatter.Count(next.CapacityMale));
        AddTextChange(changes, "ظرفیت بانوان", DisplayFormatter.Count(current.CapacityFemale), DisplayFormatter.Count(next.CapacityFemale));
        AddTextChange(changes, "تعداد گفتگو", DisplayFormatter.Count(current.ChatLimit), DisplayFormatter.Count(next.ChatLimit));
        AddTextChange(changes, "وضعیت فروش", DisplayFormatter.Bool(current.IsOpenForSell), DisplayFormatter.Bool(next.IsOpenForSell));
        AddTextChange(changes, "زمان شروع", PersianDateFormatter.Format(current.StartAtUtc, true), PersianDateFormatter.Format(next.StartAtUtc, true));
        AddTextChange(changes, "زمان پایان", PersianDateFormatter.Format(current.EndAtUtc, true), PersianDateFormatter.Format(next.EndAtUtc, true));
        AddHtmlChange(changes, "توضیحات رویداد", current.DescriptionHtml, next.DescriptionHtml);
        AddImageChange(changes, "تصویر اول", current.Image1, next.Image1);
        AddImageChange(changes, "تصویر دوم", current.Image2, next.Image2);
        AddImageChange(changes, "تصویر سوم", current.Image3, next.Image3);

        if (changes.Count == 0)
        {
            changes.Add(new PendingEventChangeItem
            {
                Label = "بازبینی مجدد",
                BeforeText = "تغییر قابل مشاهده ای ثبت نشده است.",
                AfterText = "درخواست بازبینی دوباره برای رویداد ارسال شده است."
            });
        }

        return changes;
    }

    private static void AddTextChange(List<PendingEventChangeItem> changes, string label, string? oldValue, string? newValue)
    {
        var normalizedOld = NormalizeReviewText(oldValue);
        var normalizedNew = NormalizeReviewText(newValue);
        if (!string.Equals(normalizedOld, normalizedNew, StringComparison.Ordinal))
        {
            changes.Add(new PendingEventChangeItem
            {
                Label = label,
                BeforeText = normalizedOld,
                AfterText = normalizedNew
            });
        }
    }

    private static void AddHtmlChange(List<PendingEventChangeItem> changes, string label, string oldValue, string newValue)
    {
        if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            changes.Add(new PendingEventChangeItem
            {
                Label = label,
                BeforeHtml = oldValue,
                AfterHtml = newValue,
                IsHtml = true
            });
        }
    }

    private static void AddImageChange(List<PendingEventChangeItem> changes, string label, string? oldValue, string? newValue)
    {
        if (!string.Equals(oldValue, newValue, StringComparison.Ordinal))
        {
            changes.Add(new PendingEventChangeItem
            {
                Label = label,
                BeforeImageUrl = oldValue,
                AfterImageUrl = newValue,
                IsImage = true
            });
        }
    }

    private static void AddTagChange(List<PendingEventChangeItem> changes, string label, IReadOnlyList<string> oldTags, IReadOnlyList<string> newTags)
    {
        var oldSerialized = string.Join('|', oldTags);
        var newSerialized = string.Join('|', newTags);
        if (!string.Equals(oldSerialized, newSerialized, StringComparison.Ordinal))
        {
            changes.Add(new PendingEventChangeItem
            {
                Label = label,
                BeforeTags = oldTags.ToList(),
                AfterTags = newTags.ToList(),
                IsTagList = true
            });
        }
    }

    private static IReadOnlyList<string> BuildImageList(EventDraftInput draft)
    {
        return new[] { draft.Image1, draft.Image2, draft.Image3 }
            .Where(image => !string.IsNullOrWhiteSpace(image))
            .Cast<string>()
            .ToList();
    }

    private static string NormalizeReviewText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "ثبت نشده" : value.Trim();
    }
}
