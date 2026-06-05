using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.DiscountCodes;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.DiscountCodes;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IEventDiscountCodesApiClient _discountCodesApi;
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public IndexModel(IEventDiscountCodesApiClient discountCodesApi, IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _discountCodesApi = discountCodesApi;
        _eventsApi = eventsApi;
        _session = session;
    }

    [BindProperty]
    public EventDiscountCodeEditorInput Input { get; set; } = new();

    [BindProperty]
    public string StartsAtText { get; set; } = string.Empty;

    [BindProperty]
    public string EndsAtText { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public long? Id { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? EventId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? UsageId { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public IReadOnlyList<EventDiscountCodeAdminItem> DiscountCodes { get; private set; } = Array.Empty<EventDiscountCodeAdminItem>();

    public EventDiscountCodeAdminItem? UsageDiscountCode { get; private set; }

    public IReadOnlyList<EventDiscountCodeUsageItem> UsageItems { get; private set; } = Array.Empty<EventDiscountCodeUsageItem>();

    public SelectList EventOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList EventFilterOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList GenderOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList DiscountTypeOptions { get; private set; } = new(Array.Empty<object>());

    public bool IsEditing => Id is not null;

    public bool IsRtl => _session.IsRtl;

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadAsync();

        if (Id is long discountCodeId)
        {
            var discountCode = await _discountCodesApi.GetDiscountCodeAsync(discountCodeId);
            if (discountCode is null)
                return NotFound();

            Input = new EventDiscountCodeEditorInput
            {
                DatingEventId = discountCode.DatingEventId,
                Code = discountCode.Code,
                Title = discountCode.Title,
                Description = discountCode.Description,
                GenderScope = discountCode.GenderScope,
                DiscountType = discountCode.DiscountType,
                Value = discountCode.Value,
                StartsAtUtc = discountCode.StartsAtUtc,
                EndsAtUtc = discountCode.EndsAtUtc,
                MaxUsageCount = discountCode.MaxUsageCount,
                IsActive = discountCode.IsActive
            };
        }
        else if (EventId is long eventId)
        {
            Input.DatingEventId = eventId;
        }

        SyncDateTextFromInput();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        ApplyDateTextToInput();
        ValidateInput();
        if (!ModelState.IsValid)
        {
            await LoadAsync();
            SyncDateTextFromInput();
            return Page();
        }

        try
        {
            var saved = await _discountCodesApi.UpsertDiscountCodeAsync(Input, current, Id);
            StatusMessage = Id is null
                ? $"کد تخفیف «{saved.Code}» ایجاد شد."
                : $"کد تخفیف «{saved.Code}» به روز شد.";
            return RedirectToPage("/DiscountCodes/Index", new { eventId = saved.DatingEventId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadAsync();
            SyncDateTextFromInput();
            return Page();
        }
    }

    public async Task<IActionResult> OnPostToggleAsync(long id, bool isActive)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        try
        {
            await _discountCodesApi.SetDiscountCodeActiveAsync(id, current, isActive);
            StatusMessage = isActive ? "کد تخفیف فعال شد." : "کد تخفیف غیرفعال شد.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/DiscountCodes/Index", new { eventId = EventId, search = Search });
    }

    private async Task LoadAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        var events = await _eventsApi.GetEventsAsync(current);
        var eventItems = events
            .OrderBy(item => item.DisplayTitle)
            .Select(item => new { item.Id, item.DisplayTitle })
            .ToList();

        EventOptions = new SelectList(eventItems, "Id", "DisplayTitle", Input.DatingEventId);
        EventFilterOptions = new SelectList(eventItems, "Id", "DisplayTitle", EventId);

        GenderOptions = new SelectList(new[]
        {
            new { Value = EventDiscountGenderScope.All, Text = "همه" },
            new { Value = EventDiscountGenderScope.Male, Text = "فقط آقایان" },
            new { Value = EventDiscountGenderScope.Female, Text = "فقط خانم ها" }
        }, "Value", "Text", Input.GenderScope);

        DiscountTypeOptions = new SelectList(new[]
        {
            new { Value = EventDiscountType.Percentage, Text = "درصدی" },
            new { Value = EventDiscountType.FixedAmount, Text = "مبلغ ثابت" }
        }, "Value", "Text", Input.DiscountType);

        var items = await _discountCodesApi.GetDiscountCodesAsync();
        if (EventId is long eventId)
            items = items.Where(item => item.DatingEventId == eventId).ToList();

        if (!string.IsNullOrWhiteSpace(Search))
        {
            var query = Search.Trim();
            items = items.Where(item =>
                item.Code.Contains(query, StringComparison.OrdinalIgnoreCase)
                || item.EventTitle.Contains(query, StringComparison.OrdinalIgnoreCase)
                || (item.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();
        }

        DiscountCodes = items
            .OrderByDescending(item => item.StartsAtUtc)
            .ThenBy(item => item.Code)
            .ToList();

        if (UsageId is long usageId)
        {
            UsageDiscountCode = items.FirstOrDefault(item => item.Id == usageId)
                ?? await _discountCodesApi.GetDiscountCodeAsync(usageId);
            UsageItems = await _discountCodesApi.GetDiscountCodeUsageAsync(usageId);
        }
    }

    private void ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(Input.Code))
            ModelState.AddModelError(nameof(Input.Code), "کد تخفیف را وارد کنید.");

        if (Input.EndsAtUtc <= Input.StartsAtUtc)
            ModelState.AddModelError(nameof(Input.EndsAtUtc), "زمان پایان باید بعد از زمان شروع باشد.");

        if (Input.MaxUsageCount <= 0)
            ModelState.AddModelError(nameof(Input.MaxUsageCount), "حداکثر تعداد استفاده باید بیشتر از صفر باشد.");

        if (Input.DiscountType == EventDiscountType.Percentage)
        {
            if (Input.Value is <= 0 or > 100)
                ModelState.AddModelError(nameof(Input.Value), "درصد تخفیف باید بین 1 تا 100 باشد.");
        }
        else if (Input.Value <= 0)
        {
            ModelState.AddModelError(nameof(Input.Value), "مبلغ تخفیف باید بیشتر از صفر باشد.");
        }
    }

    private void ApplyDateTextToInput()
    {
        if (!string.IsNullOrWhiteSpace(StartsAtText))
            Input.StartsAtUtc = PersianDateFormatter.Parse(StartsAtText).UtcDateTime;

        if (!string.IsNullOrWhiteSpace(EndsAtText))
            Input.EndsAtUtc = PersianDateFormatter.Parse(EndsAtText).UtcDateTime;
    }

    private void SyncDateTextFromInput()
    {
        StartsAtText = PersianDateFormatter.Format(Input.StartsAtUtc, IsRtl);
        EndsAtText = PersianDateFormatter.Format(Input.EndsAtUtc, IsRtl);
    }
}
