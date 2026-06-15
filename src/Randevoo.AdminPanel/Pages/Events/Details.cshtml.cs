using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using DomainEventApprovalStatus = Randevoo.Domain.Enums.EventApprovalStatus;

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

    public PlannerProfileViewModel? PlannerProfile { get; private set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;

    public bool IsRtl => _session.IsRtl;

    public string OperationalStatusClass => EditModel.GetOperationalStatusClass(Event.OperationalStatus);

    public string ProfileStatusClass => DisplayFormatter.ApprovalStatusClass(Event.ApprovalStatus);

    public IReadOnlyList<EventChangeLogEntry> ChangeLogEntries => Event.ChangeLog
        .Where(item => !string.Equals(item.Category, "communication", StringComparison.OrdinalIgnoreCase))
        .OrderByDescending(item => item.CreatedAtUtc)
        .ToList();

    public IReadOnlyList<EventChangeLogEntry> ProfileReviewHistoryEntries => Event.ChangeLog
        .Where(item => string.Equals(item.Category, "review", StringComparison.OrdinalIgnoreCase))
        .OrderByDescending(item => item.CreatedAtUtc)
        .ToList();

    public EventProfileReviewHistoryModalViewModel ProfileReviewHistoryModal => new()
    {
        EventId = Event.Id,
        EventTitle = Event.DisplayTitle,
        Entries = ProfileReviewHistoryEntries
    };

    public IReadOnlyList<string> EventImages => BuildImageList(Event.ActiveDraft);

    public EventImageCarouselModel EventImageCarousel => new()
    {
        CarouselId = $"event-slider-{Event.Id}",
        Images = EventImages
    };

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var @event = await _eventsApi.GetEventAsync(id);
        if (@event is null)
        {
            return NotFound();
        }

        Event = @event;
        PlannerProfile = await _plannerProfilesApi.GetByUserIdAsync(@event.PlannerUserId);

        return Page();
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(long id, EventStatusTransitionAction action, string? note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        try
        {
            await _eventsApi.ApplyStatusTransitionAsync(id, current, action, note);
            StatusMessage = "وضعیت رویداد با موفقیت تغییر کرد.";
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostApproveProfileAsync(long id, string? note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        try
        {
            await _eventsApi.ApproveAsync(id, current, note: note);
            StatusMessage = "پروفایل رویداد تایید شد. وضعیت عملیاتی همچنان فروش بسته می‌ماند تا جداگانه تغییر کند.";
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRejectProfileAsync(long id, string note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        if (string.IsNullOrWhiteSpace(note))
        {
            ErrorMessage = "برای رد پروفایل رویداد، توضیح مدیر لازم است.";
            return RedirectToPage(new { id });
        }

        try
        {
            await _eventsApi.RejectAsync(id, current, note);
            StatusMessage = "پروفایل رویداد برای اصلاح به پیش‌نویس برگشت.";
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }

        return RedirectToPage(new { id });
    }

    public EventStatusTransitionModalViewModel CreateStatusTransitionModal()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        return new EventStatusTransitionModalViewModel
        {
            Event = Event,
            Options = EventStatusTransitionCatalog.GetOptions(Event, current.Role),
            EmptyMessage = EventStatusTransitionCatalog.GetEmptyMessage(Event),
            ReturnUrl = Request.Path + Request.QueryString
        };
    }

    private static IReadOnlyList<string> BuildImageList(EventDraftInput draft)
    {
        return new[] { draft.Image1, draft.Image2, draft.Image3 }
            .Where(image => !string.IsNullOrWhiteSpace(image))
            .Cast<string>()
            .ToList();
    }

}
