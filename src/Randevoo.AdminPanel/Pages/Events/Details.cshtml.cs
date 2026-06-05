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

    public PlannerProfileViewModel? PlannerProfile { get; private set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;

    public bool IsRtl => _session.IsRtl;

    public string OperationalStatusClass => EditModel.GetOperationalStatusClass(Event.OperationalStatus);

    public string ReviewStatusClass => EditModel.GetReviewStatusClass(Event.ReviewStatus);

    public IReadOnlyList<EventChangeLogEntry> ChangeLogEntries => Event.ChangeLog
        .Where(item => !string.Equals(item.Category, "communication", StringComparison.OrdinalIgnoreCase))
        .OrderByDescending(item => item.CreatedAtUtc)
        .ToList();

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

    public async Task<IActionResult> OnPostApproveAsync(long id, decimal commissionPercent, string? note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ApproveAsync(id, current, commissionPercent, note);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostApproveAndOpenAsync(long id, decimal commissionPercent, string? note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ApproveAsync(id, current, commissionPercent, note);
        await _eventsApi.ToggleSaleAsync(id, current, true);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostRejectAsync(long id, string note)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.RejectAsync(id, current, note);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostOpenSaleAsync(long id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ToggleSaleAsync(id, current, true);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCloseSaleAsync(long id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ToggleSaleAsync(id, current, false);
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostCancelAsync(long id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.CancelAsync(id, current);
        return RedirectToPage(new { id });
    }

    private static IReadOnlyList<string> BuildImageList(EventDraftInput draft)
    {
        return new[] { draft.Image1, draft.Image2, draft.Image3 }
            .Where(image => !string.IsNullOrWhiteSpace(image))
            .Cast<string>()
            .ToList();
    }

}
