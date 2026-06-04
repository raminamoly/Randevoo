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

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly IEventTagsApiClient _eventTagsApi;
    private readonly ILocationsApiClient _locationsApi;
    private readonly CurrentSessionState _session;

    public IndexModel(IEventsApiClient eventsApi, IEventTagsApiClient eventTagsApi, ILocationsApiClient locationsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _eventTagsApi = eventTagsApi;
        _locationsApi = locationsApi;
        _session = session;
    }

    public IReadOnlyList<DatingEvent> Events { get; private set; } = Array.Empty<DatingEvent>();

    public bool IsRtl => _session.IsRtl;

    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search)
        || TagId is not null
        || !string.IsNullOrWhiteSpace(City)
        || EventModeId is not null
        || Status is not null
        || !string.IsNullOrWhiteSpace(FromDate)
        || !string.IsNullOrWhiteSpace(ToDate)
        || !string.Equals(Sort, "updated-desc", StringComparison.OrdinalIgnoreCase);

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? TagId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? City { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? EventModeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public EventApprovalState? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "updated-desc";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; } = 10;

    public int TotalCount { get; private set; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public SelectList TagOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList CityOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList EventModeOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList StatusOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList SortOptions { get; private set; } = new(Array.Empty<object>());

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await LoadFilterOptionsAsync();

        var events = (await _eventsApi.GetEventsAsync(current)).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var query = Search.Trim();
            events = events.Where(item =>
                item.DisplayTitle.Contains(query, StringComparison.OrdinalIgnoreCase)
                || item.PlannerName.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        if (TagId is long tagId)
            events = events.Where(item => item.ActiveDraft.TagIds.Contains(tagId));

        if (!string.IsNullOrWhiteSpace(City))
            events = events.Where(item => string.Equals(item.ActiveDraft.City, City, StringComparison.OrdinalIgnoreCase));

        if (EventModeId is long eventModeId)
            events = events.Where(item => item.ActiveDraft.EventModeId == eventModeId);

        if (Status is EventApprovalState status)
            events = events.Where(item => item.Status == status);

        if (PersianDateFormatter.TryParseDate(FromDate, IsRtl, out var fromDate))
            events = events.Where(item => item.ActiveDraft.StartAtUtc.Date >= fromDate.UtcDateTime.Date);

        if (PersianDateFormatter.TryParseDate(ToDate, IsRtl, out var toDate))
            events = events.Where(item => item.ActiveDraft.StartAtUtc.Date <= toDate.UtcDateTime.Date);

        events = Sort switch
        {
            "start-asc" => events.OrderBy(item => item.ActiveDraft.StartAtUtc),
            "start-desc" => events.OrderByDescending(item => item.ActiveDraft.StartAtUtc),
            "title-asc" => events.OrderBy(item => item.DisplayTitle),
            "price-desc" => events.OrderByDescending(item => item.ActiveDraft.TicketPrice),
            "price-asc" => events.OrderBy(item => item.ActiveDraft.TicketPrice),
            _ => events.OrderByDescending(item => item.UpdatedAtUtc)
        };

        var filtered = events.ToList();
        TotalCount = filtered.Count;
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Events = filtered
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }

    private async Task LoadFilterOptionsAsync()
    {
        TagOptions = new SelectList(await _eventTagsApi.GetActiveTagsAsync(), "Id", "Name", TagId);
        var cities = (await _locationsApi.GetCitiesAsync())
            .Select(item => item.Name)
            .Distinct()
            .OrderBy(item => item)
            .Select(item => new { Name = item })
            .ToList();
        CityOptions = new SelectList(cities, "Name", "Name", City);
        EventModeOptions = new SelectList(await _eventsApi.GetEventModesAsync(), "Id", "Name", EventModeId);
        StatusOptions = new SelectList(new[]
        {
            new { Value = EventApprovalState.Draft.ToString(), Text = "پیش نویس" },
            new { Value = EventApprovalState.Approved.ToString(), Text = "تایید شده" },
            new { Value = EventApprovalState.PendingAdminReview.ToString(), Text = "در انتظار تایید" },
            new { Value = EventApprovalState.Rejected.ToString(), Text = "رد شده" },
            new { Value = EventApprovalState.Closed.ToString(), Text = "تمام شده" },
            new { Value = EventApprovalState.Cancelled.ToString(), Text = "لغو شده" }
        }, "Value", "Text", Status?.ToString());
        SortOptions = new SelectList(new[]
        {
            new { Value = "updated-desc", Text = "آخرین تغییر" },
            new { Value = "start-desc", Text = "شروع جدیدتر" },
            new { Value = "start-asc", Text = "شروع نزدیک تر" },
            new { Value = "title-asc", Text = "عنوان" },
            new { Value = "price-desc", Text = "قیمت بیشتر" },
            new { Value = "price-asc", Text = "قیمت کمتر" }
        }, "Value", "Text", Sort);
    }

    public string GetStatusClass(EventApprovalState state) => state switch
    {
        EventApprovalState.Approved => "status-approved",
        EventApprovalState.PendingAdminReview => "status-pending",
        EventApprovalState.Rejected => "status-rejected",
        EventApprovalState.Closed => "status-closed",
        EventApprovalState.Cancelled => "status-cancelled",
        _ => "status-draft"
    };
}
