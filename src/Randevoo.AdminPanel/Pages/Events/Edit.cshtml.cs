using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class EditModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public EditModel(IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _session = session;
    }

    [BindProperty]
    public EventDraftInput Input { get; set; } = new();

    [BindProperty]
    public string StartAtText { get; set; } = string.Empty;

    [BindProperty]
    public string EndAtText { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? Image1File { get; set; }

    [BindProperty]
    public IFormFile? Image2File { get; set; }

    [BindProperty]
    public IFormFile? Image3File { get; set; }

    [BindProperty(SupportsGet = true)]
    public Guid? ExistingEventId { get; set; }

    public bool IsNew => ExistingEventId is null;

    public bool IsRtl => _session.IsRtl;

    public bool IsAdmin => _session.CurrentUser?.Role is AdminRole.Admin or AdminRole.SupportTeam;

    public bool IsPlanner => _session.CurrentUser?.Role == AdminRole.EventPlanner;

    public string? ReviewNote { get; set; }

    public string StatusText { get; set; } = EventApprovalState.Draft.ToString();

    public string StatusClass { get; set; } = "status-draft";

    public SelectList CountryOptions => new(new[] { "Iran", "United Arab Emirates", "Turkey" });

    public SelectList CityOptions => new(new[] { "Tehran", "Mashhad", "Shiraz", "Isfahan", "Tabriz" });

    public SelectList AgeRangeOptions => new(new[] { "20-30", "25-35", "30-40", "35-45" });

    public SelectList EventTypeOptions => new(Enum.GetValues<EventType>().Select(item => new { Value = item, Text = item.ToString() }), "Value", "Text");

    public async Task<IActionResult> OnGetAsync()
    {
        if (ExistingEventId is Guid id)
        {
            var @event = await _eventsApi.GetEventAsync(id);
            if (@event is null)
            {
                return NotFound();
            }

            Input = @event.ActiveDraft;
            ReviewNote = @event.AdminReviewNote ?? @event.Pending?.ReviewNote;
            StatusText = @event.Status.ToString();
            StatusClass = GetStatusClass(@event.Status);
        }
        else
        {
            if (_session.CurrentUser is null)
            {
                return RedirectToPage("/Account/Login");
            }

            Input = new EventDraftInput();
            StatusText = EventApprovalState.Draft.ToString();
            StatusClass = GetStatusClass(EventApprovalState.Draft);
        }

        StartAtText = _session.IsRtl
            ? PersianDateFormatter.Format(Input.StartAtUtc, useShamsi: true)
            : Input.StartAtUtc.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");

        EndAtText = _session.IsRtl
            ? PersianDateFormatter.Format(Input.EndAtUtc, useShamsi: true)
            : Input.EndAtUtc.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("Current user was not resolved.");

        Input.StartAtUtc = _session.IsRtl ? PersianDateFormatter.Parse(StartAtText) : DateTimeOffset.Parse(StartAtText).ToUniversalTime();
        Input.EndAtUtc = _session.IsRtl ? PersianDateFormatter.Parse(EndAtText) : DateTimeOffset.Parse(EndAtText).ToUniversalTime();

        if (Image1File is not null)
        {
            Input.Image1 = await ToDataUrlAsync(Image1File);
        }

        if (Image2File is not null)
        {
            Input.Image2 = await ToDataUrlAsync(Image2File);
        }

        if (Image3File is not null)
        {
            Input.Image3 = await ToDataUrlAsync(Image3File);
        }

        if (current.Role == AdminRole.EventPlanner && ExistingEventId is Guid editId)
        {
            var existing = await _eventsApi.GetEventAsync(editId);
            if (existing is not null)
            {
                Input.OrganizerCommissionPercent = existing.ActiveDraft.OrganizerCommissionPercent;
            }
        }

        var saved = await _eventsApi.SaveEventAsync(Input, current, ExistingEventId);
        return RedirectToPage("/Events/Details", new { id = saved.Id });
    }

    public static string GetStatusClass(EventApprovalState state) => state switch
    {
        EventApprovalState.Approved => "status-approved",
        EventApprovalState.PendingAdminReview => "status-pending",
        EventApprovalState.Rejected => "status-rejected",
        EventApprovalState.Closed => "status-closed",
        EventApprovalState.Cancelled => "status-cancelled",
        _ => "status-draft"
    };

    private static async Task<string> ToDataUrlAsync(IFormFile file)
    {
        await using var memory = new MemoryStream();
        await file.CopyToAsync(memory);
        var base64 = Convert.ToBase64String(memory.ToArray());
        return $"data:{file.ContentType};base64,{base64}";
    }
}

