using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Exceptions;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class SmsModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public SmsModel(IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _session = session;
    }

    public DatingEvent Event { get; private set; } = new();

    public IReadOnlyList<EventSmsRequest> Requests { get; private set; } = Array.Empty<EventSmsRequest>();

    public EventSmsRequest? SelectedReviewRequest { get; private set; }

    [BindProperty]
    public string NewMessage { get; set; } = string.Empty;

    [BindProperty]
    public string? NewPlannedSendAtLocal { get; set; }

    [BindProperty]
    public long ReviewRequestId { get; set; }

    [BindProperty]
    public string ApprovedMessage { get; set; } = string.Empty;

    [BindProperty]
    public string? ReviewPlannedSendAtLocal { get; set; }

    [BindProperty]
    public string? ReviewNote { get; set; }

    [BindProperty]
    public string RejectNote { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ScheduleFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? RequesterFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchText { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? ReviewId { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;

    public bool IsRtl => _session.IsRtl;

    public SelectList StatusOptions => new(new[]
    {
        new { Value = "", Text = "همه وضعیت ها" },
        new { Value = "pending", Text = "در انتظار تایید" },
        new { Value = "approved", Text = "تایید شده" },
        new { Value = "rejected", Text = "رد شده" }
    }, "Value", "Text", StatusFilter);

    public SelectList ScheduleOptions => new(new[]
    {
        new { Value = "", Text = "همه زمان بندی ها" },
        new { Value = "scheduled", Text = "زمان بندی شده" },
        new { Value = "immediate", Text = "ارسال فوری" }
    }, "Value", "Text", ScheduleFilter);

    public SelectList RequesterOptions { get; private set; } = new(Array.Empty<object>());

    public async Task<IActionResult> OnGetAsync(long eventId)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        await LoadPageAsync(eventId, current);
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync(long eventId)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");

        if (!TryParsePlannedSendAt(NewPlannedSendAtLocal, out var plannedSendAtUtc, out var scheduleError))
        {
            ModelState.AddModelError(nameof(NewPlannedSendAtLocal), scheduleError);
        }

        ValidateMessage(nameof(NewMessage), NewMessage);
        if (!ModelState.IsValid)
        {
            await LoadPageAsync(eventId, current);
            return Page();
        }

        try
        {
            await _eventsApi.RequestSmsAsync(eventId, current, NewMessage.Trim(), plannedSendAtUtc);
            StatusMessage = plannedSendAtUtc.HasValue
                ? "درخواست پیام زمان بندی شده برای بررسی مدیر ثبت شد."
                : "درخواست پیام برای بررسی مدیر ثبت شد.";
            return RedirectToPage(new { eventId });
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ToFriendlySmsMessage(ex.Message));
            await LoadPageAsync(eventId, current);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostApproveAsync(long eventId)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");

        if (!TryParsePlannedSendAt(ReviewPlannedSendAtLocal, out var plannedSendAtUtc, out var scheduleError))
        {
            ModelState.AddModelError(nameof(ReviewPlannedSendAtLocal), scheduleError);
        }

        ValidateMessage(nameof(ApprovedMessage), ApprovedMessage);
        if (ReviewRequestId <= 0)
        {
            ModelState.AddModelError(string.Empty, "درخواست پیامک معتبر انتخاب نشده است.");
        }

        if (!ModelState.IsValid)
        {
            ReviewId = ReviewRequestId;
            await LoadPageAsync(eventId, current);
            return Page();
        }

        try
        {
            await _eventsApi.ApproveSmsRequestAsync(
                eventId,
                ReviewRequestId,
                current,
                ApprovedMessage.Trim(),
                plannedSendAtUtc,
                string.IsNullOrWhiteSpace(ReviewNote) ? null : ReviewNote.Trim());

            StatusMessage = plannedSendAtUtc.HasValue
                ? "پیام تایید شد و برای زمان انتخاب شده وارد صف ارسال شد."
                : "پیام تایید شد و وارد صف ارسال شد.";
            return RedirectToPage(new { eventId });
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ToFriendlySmsMessage(ex.Message));
            ReviewId = ReviewRequestId;
            await LoadPageAsync(eventId, current);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostRejectAsync(long eventId)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");

        if (ReviewRequestId <= 0)
        {
            ModelState.AddModelError(string.Empty, "درخواست پیامک معتبر انتخاب نشده است.");
        }

        if (string.IsNullOrWhiteSpace(RejectNote) || RejectNote.Trim().Length < 3)
        {
            ModelState.AddModelError(nameof(RejectNote), "علت رد باید حداقل 3 کاراکتر باشد.");
        }

        if (!ModelState.IsValid)
        {
            ReviewId = ReviewRequestId;
            await LoadPageAsync(eventId, current);
            return Page();
        }

        try
        {
            await _eventsApi.RejectSmsRequestAsync(eventId, ReviewRequestId, current, RejectNote.Trim());
            StatusMessage = "درخواست پیامک رد شد.";
            return RedirectToPage(new { eventId });
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ToFriendlySmsMessage(ex.Message));
            ReviewId = ReviewRequestId;
            await LoadPageAsync(eventId, current);
            return Page();
        }
    }

    private async Task LoadPageAsync(long eventId, MockUser current)
    {
        Event = await _eventsApi.GetEventAsync(eventId) ?? throw new InvalidOperationException("رویداد مورد نظر پیدا نشد.");

        var requests = await _eventsApi.GetSmsRequestsAsync(eventId, current);
        RequesterOptions = new SelectList(
            new[] { new { Value = "", Text = "همه درخواست دهنده ها" } }
                .Concat(requests
                    .Select(item => item.RequestedByName)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(item => item)
                    .Select(item => new { Value = item, Text = item })),
            "Value",
            "Text",
            RequesterFilter);

        Requests = ApplyFilters(requests);
        SelectedReviewRequest = IsAdmin && ReviewId.HasValue
            ? requests.FirstOrDefault(item => item.Id == ReviewId.Value)
            : null;

        if (SelectedReviewRequest is not null)
        {
            ReviewRequestId = SelectedReviewRequest.Id;
            ApprovedMessage = string.IsNullOrWhiteSpace(ApprovedMessage)
                ? SelectedReviewRequest.EffectiveMessage
                : ApprovedMessage;
            ReviewPlannedSendAtLocal ??= ToDateTimeLocalValue(SelectedReviewRequest.PlannedSendAtUtc);
            ReviewNote ??= SelectedReviewRequest.ReviewNote;
        }
    }

    private IReadOnlyList<EventSmsRequest> ApplyFilters(IReadOnlyList<EventSmsRequest> requests)
    {
        var query = requests.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(StatusFilter))
        {
            query = StatusFilter.Trim().ToLowerInvariant() switch
            {
                "pending" => query.Where(item => item.Status == EventSmsRequestStatus.Pending),
                "approved" => query.Where(item => item.Status == EventSmsRequestStatus.Approved),
                "rejected" => query.Where(item => item.Status == EventSmsRequestStatus.Rejected),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(ScheduleFilter))
        {
            query = ScheduleFilter.Trim().ToLowerInvariant() switch
            {
                "scheduled" => query.Where(item => item.PlannedSendAtUtc.HasValue),
                "immediate" => query.Where(item => !item.PlannedSendAtUtc.HasValue),
                _ => query
            };
        }

        if (!string.IsNullOrWhiteSpace(RequesterFilter))
        {
            query = query.Where(item => string.Equals(item.RequestedByName, RequesterFilter.Trim(), StringComparison.Ordinal));
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var search = SearchText.Trim();
            query = query.Where(item =>
                item.Message.Contains(search, StringComparison.OrdinalIgnoreCase)
                || item.EffectiveMessage.Contains(search, StringComparison.OrdinalIgnoreCase)
                || item.RequestedByName.Contains(search, StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(item.ReviewNote) && item.ReviewNote.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        return query
            .OrderByDescending(item => item.RequestedAtUtc)
            .ToList();
    }

    private void ValidateMessage(string key, string? message)
    {
        var normalized = (message ?? string.Empty).Trim();
        if (normalized.Length is < 5 or > 480)
        {
            ModelState.AddModelError(key, "متن پیام باید بین 5 تا 480 کاراکتر باشد.");
        }
    }

    private static bool TryParsePlannedSendAt(string? localDateTimeText, out DateTimeOffset? plannedSendAtUtc, out string errorMessage)
    {
        plannedSendAtUtc = null;
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(localDateTimeText))
            return true;

        if (!DateTime.TryParse(localDateTimeText.Trim(), out var parsedLocal))
        {
            errorMessage = "زمان برنامه ریزی شده معتبر نیست.";
            return false;
        }

        var localDateTime = DateTime.SpecifyKind(parsedLocal, DateTimeKind.Local);
        var converted = new DateTimeOffset(localDateTime).ToUniversalTime();
        if (converted <= DateTimeOffset.UtcNow)
        {
            errorMessage = "زمان برنامه ریزی شده باید در آینده باشد.";
            return false;
        }

        plannedSendAtUtc = converted;
        return true;
    }

    private static string? ToDateTimeLocalValue(DateTimeOffset? utcValue)
        => utcValue?.ToLocalTime().ToString("yyyy-MM-ddTHH:mm");

    private static string ToFriendlySmsMessage(string message)
    {
        if (message.Contains("Planned SMS send time must be in the future", StringComparison.OrdinalIgnoreCase))
            return "زمان برنامه ریزی شده پیامک باید در آینده باشد.";
        if (message.Contains("Only pending participant SMS requests can be reviewed", StringComparison.OrdinalIgnoreCase))
            return "فقط درخواست های در انتظار تایید قابل بازبینی هستند.";

        return "عملیات پیامک انجام نشد. لطفاً اطلاعات را بازبینی کنید.";
    }
}
