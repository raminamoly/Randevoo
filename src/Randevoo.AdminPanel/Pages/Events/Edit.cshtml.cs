using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using AdminEventOperationalStatus = Randevoo.AdminPanel.Models.Events.EventOperationalStatus;
using AdminEventReviewStatus = Randevoo.AdminPanel.Models.Events.EventReviewStatus;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class EditModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly IEventTagsApiClient _eventTagsApi;
    private readonly IUsersApiClient _usersApi;
    private readonly ILocationsApiClient _locationsApi;
    private readonly CurrentSessionState _session;

    public EditModel(IEventsApiClient eventsApi, IEventTagsApiClient eventTagsApi, IUsersApiClient usersApi, ILocationsApiClient locationsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _eventTagsApi = eventTagsApi;
        _usersApi = usersApi;
        _locationsApi = locationsApi;
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
    public IFormFile? Image1File { get; set; }

    [BindProperty]
    public IFormFile? Image2File { get; set; }

    [BindProperty]
    public IFormFile? Image3File { get; set; }

    [BindProperty]
    public long? AssignedPlannerId { get; set; }

    [BindProperty(SupportsGet = true)]
    public long? ExistingEventId { get; set; }

    public bool IsNew => ExistingEventId is null;

    public bool IsRtl => _session.IsRtl;

    public bool IsAdmin => _session.CurrentUser?.Role == AdminRole.Admin;

    public bool IsPlanner => _session.CurrentUser?.Role == AdminRole.EventPlanner;

    public string? ReviewNote { get; set; }

    public string StatusText { get; set; } = AdminEventOperationalStatus.Draft.ToString();

    public AdminEventOperationalStatus StatusValue { get; set; } = AdminEventOperationalStatus.Draft;

    public string StatusClass { get; set; } = "status-draft";

    public AdminEventReviewStatus ReviewStatusValue { get; set; } = AdminEventReviewStatus.NotSubmitted;

    public string ReviewStatusClass { get; set; } = "status-draft";

    public SelectList CountryOptions { get; private set; } = new(Array.Empty<object>());

    public SelectList CityOptions { get; private set; } = new(Array.Empty<object>());

    public string CityOptionsJson { get; private set; } = "[]";

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

    private List<long> PlannerIds { get; set; } = new();

    private IReadOnlyList<SystemLookupOption> CurrencyLookupOptions { get; set; } = Array.Empty<SystemLookupOption>();

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadLookupOptionsAsync();

        if (ExistingEventId is long id)
        {
            var @event = await _eventsApi.GetEventAsync(id);
            if (@event is null)
            {
                return NotFound();
            }

            Input = @event.ActiveDraft;
            ReviewNote = @event.AdminReviewNote;
            StatusText = @event.OperationalStatus.ToString();
            StatusValue = @event.OperationalStatus;
            StatusClass = GetOperationalStatusClass(@event.OperationalStatus);
            ReviewStatusValue = @event.ReviewStatus;
            ReviewStatusClass = GetReviewStatusClass(@event.ReviewStatus);
            AssignedPlannerId = @event.PlannerUserId;
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
            StatusText = AdminEventOperationalStatus.Draft.ToString();
            StatusValue = AdminEventOperationalStatus.Draft;
            StatusClass = GetOperationalStatusClass(AdminEventOperationalStatus.Draft);
            ReviewStatusValue = AdminEventReviewStatus.NotSubmitted;
            ReviewStatusClass = GetReviewStatusClass(AdminEventReviewStatus.NotSubmitted);
            if (IsAdmin)
            {
                AssignedPlannerId = GetDefaultPlannerId();
            }
        }

        await LoadLookupOptionsAsync();
        SyncFormTextFromInput();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        await LoadLookupOptionsAsync();

        ApplyEventContextDefaults();

        if (!TryCombineDateAndTime(StartDateText, StartTimeText, _session.IsRtl, out var startAtUtc, out var startError))
        {
            ModelState.AddModelError(nameof(StartDateText), startError);
        }
        else
        {
            Input.StartAtUtc = startAtUtc;
        }

        if (!TryCombineDateAndTime(EndDateText, EndTimeText, _session.IsRtl, out var endAtUtc, out var endError))
        {
            ModelState.AddModelError(nameof(EndDateText), endError);
        }
        else
        {
            Input.EndAtUtc = endAtUtc;
        }

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

        try
        {
            var saved = await _eventsApi.SaveEventAsync(Input, current, ExistingEventId, AssignedPlannerId);
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
        CurrencyOptions = new SelectList(
            CurrencyLookupOptions.Select(item => new { Code = item.Name, Title = $"{item.DisplayNameFa} ({item.Name})" }),
            "Code",
            "Title");

        Countries = await _locationsApi.GetCountriesAsync();
        Cities = await _locationsApi.GetCitiesAsync();
        EducationLevels = await _locationsApi.GetEducationLevelsAsync();

        if (string.IsNullOrWhiteSpace(Input.Country) || Countries.All(country => country.Name != Input.Country))
        {
            Input.Country = Countries.FirstOrDefault()?.Name ?? "ایران";
        }

        if (string.IsNullOrWhiteSpace(Input.City) || Cities.All(city => city.CountryName != Input.Country || city.Name != Input.City))
        {
            Input.City = Cities.FirstOrDefault(city => city.CountryName == Input.Country)?.Name ?? string.Empty;
        }

        CountryOptions = new SelectList(Countries, "Name", "Name", Input.Country);
        CityOptions = new SelectList(Cities.Where(city => city.CountryName == Input.Country), "Name", "Name", Input.City);
        Input.MinimumEducationLevelId ??= MapRestrictionToEducationLevelId(Input.EducationLevelRestriction);
        var minimumEducationOptions = new[] { new { Id = string.Empty, Title = "بدون محدودیت" } }
            .Concat(EducationLevels
                .Where(level => level.Rank > 0)
                .Select(level => new { Id = level.Id.ToString(), level.Title }))
            .ToList();
        MinimumEducationLevelOptions = new SelectList(minimumEducationOptions, "Id", "Title", Input.MinimumEducationLevelId?.ToString() ?? string.Empty);
        CityOptionsJson = JsonSerializer.Serialize(Cities.Select(city => new
        {
            city.CountryName,
            city.Name,
            city.Latitude,
            city.Longitude
        }));
    }

    private long? GetDefaultPlannerId() => PlannerIds.Count == 0 ? null : PlannerIds[0];

    private void SyncFormTextFromInput()
    {
        StartDateText = PersianDateFormatter.FormatDate(Input.StartAtUtc, _session.IsRtl);
        StartTimeText = PersianDateFormatter.FormatTime(Input.StartAtUtc);
        EndDateText = PersianDateFormatter.FormatDate(Input.EndAtUtc, _session.IsRtl);
        EndTimeText = PersianDateFormatter.FormatTime(Input.EndAtUtc);
    }

    private void ApplyEventContextDefaults()
    {
        StatusText = IsNew ? AdminEventOperationalStatus.Draft.ToString() : StatusText;
        StatusValue = IsNew ? AdminEventOperationalStatus.Draft : StatusValue;
        StatusClass = GetOperationalStatusClass(StatusValue);
        ReviewStatusValue = IsNew ? AdminEventReviewStatus.NotSubmitted : ReviewStatusValue;
        ReviewStatusClass = GetReviewStatusClass(ReviewStatusValue);
        ReviewNote ??= null;
    }

    private async Task LoadExistingEventStatusAsync()
    {
        if (ExistingEventId is not long id)
        {
            StatusText = AdminEventOperationalStatus.Draft.ToString();
            StatusValue = AdminEventOperationalStatus.Draft;
            StatusClass = GetOperationalStatusClass(StatusValue);
            ReviewStatusValue = AdminEventReviewStatus.NotSubmitted;
            ReviewStatusClass = GetReviewStatusClass(ReviewStatusValue);
            return;
        }

        var existing = await _eventsApi.GetEventAsync(id);
        if (existing is null)
            return;

        ReviewNote = existing.AdminReviewNote;
        StatusText = existing.OperationalStatus.ToString();
        StatusValue = existing.OperationalStatus;
        StatusClass = GetOperationalStatusClass(existing.OperationalStatus);
        ReviewStatusValue = existing.ReviewStatus;
        ReviewStatusClass = GetReviewStatusClass(existing.ReviewStatus);
    }

    private void ValidateEventInput()
    {
        ValidateRequiredText(nameof(Input.Title), Input.Title, "عنوان رویداد", 2, 150);
        ValidateRequiredText(nameof(Input.DescriptionHtml), StripHtml(Input.DescriptionHtml), "توضیحات رویداد", 10, 10000);

        if (Input.EventTypeId <= 0)
            ModelState.AddModelError(nameof(Input.EventTypeId), "نوع رویداد را انتخاب کنید.");

        if (Input.EventModeId <= 0)
            ModelState.AddModelError(nameof(Input.EventModeId), "نحوه برگزاری را انتخاب کنید.");

        if (Input.IsOnline)
        {
            if (Input.OnlineEventPlatformId is null or <= 0)
                ModelState.AddModelError(nameof(Input.OnlineEventPlatformId), "پلتفرم آنلاین را انتخاب کنید.");

            if (string.IsNullOrWhiteSpace(Input.OnlineJoinUrl))
                ModelState.AddModelError(nameof(Input.OnlineJoinUrl), "لینک ورود رویداد آنلاین را وارد کنید.");
        }
        else
        {
            ValidateRequiredText(nameof(Input.Address), Input.Address, "آدرس", 5, 300);
        }

        Input.EducationLevelRestriction = MapEducationLevelIdToRestriction(Input.MinimumEducationLevelId);

        if (Input.MinimumEducationLevelId is long educationLevelId && EducationLevels.All(level => level.Id != educationLevelId || level.Rank <= 0))
            ModelState.AddModelError(nameof(Input.MinimumEducationLevelId), "حداقل سطح تحصیل معتبر نیست.");

        if (!Input.IsOnline)
        {
            if (Countries.All(country => country.Name != Input.Country))
                ModelState.AddModelError(nameof(Input.Country), "کشور انتخاب شده معتبر نیست.");

            if (Cities.All(city => city.CountryName != Input.Country || city.Name != Input.City))
                ModelState.AddModelError(nameof(Input.City), "شهر انتخاب شده برای این کشور معتبر نیست.");
        }

        if (Input.StartAtUtc != default
            && Input.EndAtUtc != default
            && Input.EndAtUtc <= Input.StartAtUtc)
        {
            ModelState.AddModelError(nameof(EndDateText), "زمان پایان باید بعد از زمان شروع باشد.");
        }

        Input.MaleTicketCurrencyCode = NormalizeCurrencyCodeForForm(Input.MaleTicketCurrencyCode);
        Input.FemaleTicketCurrencyCode = NormalizeCurrencyCodeForForm(Input.FemaleTicketCurrencyCode);

        if (Input.MaleTicketPrice is < 0.01m or > 1_000_000_000m)
            ModelState.AddModelError(nameof(Input.MaleTicketPrice), "مبلغ بلیت آقایان باید بیشتر از صفر و کمتر از ۱,۰۰۰,۰۰۰,۰۰۰ باشد.");

        if (Input.FemaleTicketPrice is < 0.01m or > 1_000_000_000m)
            ModelState.AddModelError(nameof(Input.FemaleTicketPrice), "مبلغ بلیت خانم‌ها باید بیشتر از صفر و کمتر از ۱,۰۰۰,۰۰۰,۰۰۰ باشد.");

        if (CurrencyLookupOptions.All(item => item.Name != Input.MaleTicketCurrencyCode))
            ModelState.AddModelError(nameof(Input.MaleTicketCurrencyCode), "واحد پول بلیت آقایان معتبر نیست.");

        if (CurrencyLookupOptions.All(item => item.Name != Input.FemaleTicketCurrencyCode))
            ModelState.AddModelError(nameof(Input.FemaleTicketCurrencyCode), "واحد پول بلیت خانم‌ها معتبر نیست.");

        if (Input.OrganizerCommissionPercent is < 0 or > 100)
            ModelState.AddModelError(nameof(Input.OrganizerCommissionPercent), "درصد کمیسیون باید بین 0 تا 100 باشد.");

        if (Input.CapacityMale <= 0)
            ModelState.AddModelError(nameof(Input.CapacityMale), "ظرفیت آقایان باید بیشتر از صفر باشد.");

        if (Input.CapacityFemale <= 0)
            ModelState.AddModelError(nameof(Input.CapacityFemale), "ظرفیت بانوان باید بیشتر از صفر باشد.");

        if (Input.LikeLimit is < 0 or > 10)
            ModelState.AddModelError(nameof(Input.LikeLimit), "تعداد لایک مجاز باید بین 0 تا 10 باشد.");

        ValidateAgeRange(nameof(Input.AgeRangeForMale), Input.AgeRangeForMale, "بازه سنی آقایان");
        ValidateAgeRange(nameof(Input.AgeRangeForFemale), Input.AgeRangeForFemale, "بازه سنی بانوان");

        if (Input.TagIds.Count > 10)
            ModelState.AddModelError(nameof(Input.TagIds), "برای هر رویداد حداکثر 10 تگ می توانید انتخاب کنید.");

        Input.TagIds = Input.TagIds.Distinct().ToList();
        ValidateFaqs();
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

    public static string GetReviewStatusClass(AdminEventReviewStatus status) => DisplayFormatter.ReviewStatusClass(status);

    private static async Task<string> ToDataUrlAsync(IFormFile file)
    {
        await using var memory = new MemoryStream();
        await file.CopyToAsync(memory);
        var base64 = Convert.ToBase64String(memory.ToArray());
        return $"data:{file.ContentType};base64,{base64}";
    }

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
            if (useShamsi)
            {
                var datePart = PersianDateFormatter.Parse($"{normalizedDate} 00:00");
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

            combined = DateTimeOffset.Parse($"{normalizedDate} {normalizedTime}").ToUniversalTime();
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
