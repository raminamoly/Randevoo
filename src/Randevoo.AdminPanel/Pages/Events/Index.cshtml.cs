using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using DomainEventApprovalStatus = Randevoo.Domain.Enums.EventApprovalStatus;

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

    public bool IsArchive => Scope == EventListScope.Archive;

    public string PageTitle => IsArchive ? "آرشیو و تمام شده" : "فعال و در حال آماده سازی";

    public string PageDescription => IsArchive
        ? "رویدادهای تمام شده یا لغو شده را با فیلتر و صفحه بندی سمت سرور بررسی کنید."
        : "رویدادهای فروش بسته، فروش باز یا در حال آماده‌سازی را با فیلتر و صفحه‌بندی سمت سرور مدیریت کنید.";

    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search)
        || TagId is not null
        || !string.IsNullOrWhiteSpace(City)
        || EventModeId is not null
        || OperationalStatus is not null
        || ApprovalStatus is not null
        || !string.IsNullOrWhiteSpace(FromDate)
        || !string.IsNullOrWhiteSpace(ToDate)
        || !string.Equals(Sort, "updated-desc", StringComparison.OrdinalIgnoreCase);

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public EventListScope Scope { get; set; } = EventListScope.Active;

    [BindProperty(SupportsGet = true)]
    public long? TagId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? City { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? EventModeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public EventOperationalStatus? OperationalStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public DomainEventApprovalStatus? ApprovalStatus { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Sort { get; set; } = "updated-desc";

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; } = 25;

    public int TotalCount { get; private set; }

    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public SelectList TagOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList CityOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList EventModeOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList OperationalStatusOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList ApprovalStatusOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList SortOptions { get; private set; } = new(Array.Empty<object>());

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        NormalizeScopeFilters();
        await LoadFilterOptionsAsync();

        var hasFromDate = PersianDateFormatter.TryParseDate(FromDate, IsRtl, out var fromDate);
        var hasToDate = PersianDateFormatter.TryParseDate(ToDate, IsRtl, out var toDate);

        var result = await _eventsApi.GetEventsPageAsync(current, new EventListFilter
        {
            Search = Search,
            TagId = TagId,
            City = City,
            EventModeId = EventModeId,
            OperationalStatus = OperationalStatus,
            ApprovalStatus = ApprovalStatus,
            FromDateUtc = hasFromDate ? fromDate : null,
            ToDateUtc = hasToDate ? toDate : null,
            Sort = Sort,
            PageNumber = PageNumber,
            PageSize = PageSize,
            Scope = Scope
        });

        TotalCount = result.TotalCount;
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Events = result.Items;
    }

    public async Task<IActionResult> OnGetCancellationPreviewAsync(long id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        var preview = await _eventsApi.PreviewCancellationAsync(id, current);
        return new JsonResult(preview);
    }

    public async Task<IActionResult> OnPostChangeStatusAsync(long id, EventStatusTransitionAction action, string? note, string? publicMessage, bool cancellationConfirmed, string? returnUrl)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");

        try
        {
            if (action == EventStatusTransitionAction.CancelEvent)
            {
                await _eventsApi.CancelEventWithChecklistAsync(
                    id,
                    current,
                    note ?? string.Empty,
                    publicMessage ?? string.Empty,
                    cancellationConfirmed);
            }
            else
            {
                await _eventsApi.ApplyStatusTransitionAsync(id, current, action, note);
            }

            StatusMessage = "وضعیت رویداد با موفقیت تغییر کرد.";
        }
        catch (InvalidOperationException ex)
        {
            ErrorMessage = ex.Message;
        }

        return RedirectToSafeReturnUrl(returnUrl);
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
        var operationalStatuses = IsArchive
            ? new[]
            {
                new { Value = EventOperationalStatus.Completed.ToString(), Text = "تمام شده" },
                new { Value = EventOperationalStatus.Cancelled.ToString(), Text = "لغو شده" }
            }
            : new[]
            {
                new { Value = EventOperationalStatus.SaleClosed.ToString(), Text = "فروش بسته" },
                new { Value = EventOperationalStatus.SaleOpen.ToString(), Text = "فروش باز" }
            };
        OperationalStatusOptions = new SelectList(operationalStatuses, "Value", "Text", OperationalStatus?.ToString());
        ApprovalStatusOptions = new SelectList(new[]
        {
            new { Value = DomainEventApprovalStatus.Draft.ToString(), Text = "پیش‌نویس" },
            new { Value = DomainEventApprovalStatus.PendingReview.ToString(), Text = "در انتظار بررسی مدیر" },
            new { Value = DomainEventApprovalStatus.Approved.ToString(), Text = "تایید شده" }
        }, "Value", "Text", ApprovalStatus?.ToString());
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

    private void NormalizeScopeFilters()
    {
        if (Scope == EventListScope.Archive)
        {
            if (OperationalStatus is EventOperationalStatus.SaleClosed or EventOperationalStatus.SaleOpen)
                OperationalStatus = null;
            return;
        }

        if (OperationalStatus is EventOperationalStatus.Completed or EventOperationalStatus.Cancelled)
            OperationalStatus = null;
    }

    public string GetOperationalStatusClass(EventOperationalStatus status) => DisplayFormatter.OperationalStatusClass(status);

    public string GetProfileStatusClass(DomainEventApprovalStatus status) => DisplayFormatter.ApprovalStatusClass(status);

    public EventStatusTransitionModalViewModel CreateStatusTransitionModal(DatingEvent datingEvent)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        return new EventStatusTransitionModalViewModel
        {
            Event = datingEvent,
            Options = EventStatusTransitionCatalog.GetOptions(datingEvent, current.Role),
            EmptyMessage = EventStatusTransitionCatalog.GetEmptyMessage(datingEvent),
            ReturnUrl = Request.Path + Request.QueryString,
            CancellationPreviewUrl = Url.Page(null, "CancellationPreview", new { id = datingEvent.Id }) ?? string.Empty
        };
    }

    private IActionResult RedirectToSafeReturnUrl(string? returnUrl)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToPage();
    }
}
