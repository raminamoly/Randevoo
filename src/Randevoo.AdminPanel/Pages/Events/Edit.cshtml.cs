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
    public string StartDateText { get; set; } = string.Empty;

    [BindProperty]
    public string StartTimeText { get; set; } = string.Empty;

    [BindProperty]
    public string EndDateText { get; set; } = string.Empty;

    [BindProperty]
    public string EndTimeText { get; set; } = string.Empty;

    [BindProperty]
    public string TagsText { get; set; } = string.Empty;

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

    public EventApprovalState StatusValue { get; set; } = EventApprovalState.Draft;

    public string StatusClass { get; set; } = "status-draft";

    public SelectList CountryOptions => new(new[] { "ایران", "امارات متحده عربی", "ترکیه" });

    public SelectList CityOptions => new(new[] { "تهران", "مشهد", "شیراز", "اصفهان", "تبریز" });

    public SelectList AgeRangeOptions => new(new[] { "20-30", "25-35", "30-40", "35-45" });

    public SelectList EventTypeOptions => new(Enum.GetValues<EventType>().Select(item => new { Value = item, Text = DisplayFormatter.EventTypeLabel(item) }), "Value", "Text");

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
            StatusValue = @event.Status;
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
            StatusValue = EventApprovalState.Draft;
            StatusClass = GetStatusClass(EventApprovalState.Draft);
        }

        StartDateText = PersianDateFormatter.FormatDate(Input.StartAtUtc, _session.IsRtl);
        StartTimeText = PersianDateFormatter.FormatTime(Input.StartAtUtc);
        EndDateText = PersianDateFormatter.FormatDate(Input.EndAtUtc, _session.IsRtl);
        EndTimeText = PersianDateFormatter.FormatTime(Input.EndAtUtc);
        TagsText = string.Join("، ", Input.Tags);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");

        Input.StartAtUtc = CombineDateAndTime(StartDateText, StartTimeText, _session.IsRtl);
        Input.EndAtUtc = CombineDateAndTime(EndDateText, EndTimeText, _session.IsRtl);
        Input.Tags = ParseTags(TagsText);

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

    private static DateTimeOffset CombineDateAndTime(string dateText, string timeText, bool useShamsi)
    {
        var normalizedDate = NormalizeNumericText(dateText);
        var normalizedTime = NormalizeNumericText(timeText);

        var parts = normalizedTime.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var hour = parts.Length > 0 ? int.Parse(parts[0]) : 0;
        var minute = parts.Length > 1 ? int.Parse(parts[1]) : 0;

        if (useShamsi)
        {
            var datePart = PersianDateFormatter.Parse($"{normalizedDate} 00:00");
            return new DateTimeOffset(
                datePart.Year,
                datePart.Month,
                datePart.Day,
                hour,
                minute,
                0,
                datePart.Offset).ToUniversalTime();
        }

        var gregorian = DateTimeOffset.Parse($"{normalizedDate} {normalizedTime}").ToUniversalTime();
        return gregorian;
    }

    private static string NormalizeNumericText(string value) => (value ?? string.Empty)
        .Trim()
        .Replace('۰', '0')
        .Replace('۱', '1')
        .Replace('۲', '2')
        .Replace('۳', '3')
        .Replace('۴', '4')
        .Replace('۵', '5')
        .Replace('۶', '6')
        .Replace('۷', '7')
        .Replace('۸', '8')
        .Replace('۹', '9')
        .Replace('٠', '0')
        .Replace('١', '1')
        .Replace('٢', '2')
        .Replace('٣', '3')
        .Replace('٤', '4')
        .Replace('٥', '5')
        .Replace('٦', '6')
        .Replace('٧', '7')
        .Replace('٨', '8')
        .Replace('٩', '9');

    private static List<string> ParseTags(string rawTags)
    {
        return (rawTags ?? string.Empty)
            .Split([',', '،', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(10)
            .ToList();
    }
}
