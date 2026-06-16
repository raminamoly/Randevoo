using System.Globalization;
using System.Buffers.Binary;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using AdminEventOperationalStatus = Randevoo.AdminPanel.Models.Events.EventOperationalStatus;
using DomainEventApprovalStatus = Randevoo.Domain.Enums.EventApprovalStatus;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class EditModel : PageModel
{
    public const int MinEventImageWidth = 1000;
    public const int MinEventImageHeight = 625;
    public const int EventImageAspectWidth = 16;
    public const int EventImageAspectHeight = 10;
    public const long MaxEventImageBytes = 5 * 1024 * 1024;
    public const int MinGenderCapacity = 1;
    public const int MaxGenderCapacity = 500;
    public const int MinLikeLimit = 0;
    public const int MaxLikeLimit = 10;
    public const decimal MinTicketValueIrr = 100_000m;
    public const decimal MaxTicketValueIrr = 200_000_000m;

    private readonly IEventsApiClient _eventsApi;
    private readonly IEventTagsApiClient _eventTagsApi;
    private readonly IUsersApiClient _usersApi;
    private readonly IFinanceApiClient _financeApi;
    private readonly ILocationsApiClient _locationsApi;
    private readonly CurrentSessionState _session;
    private readonly IWebHostEnvironment _environment;

    public EditModel(IEventsApiClient eventsApi, IEventTagsApiClient eventTagsApi, IUsersApiClient usersApi, IFinanceApiClient financeApi, ILocationsApiClient locationsApi, CurrentSessionState session, IWebHostEnvironment environment)
    {
        _eventsApi = eventsApi;
        _eventTagsApi = eventTagsApi;
        _usersApi = usersApi;
        _financeApi = financeApi;
        _locationsApi = locationsApi;
        _session = session;
        _environment = environment;
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
    public IFormFile? Image1File { get; set; }

    [BindProperty]
    public IFormFile? Image2File { get; set; }

    [BindProperty]
    public IFormFile? Image3File { get; set; }

    [BindProperty]
    public long? AssignedPlannerId { get; set; }

    [BindProperty]
    public string SubmitAction { get; set; } = "draft";

    [BindProperty(SupportsGet = true)]
    public long? ExistingEventId { get; set; }

    public bool IsNew => ExistingEventId is null;

    public bool IsRtl => _session.IsRtl;

    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;

    public bool IsPlanner => _session.CurrentUser?.Role == AdminRole.EventPlanner;

    public bool IsEventCurrencyLocked { get; private set; }

    public string? ReviewNote { get; set; }

    public string StatusText { get; set; } = AdminEventOperationalStatus.SaleClosed.ToString();

    public AdminEventOperationalStatus StatusValue { get; set; } = AdminEventOperationalStatus.SaleClosed;

    public string StatusClass { get; set; } = "status-draft";

    public DomainEventApprovalStatus ProfileStatusValue { get; set; } = DomainEventApprovalStatus.Draft;

    public string ProfileStatusClass { get; set; } = "status-draft";

    public SelectList CountryOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList CityOptions { get; private set; } = new(Array.Empty<object>());

    public string CityOptionsJson { get; private set; } = "[]";

    public string CurrencyRatesJson { get; private set; } = "{}";

    private IReadOnlyList<CountryOption> Countries { get; set; } = Array.Empty<CountryOption>();

    private IReadOnlyList<CityOption> Cities { get; set; } = Array.Empty<CityOption>();

    private IReadOnlyList<EducationLevelOption> EducationLevels { get; set; } = Array.Empty<EducationLevelOption>();

    public SelectList AgeRangeOptions => new(new[] { "20-30", "25-35", "30-40", "35-45" });

    public SelectList EventTypeOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList EventModeOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList OnlinePlatformOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList MinimumEducationLevelOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList CurrencyOptions { get; private set; } = new(Array.Empty<object>());

    public MultiSelectList TagOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList PlannerOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList OrganizerPaymentAccountOptions { get; private set; } = new(Array.Empty<object>());

    public string OrganizerPaymentAccountOptionsJson { get; private set; } = "[]";

    public string? OrganizerPaymentAccountWarning { get; private set; }

    public IReadOnlyList<EventChangeLogEntry> ProfileReviewHistoryEntries { get; private set; } = Array.Empty<EventChangeLogEntry>();

    public EventProfileReviewHistoryModalViewModel ProfileReviewHistoryModal => new()
    {
        EventId = ExistingEventId ?? 0,
        EventTitle = Input.Title,
        Entries = ProfileReviewHistoryEntries
    };

    private List<long> PlannerIds { get; set; } = new();

    private IReadOnlyList<PlannerBankAccountItem> OrganizerPaymentAccounts { get; set; } = Array.Empty<PlannerBankAccountItem>();

    private IReadOnlyList<SystemLookupOption> CurrencyLookupOptions { get; set; } = Array.Empty<SystemLookupOption>();

    public async Task<IActionResult> OnGetAsync(long? id)
    {
        ExistingEventId = ResolveExistingEventId(id);
        await LoadLookupOptionsAsync();

        if (ExistingEventId is long eventId)
        {
            var @event = await _eventsApi.GetEventAsync(eventId);
            if (@event is null)
            {
                return NotFound();
            }

            Input = @event.ActiveDraft;
            ReviewNote = @event.AdminReviewNote;
            StatusText = @event.OperationalStatus.ToString();
            StatusValue = @event.OperationalStatus;
            StatusClass = GetOperationalStatusClass(@event.OperationalStatus);
            ProfileStatusValue = @event.ApprovalStatus;
            ProfileStatusClass = GetProfileStatusClass(@event.ApprovalStatus);
            ProfileReviewHistoryEntries = ExtractProfileReviewHistory(@event);
            AssignedPlannerId = @event.PlannerUserId;
            IsEventCurrencyLocked = @event.IsCurrencyLocked;
        }
        else
        {
            if (_session.CurrentUser is null)
            {
                return RedirectToPage("/Account/Login");
            }

            Input = new EventDraftInput();
            if (EventTypeOptions.Items.Cast<object>().Any() && Input.EventTypeId == 0)
            {
                Input.EventTypeId = long.Parse(EventTypeOptions.First().Value!);
            }
            StatusText = AdminEventOperationalStatus.SaleClosed.ToString();
            StatusValue = AdminEventOperationalStatus.SaleClosed;
            StatusClass = GetOperationalStatusClass(AdminEventOperationalStatus.SaleClosed);
            ProfileStatusValue = DomainEventApprovalStatus.Draft;
            ProfileStatusClass = GetProfileStatusClass(DomainEventApprovalStatus.Draft);
            IsEventCurrencyLocked = false;
            if (IsAdmin)
            {
                AssignedPlannerId = GetDefaultPlannerId();
            }
        }

        await LoadLookupOptionsAsync();
        SyncFormTextFromInput();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(long? id)
    {
        ExistingEventId = ResolveExistingEventId(id);
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        await LoadLookupOptionsAsync();

        ApplyEventContextDefaults();
        await ApplyExistingCurrencyLockAsync();

        if (!TryCombineDateAndTime(StartDateText, StartTimeText, _session.IsRtl, out var startAtUtc, out var startError))
        {
            ModelState.AddModelError(nameof(StartDateText), startError);
        }
        else
        {
            ModelState.Remove(nameof(StartDateText));
            ModelState.Remove(nameof(StartTimeText));
            Input.StartAtUtc = startAtUtc;
        }

        if (!TryCombineDateAndTime(EndDateText, EndTimeText, _session.IsRtl, out var endAtUtc, out var endError))
        {
            ModelState.AddModelError(nameof(EndDateText), endError);
        }
        else
        {
            ModelState.Remove(nameof(EndDateText));
            ModelState.Remove(nameof(EndTimeText));
            Input.EndAtUtc = endAtUtc;
        }

        var image1Upload = ValidateEventImage(Image1File, nameof(Image1File), "تصویر اول");
        var image2Upload = ValidateEventImage(Image2File, nameof(Image2File), "تصویر دوم");
        var image3Upload = ValidateEventImage(Image3File, nameof(Image3File), "تصویر سوم");

        if (current.Role == AdminRole.EventPlanner && ExistingEventId is long editId)
        {
            var existing = await _eventsApi.GetEventAsync(editId);
            if (existing is not null)
            {
                Input.OrganizerCommissionPercent = existing.ActiveDraft.OrganizerCommissionPercent;
                Input.Faqs = existing.ActiveDraft.Faqs;
                AssignedPlannerId = existing.PlannerUserId;
            }
        }
        else if (ExistingEventId is long adminEditId)
        {
            var existing = await _eventsApi.GetEventAsync(adminEditId);
            if (existing is not null)
            {
                Input.Faqs = existing.ActiveDraft.Faqs;
            }
        }

        if (IsAdmin && AssignedPlannerId is null)
        {
            ModelState.AddModelError(nameof(AssignedPlannerId), "انتخاب برگزارکننده برای مدیر الزامی است.");
        }

        ValidateEventInput();
        if (!ModelState.IsValid)
        {
            await LoadExistingEventStatusAsync();
            return Page();
        }

        if (image1Upload is not null)
        {
            Input.Image1 = await SaveEventImageAsync(image1Upload, HttpContext.RequestAborted);
        }

        if (image2Upload is not null)
        {
            Input.Image2 = await SaveEventImageAsync(image2Upload, HttpContext.RequestAborted);
        }

        if (image3Upload is not null)
        {
            Input.Image3 = await SaveEventImageAsync(image3Upload, HttpContext.RequestAborted);
        }

        try
        {
            var submitForReview = string.Equals(SubmitAction, "submit", StringComparison.OrdinalIgnoreCase);
            var saved = await _eventsApi.SaveEventAsync(Input, current, ExistingEventId, AssignedPlannerId, submitForReview);
            return RedirectToPage("/Events/Details", new { id = saved.Id });
        }
        catch (BusinessRuleViolationException ex)
        {
            ModelState.AddModelError(string.Empty, ToFriendlyValidationMessage(ex.Message));
            await LoadExistingEventStatusAsync();
            return Page();
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ToFriendlyValidationMessage(ex.Message));
            await LoadExistingEventStatusAsync();
            return Page();
        }
    }

    private async Task LoadLookupOptionsAsync()
    {
        var planners = (await _usersApi.GetUsersAsync())
            .Where(user => user.Role == AdminRole.EventPlanner && user.IsActive)
            .OrderBy(user => user.FullName)
            .Select(user => new
            {
                Value = user.Id,
                Text = user.FullName
            })
            .ToList();

        PlannerIds = planners.Select(item => item.Value).ToList();
        PlannerOptions = new SelectList(planners, "Value", "Text");

        var eventTypes = await _eventsApi.GetEventTypesAsync();
        EventTypeOptions = new SelectList(eventTypes, "Id", "Name");
        var eventModes = await _eventsApi.GetEventModesAsync();
        EventModeOptions = new SelectList(eventModes, "Id", "Name", Input.EventModeId <= 0 ? 2L : Input.EventModeId);
        var onlinePlatforms = await _eventsApi.GetOnlineEventPlatformsAsync();
        OnlinePlatformOptions = new SelectList(onlinePlatforms, "Id", "Name", Input.OnlineEventPlatformId);
        var tagOptions = await _eventTagsApi.GetActiveTagsAsync();
        TagOptions = new MultiSelectList(tagOptions, "Id", "Name", Input.TagIds);
        CurrencyLookupOptions = await _eventsApi.GetCurrencyOptionsAsync();
        Input.MaleTicketCurrencyCode = NormalizeCurrencyCodeForForm(Input.MaleTicketCurrencyCode);
        Input.FemaleTicketCurrencyCode = NormalizeCurrencyCodeForForm(Input.FemaleTicketCurrencyCode);
        SyncSharedTicketCurrency();
        CurrencyOptions = new SelectList(
            CurrencyLookupOptions.Select(item => new { Code = item.Name, Title = $"{item.DisplayNameFa} ({item.Name})" }),
            "Code",
            "Title");
        await LoadOrganizerPaymentAccountOptionsAsync();
        CurrencyRatesJson = JsonSerializer.Serialize(CurrencyLookupOptions.ToDictionary(
            item => item.Name,
            item => new
            {
                item.DisplayNameFa,
                item.Symbol,
                item.DecimalPlaces,
                RateToIrr = item.ExchangeRateToIrr ?? 1m,
                item.ExchangeRateEffectiveFromUtc
            },
            StringComparer.OrdinalIgnoreCase));

        Countries = await _locationsApi.GetCountriesAsync(includeInactive: true);
        Cities = await _locationsApi.GetCitiesAsync(includeInactive: true);
        EducationLevels = await _locationsApi.GetEducationLevelsAsync();

        var activeCountries = Countries.Where(country => country.IsActive).ToList();
        if (string.IsNullOrWhiteSpace(Input.Country) || Countries.All(country => country.Name != Input.Country))
        {
            Input.Country = activeCountries.FirstOrDefault()?.Name ?? Countries.FirstOrDefault()?.Name ?? "ایران";
        }

        var activeCitiesForCountry = Cities.Where(city => city.CountryName == Input.Country && city.IsActive).ToList();
        if (string.IsNullOrWhiteSpace(Input.City) || Cities.All(city => city.CountryName != Input.Country || city.Name != Input.City))
        {
            Input.City = activeCitiesForCountry.FirstOrDefault()?.Name
                ?? Cities.FirstOrDefault(city => city.CountryName == Input.Country)?.Name
                ?? string.Empty;
        }

        var countryOptions = Countries
            .Where(country => country.IsActive || string.Equals(country.Name, Input.Country, StringComparison.OrdinalIgnoreCase))
            .Select(country => new
            {
                Value = country.Name,
                Text = country.IsActive ? country.Name : $"{country.Name} (غیرفعال)"
            })
            .ToList();
        var cityOptions = Cities
            .Where(city => city.CountryName == Input.Country
                && (city.IsActive || string.Equals(city.Name, Input.City, StringComparison.OrdinalIgnoreCase)))
            .Select(city => new
            {
                Value = city.Name,
                Text = city.IsActive ? city.Name : $"{city.Name} (غیرفعال)"
            })
            .ToList();
        CountryOptions = new SelectList(countryOptions, "Value", "Text", Input.Country);
        CityOptions = new SelectList(cityOptions, "Value", "Text", Input.City);
        Input.MinimumEducationLevelId ??= MapRestrictionToEducationLevelId(Input.EducationLevelRestriction);
        var minimumEducationOptions = new[] { new { Id = string.Empty, Title = "بدون محدودیت" } }
            .Concat(EducationLevels
                .Where(level => level.Rank > 0)
                .Select(level => new { Id = level.Id.ToString(), level.Title }))
            .ToList();
        MinimumEducationLevelOptions = new SelectList(minimumEducationOptions, "Id", "Title", Input.MinimumEducationLevelId?.ToString() ?? string.Empty);
        CityOptionsJson = JsonSerializer.Serialize(Cities
            .Where(city => city.IsActive
                || (string.Equals(city.CountryName, Input.Country, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(city.Name, Input.City, StringComparison.OrdinalIgnoreCase)))
            .Select(city => new
        {
            city.CountryName,
            city.Name,
            city.Latitude,
            city.Longitude,
            city.IsActive
        }));
    }

    private async Task LoadOrganizerPaymentAccountOptionsAsync()
    {
        OrganizerPaymentAccountWarning = null;
        OrganizerPaymentAccounts = Array.Empty<PlannerBankAccountItem>();
        OrganizerPaymentAccountOptions = new SelectList(Array.Empty<object>(), "Value", "Text");
        OrganizerPaymentAccountOptionsJson = "[]";

        var current = _session.CurrentUser;
        if (current is null)
            return;

        var plannerUserId = ResolvePlannerUserIdForPaymentAccounts(current);
        if (plannerUserId is null)
        {
            OrganizerPaymentAccountWarning = "برای نمایش حساب‌های دریافت وجه، ابتدا برگزارکننده را انتخاب کنید.";
            return;
        }

        OrganizerPaymentAccounts = await _financeApi.GetPlannerBankAccountsAsync(current, plannerUserId.Value);
        var eventCurrencyCode = NormalizeCurrencyCodeForForm(Input.MaleTicketCurrencyCode);
        var matchingAccounts = OrganizerPaymentAccounts
            .Where(account => account.IsActive && string.Equals(account.CurrencyCode, eventCurrencyCode, StringComparison.OrdinalIgnoreCase))
            .Select(account => new
            {
                Value = account.Id,
                Text = FormatPlannerBankAccountOption(account)
            })
            .ToList();

        OrganizerPaymentAccountOptions = new SelectList(matchingAccounts, "Value", "Text", Input.OrganizerPaymentAccountId);
        OrganizerPaymentAccountOptionsJson = JsonSerializer.Serialize(OrganizerPaymentAccounts.Select(account => new
        {
            account.Id,
            account.CurrencyCode,
            account.IsActive,
            Label = FormatPlannerBankAccountOption(account)
        }));

        if (matchingAccounts.Count == 0)
        {
            OrganizerPaymentAccountWarning = $"حساب فعال {eventCurrencyCode} برای این برگزارکننده ثبت نشده است. ابتدا حساب را در پروفایل برگزارکننده ثبت و فعال کنید.";
        }
    }

    private long? ResolveExistingEventId(long? handlerId)
    {
        if (handlerId is > 0)
            return handlerId;

        if (ExistingEventId is > 0)
            return ExistingEventId;

        if (TryReadEventId(RouteData.Values["id"], out var routeId))
            return routeId;

        if (TryReadEventId(Request.Query["id"].ToString(), out var queryId))
            return queryId;

        if (Request.HasFormContentType
            && TryReadEventId(Request.Form[nameof(ExistingEventId)].ToString(), out var formId))
        {
            return formId;
        }

        return null;
    }

    private static bool TryReadEventId(object? value, out long eventId)
    {
        eventId = 0;
        var text = NormalizeNumericText(Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty);
        return long.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out eventId) && eventId > 0;
    }

    private long? GetDefaultPlannerId() => PlannerIds.Count == 0 ? null : PlannerIds[0];

    private long? ResolvePlannerUserIdForPaymentAccounts(MockUser current)
    {
        if (current.Role == AdminRole.EventPlanner)
            return current.Id;

        return AssignedPlannerId;
    }

    private static string FormatPlannerBankAccountOption(PlannerBankAccountItem account)
    {
        var method = DisplayFormatter.PayoutMethod(account.PayoutMethod);
        var identity = account.CurrencyCode == "IRR"
            ? FirstNonEmpty(account.BankName, account.Iban, account.CardNumber, account.AccountNumber)
            : FirstNonEmpty(account.BankName, account.Iban, account.SwiftCode, account.AccountIdentifier, account.PublicPaymentInstructions);
        return string.Join(" - ", new[] { account.CurrencyCode, method, identity }.Where(item => !string.IsNullOrWhiteSpace(item)));
    }

    private static string? FirstNonEmpty(params string?[] values)
        => values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));

    private async Task ApplyExistingCurrencyLockAsync()
    {
        if (ExistingEventId is not long eventId)
            return;

        var existing = await _eventsApi.GetEventAsync(eventId);
        if (existing?.IsCurrencyLocked != true)
            return;

        IsEventCurrencyLocked = true;
        Input.MaleTicketCurrencyCode = NormalizeCurrencyCodeForForm(existing.ActiveDraft.MaleTicketCurrencyCode);
        Input.FemaleTicketCurrencyCode = Input.MaleTicketCurrencyCode;
        ModelState.Remove($"{nameof(Input)}.{nameof(EventDraftInput.MaleTicketCurrencyCode)}");
        ModelState.Remove($"{nameof(Input)}.{nameof(EventDraftInput.FemaleTicketCurrencyCode)}");
    }

    private void SyncFormTextFromInput()
    {
        StartDateText = FormatDateInput(Input.StartAtUtc);
        StartTimeText = FormatTimeInput(Input.StartAtUtc);
        EndDateText = FormatDateInput(Input.EndAtUtc);
        EndTimeText = FormatTimeInput(Input.EndAtUtc);
    }

    private void ApplyEventContextDefaults()
    {
        StatusText = IsNew ? AdminEventOperationalStatus.SaleClosed.ToString() : StatusText;
        StatusValue = IsNew ? AdminEventOperationalStatus.SaleClosed : StatusValue;
        StatusClass = GetOperationalStatusClass(StatusValue);
        ProfileStatusValue = IsNew ? DomainEventApprovalStatus.Draft : ProfileStatusValue;
        ProfileStatusClass = GetProfileStatusClass(ProfileStatusValue);
        ReviewNote ??= null;
    }

    private async Task LoadExistingEventStatusAsync()
    {
        if (ExistingEventId is not long id)
        {
            StatusText = AdminEventOperationalStatus.SaleClosed.ToString();
            StatusValue = AdminEventOperationalStatus.SaleClosed;
            StatusClass = GetOperationalStatusClass(StatusValue);
            ProfileStatusValue = DomainEventApprovalStatus.Draft;
            ProfileStatusClass = GetProfileStatusClass(ProfileStatusValue);
            return;
        }

        var existing = await _eventsApi.GetEventAsync(id);
        if (existing is null)
            return;

        ReviewNote = existing.AdminReviewNote;
        StatusText = existing.OperationalStatus.ToString();
        StatusValue = existing.OperationalStatus;
        StatusClass = GetOperationalStatusClass(existing.OperationalStatus);
        ProfileStatusValue = existing.ApprovalStatus;
        ProfileStatusClass = GetProfileStatusClass(existing.ApprovalStatus);
        ProfileReviewHistoryEntries = ExtractProfileReviewHistory(existing);
        IsEventCurrencyLocked = existing.IsCurrencyLocked;
    }

    private static IReadOnlyList<EventChangeLogEntry> ExtractProfileReviewHistory(DatingEvent datingEvent)
    {
        return datingEvent.ChangeLog
            .Where(item => string.Equals(item.Category, "review", StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(item => item.CreatedAtUtc)
            .ToList();
    }

    private void ValidateEventInput()
    {
        ValidateRequiredText(InputKey(nameof(EventDraftInput.Title)), Input.Title, "عنوان رویداد", 2, 150);
        ValidateRequiredText(InputKey(nameof(EventDraftInput.DescriptionHtml)), StripHtml(Input.DescriptionHtml), "توضیحات رویداد", 10, 10000);

        if (Input.EventTypeId <= 0)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.EventTypeId)), "نوع رویداد را انتخاب کنید.");

        if (Input.EventModeId <= 0)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.EventModeId)), "نحوه برگزاری را انتخاب کنید.");

        if (Input.IsOnline)
        {
            if (Input.OnlineEventPlatformId is null or <= 0)
                ModelState.AddModelError(InputKey(nameof(EventDraftInput.OnlineEventPlatformId)), "پلتفرم آنلاین را انتخاب کنید.");

            if (string.IsNullOrWhiteSpace(Input.OnlineJoinUrl))
                ModelState.AddModelError(InputKey(nameof(EventDraftInput.OnlineJoinUrl)), "لینک ورود رویداد آنلاین را وارد کنید.");
        }
        else
        {
            ValidateRequiredText(InputKey(nameof(EventDraftInput.Address)), Input.Address, "آدرس", 5, 300);
        }

        Input.EducationLevelRestriction = MapEducationLevelIdToRestriction(Input.MinimumEducationLevelId);

        if (Input.MinimumEducationLevelId is long educationLevelId && EducationLevels.All(level => level.Id != educationLevelId || level.Rank <= 0))
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.MinimumEducationLevelId)), "حداقل سطح تحصیل معتبر نیست.");

        if (!Input.IsOnline)
        {
            if (Countries.All(country => country.Name != Input.Country))
                ModelState.AddModelError(InputKey(nameof(EventDraftInput.Country)), "کشور انتخاب شده معتبر نیست.");

            if (Cities.All(city => city.CountryName != Input.Country || city.Name != Input.City))
                ModelState.AddModelError(InputKey(nameof(EventDraftInput.City)), "شهر انتخاب شده برای این کشور معتبر نیست.");
        }

        if (Input.StartAtUtc != default
            && Input.EndAtUtc != default
            && Input.EndAtUtc <= Input.StartAtUtc)
        {
            ModelState.AddModelError(nameof(EndDateText), "زمان پایان باید بعد از زمان شروع باشد.");
        }

        Input.MaleTicketCurrencyCode = NormalizeCurrencyCodeForForm(Input.MaleTicketCurrencyCode);
        Input.FemaleTicketCurrencyCode = NormalizeCurrencyCodeForForm(Input.FemaleTicketCurrencyCode);
        SyncSharedTicketCurrency();

        ValidateTicketPrice(InputKey(nameof(EventDraftInput.MaleTicketPrice)), Input.MaleTicketPrice, "مبلغ بلیت آقایان");
        ValidateTicketPrice(InputKey(nameof(EventDraftInput.FemaleTicketPrice)), Input.FemaleTicketPrice, "مبلغ بلیت خانم‌ها");

        if (CurrencyLookupOptions.All(item => item.Name != Input.MaleTicketCurrencyCode))
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.MaleTicketCurrencyCode)), "واحد پول بلیت آقایان معتبر نیست.");

        if (CurrencyLookupOptions.All(item => item.Name != Input.FemaleTicketCurrencyCode))
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.FemaleTicketCurrencyCode)), "واحد پول بلیت خانم‌ها معتبر نیست.");

        if (Input.OrganizerCommissionPercent is < 0 or > 100)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.OrganizerCommissionPercent)), "درصد کمیسیون باید بین 0 تا 100 باشد.");

        if (!Enum.IsDefined(Input.PaymentCollectionMethod))
            ModelState.AddModelError($"{nameof(Input)}.{nameof(EventDraftInput.PaymentCollectionMethod)}", "روش دریافت هزینه رویداد معتبر نیست.");

        if (Input.PaymentCollectionMethod == EventPaymentCollectionMethod.OrganizerManualTransfer)
        {
            ValidateOrganizerPaymentAccount();
        }
        else
        {
            Input.OrganizerPaymentInstructions = null;
            Input.OrganizerPaymentAccountId = null;
            ModelState.Remove($"{nameof(Input)}.{nameof(EventDraftInput.OrganizerPaymentInstructions)}");
            ModelState.Remove($"{nameof(Input)}.{nameof(EventDraftInput.OrganizerPaymentAccountId)}");
        }

        if (Input.CapacityMale is < MinGenderCapacity or > MaxGenderCapacity)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.CapacityMale)), $"ظرفیت آقایان باید بین {MinGenderCapacity} تا {MaxGenderCapacity} نفر باشد.");

        if (Input.CapacityFemale is < MinGenderCapacity or > MaxGenderCapacity)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.CapacityFemale)), $"ظرفیت بانوان باید بین {MinGenderCapacity} تا {MaxGenderCapacity} نفر باشد.");

        if (Input.LikeLimit is < MinLikeLimit or > MaxLikeLimit)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.LikeLimit)), $"تعداد لایک مجاز باید بین {MinLikeLimit} تا {MaxLikeLimit} باشد.");

        ValidateAgeRange(InputKey(nameof(EventDraftInput.AgeRangeForMale)), Input.AgeRangeForMale, "بازه سنی آقایان");
        ValidateAgeRange(InputKey(nameof(EventDraftInput.AgeRangeForFemale)), Input.AgeRangeForFemale, "بازه سنی بانوان");

        if (Input.TagIds.Count > 10)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.TagIds)), "برای هر رویداد حداکثر 10 تگ می توانید انتخاب کنید.");

        Input.TagIds = Input.TagIds.Distinct().ToList();
        ValidateFaqs();
    }

    private void ValidateOrganizerPaymentAccount()
    {
        if (Input.OrganizerPaymentAccountId is null or <= 0)
        {
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.OrganizerPaymentAccountId)), "حساب دریافت وجه برگزارکننده را انتخاب کنید.");
            return;
        }

        var current = _session.CurrentUser;
        var plannerUserId = current is null ? null : ResolvePlannerUserIdForPaymentAccounts(current);
        var account = OrganizerPaymentAccounts.FirstOrDefault(item => item.Id == Input.OrganizerPaymentAccountId.Value);
        if (account is null)
        {
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.OrganizerPaymentAccountId)), "حساب دریافت وجه انتخاب‌شده پیدا نشد.");
            return;
        }

        if (plannerUserId is long expectedPlannerUserId && account.UserId != expectedPlannerUserId)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.OrganizerPaymentAccountId)), "حساب دریافت وجه انتخاب‌شده متعلق به برگزارکننده این رویداد نیست.");

        if (!account.IsActive)
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.OrganizerPaymentAccountId)), "حساب دریافت وجه انتخاب‌شده فعال نیست.");

        if (!string.Equals(account.CurrencyCode, Input.MaleTicketCurrencyCode, StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(InputKey(nameof(EventDraftInput.OrganizerPaymentAccountId)), "ارز حساب دریافت وجه با ارز رویداد هماهنگ نیست.");

        Input.OrganizerPaymentInstructions = account.PublicPaymentInstructions;
        ModelState.Remove($"{nameof(Input)}.{nameof(EventDraftInput.OrganizerPaymentInstructions)}");
    }

    private void ValidateFaqs()
    {
        var filledFaqs = Input.Faqs
            .Where(item => !string.IsNullOrWhiteSpace(item.Question) || !string.IsNullOrWhiteSpace(item.Answer))
            .ToList();

        if (filledFaqs.Count > 10)
            ModelState.AddModelError(nameof(Input.Faqs), "برای هر رویداد حداکثر 10 سوال متداول می توانید ثبت کنید.");

        for (var index = 0; index < Input.Faqs.Count; index++)
        {
            var item = Input.Faqs[index];
            var hasQuestion = !string.IsNullOrWhiteSpace(item.Question);
            var hasAnswer = !string.IsNullOrWhiteSpace(item.Answer);
            if (hasQuestion != hasAnswer)
                ModelState.AddModelError($"Input.Faqs[{index}].Question", "برای هر سوال متداول، سوال و پاسخ را با هم وارد کنید.");
        }
    }

    private void EnsureFaqRows()
    {
        Input.Faqs = Input.Faqs
            .Where(item => !string.IsNullOrWhiteSpace(item.Question) || !string.IsNullOrWhiteSpace(item.Answer))
            .Take(10)
            .ToList();

        while (Input.Faqs.Count < 5)
        {
            Input.Faqs.Add(new EventFaqInput());
        }
    }

    private void ValidateRequiredText(string key, string? value, string label, int minLength, int maxLength)
    {
        var normalized = (value ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalized))
        {
            ModelState.AddModelError(key, $"{label} را وارد کنید.");
            return;
        }

        if (normalized.Length < minLength || normalized.Length > maxLength)
        {
            ModelState.AddModelError(key, $"{label} باید بین {minLength} تا {maxLength} کاراکتر باشد.");
        }
    }

    private static string InputKey(string propertyName) => $"{nameof(Input)}.{propertyName}";

    private void ValidateAgeRange(string key, string? ageRange, string label)
    {
        var normalized = (ageRange ?? string.Empty).Trim();
        var parts = normalized.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2
            || !int.TryParse(parts[0], out var min)
            || !int.TryParse(parts[1], out var max)
            || min < 18
            || max < min)
        {
            ModelState.AddModelError(key, $"{label} معتبر نیست.");
        }
    }

    public static string GetOperationalStatusClass(AdminEventOperationalStatus status) => DisplayFormatter.OperationalStatusClass(status);

    public static string GetProfileStatusClass(DomainEventApprovalStatus status) => DisplayFormatter.ApprovalStatusClass(status);

    private void ValidateTicketPrice(string key, decimal value, string label)
    {
        if (value <= 0)
        {
            ModelState.AddModelError(key, $"{label} باید بیشتر از صفر باشد.");
            return;
        }

        var rate = CurrencyLookupOptions
            .FirstOrDefault(item => string.Equals(item.Name, Input.MaleTicketCurrencyCode, StringComparison.OrdinalIgnoreCase))
            ?.ExchangeRateToIrr;
        if (rate is null or <= 0)
            return;

        var reportingValue = value * rate.Value;
        if (reportingValue < MinTicketValueIrr || reportingValue > MaxTicketValueIrr)
        {
            ModelState.AddModelError(key, $"{label} باید ارزشی بین {FormatWholeNumber(MinTicketValueIrr)} تا {FormatWholeNumber(MaxTicketValueIrr)} ریال ایران داشته باشد.");
        }
    }

    private IFormFile? ValidateEventImage(IFormFile? file, string key, string label)
    {
        if (file is null || file.Length == 0)
            return null;

        if (file.Length > MaxEventImageBytes)
        {
            ModelState.AddModelError(key, $"{label} باید کمتر از ۵ مگابایت باشد.");
            return null;
        }

        if (!IsSupportedEventImage(file))
        {
            ModelState.AddModelError(key, $"{label} باید با فرمت JPG یا PNG باشد.");
            return null;
        }

        if (!TryReadImageDimensions(file, out var width, out var height))
        {
            ModelState.AddModelError(key, $"ابعاد {label} قابل خواندن نیست.");
            return null;
        }

        if (width < MinEventImageWidth || height < MinEventImageHeight)
        {
            ModelState.AddModelError(key, $"{label} باید حداقل {MinEventImageWidth}×{MinEventImageHeight} پیکسل باشد.");
            return null;
        }

        var expectedRatio = EventImageAspectWidth / (double)EventImageAspectHeight;
        var actualRatio = width / (double)height;
        if (Math.Abs(actualRatio - expectedRatio) > 0.06)
        {
            ModelState.AddModelError(key, $"{label} باید نسبت {EventImageAspectWidth}:{EventImageAspectHeight} داشته باشد. تصویر فعلی {width}×{height} است.");
            return null;
        }

        return file;
    }

    private async Task<string> SaveEventImageAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var extension = string.Equals(file.ContentType, "image/png", StringComparison.OrdinalIgnoreCase)
            ? ".png"
            : ".jpg";
        var safeName = $"{Guid.NewGuid():N}{extension}";
        var webRoot = string.IsNullOrWhiteSpace(_environment.WebRootPath)
            ? Path.Combine(_environment.ContentRootPath, "wwwroot")
            : _environment.WebRootPath;
        var folder = Path.Combine(webRoot, "uploads", "events");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, safeName);
        await using var stream = System.IO.File.Create(path);
        await file.CopyToAsync(stream, cancellationToken);
        return $"/uploads/events/{safeName}";
    }

    private static bool IsSupportedEventImage(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        return (string.Equals(file.ContentType, "image/jpeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(file.ContentType, "image/png", StringComparison.OrdinalIgnoreCase))
            && (string.Equals(extension, ".jpg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".jpeg", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".png", StringComparison.OrdinalIgnoreCase));
    }

    private static bool TryReadImageDimensions(IFormFile file, out int width, out int height)
    {
        width = 0;
        height = 0;

        try
        {
            using var stream = file.OpenReadStream();
            var header = new byte[24];
            if (stream.Read(header, 0, header.Length) != header.Length)
                return false;

            if (header[0] == 0x89
                && header[1] == 0x50
                && header[2] == 0x4E
                && header[3] == 0x47)
            {
                width = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(16, 4));
                height = BinaryPrimitives.ReadInt32BigEndian(header.AsSpan(20, 4));
                return width > 0 && height > 0;
            }

            if (header[0] == 0xFF && header[1] == 0xD8)
            {
                stream.Position = 2;
                return TryReadJpegDimensions(stream, out width, out height);
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static bool TryReadJpegDimensions(Stream stream, out int width, out int height)
    {
        width = 0;
        height = 0;

        while (stream.Position < stream.Length)
        {
            var prefix = stream.ReadByte();
            if (prefix != 0xFF)
                continue;

            int marker;
            do
            {
                marker = stream.ReadByte();
            }
            while (marker == 0xFF);

            if (marker < 0)
                return false;

            if (marker is 0xD8 or 0xD9)
                continue;

            var length = ReadJpegSegmentLength(stream);
            if (length < 2)
                return false;

            if (marker is 0xC0 or 0xC1 or 0xC2 or 0xC3 or 0xC5 or 0xC6 or 0xC7 or 0xC9 or 0xCA or 0xCB or 0xCD or 0xCE or 0xCF)
            {
                if (stream.ReadByte() < 0)
                    return false;

                height = ReadJpegSegmentLength(stream);
                width = ReadJpegSegmentLength(stream);
                return width > 0 && height > 0;
            }

            stream.Seek(length - 2, SeekOrigin.Current);
        }

        return false;
    }

    private static int ReadJpegSegmentLength(Stream stream)
    {
        var high = stream.ReadByte();
        var low = stream.ReadByte();
        return high < 0 || low < 0 ? -1 : (high << 8) + low;
    }

    private static string FormatWholeNumber(decimal value) => value.ToString("N0", CultureInfo.InvariantCulture);

    private static bool TryCombineDateAndTime(string dateText, string timeText, bool useShamsi, out DateTimeOffset combined, out string errorMessage)
    {
        combined = default;
        errorMessage = "تاریخ و ساعت وارد شده معتبر نیست.";

        var normalizedDate = NormalizeNumericText(dateText);
        var normalizedTime = NormalizeNumericText(timeText);
        if (string.IsNullOrWhiteSpace(normalizedDate) || string.IsNullOrWhiteSpace(normalizedTime))
        {
            errorMessage = "تاریخ و ساعت را کامل وارد کنید.";
            return false;
        }

        var parts = normalizedTime.Split(':', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2
            || !int.TryParse(parts[0], out var hour)
            || !int.TryParse(parts[1], out var minute))
        {
            errorMessage = "ساعت وارد شده معتبر نیست.";
            return false;
        }

        try
        {
            if (!PersianDateFormatter.TryParseDate(normalizedDate, useShamsi, out var datePart))
            {
                errorMessage = "تاریخ وارد شده معتبر نیست.";
                return false;
            }

            combined = new DateTimeOffset(
                datePart.Year,
                datePart.Month,
                datePart.Day,
                hour,
                minute,
                0,
                datePart.Offset).ToUniversalTime();
            return true;
        }
        catch
        {
            errorMessage = "تاریخ و ساعت وارد شده معتبر نیست.";
            return false;
        }
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

    private static string NormalizeCurrencyCodeForForm(string? currencyCode)
        => string.IsNullOrWhiteSpace(currencyCode) ? "IRR" : currencyCode.Trim().ToUpperInvariant();

    private string FormatDateInput(DateTimeOffset dateTime)
        => _session.IsRtl
            ? PersianDateFormatter.FormatDate(dateTime, useShamsi: true)
            : dateTime.ToLocalTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

    private static string FormatTimeInput(DateTimeOffset dateTime)
        => dateTime.ToLocalTime().ToString("HH:mm", CultureInfo.InvariantCulture);

    private static string StripHtml(string value)
    {
        return System.Text.RegularExpressions.Regex.Replace(value ?? string.Empty, "<.*?>", string.Empty).Trim();
    }

    private static string ToFriendlyValidationMessage(string message)
    {
        if (message.Contains("End time must be after start time", StringComparison.OrdinalIgnoreCase))
            return "زمان پایان باید بعد از زمان شروع باشد.";
        if (message.Contains("Each event can have at most 10 tags", StringComparison.OrdinalIgnoreCase))
            return "برای هر رویداد حداکثر 10 تگ می توانید ثبت کنید.";
        if (message.Contains("Each event tag must be between 2 and 30 characters", StringComparison.OrdinalIgnoreCase))
            return "طول هر تگ باید بین 2 تا 30 کاراکتر باشد.";
        if (message.Contains("education level does not meet", StringComparison.OrdinalIgnoreCase))
            return "سطح تحصیلی شرکت‌کننده با حداقل شرط تحصیلی این رویداد مطابقت ندارد.";

        return "اطلاعات فرم معتبر نیست. لطفاً فیلدها را بازبینی کنید.";
    }

    private void SyncSharedTicketCurrency()
    {
        var eventCurrencyCode = NormalizeCurrencyCodeForForm(Input.MaleTicketCurrencyCode);
        Input.MaleTicketCurrencyCode = eventCurrencyCode;
        Input.FemaleTicketCurrencyCode = eventCurrencyCode;
        ModelState.Remove($"{nameof(Input)}.{nameof(EventDraftInput.FemaleTicketCurrencyCode)}");
    }

    private static long? MapRestrictionToEducationLevelId(EventEducationLevelRestriction restriction) => restriction switch
    {
        EventEducationLevelRestriction.DiplomaOrHigher => 2,
        EventEducationLevelRestriction.BachelorOrHigher => 3,
        EventEducationLevelRestriction.MasterOrHigher => 4,
        EventEducationLevelRestriction.ProfessionalDoctorateOrPhD => 5,
        _ => null
    };

    private static EventEducationLevelRestriction MapEducationLevelIdToRestriction(long? educationLevelId) => educationLevelId switch
    {
        null => EventEducationLevelRestriction.WithoutLimit,
        2 => EventEducationLevelRestriction.DiplomaOrHigher,
        3 => EventEducationLevelRestriction.BachelorOrHigher,
        4 => EventEducationLevelRestriction.MasterOrHigher,
        5 => EventEducationLevelRestriction.ProfessionalDoctorateOrPhD,
        _ => EventEducationLevelRestriction.WithoutLimit
    };
}
