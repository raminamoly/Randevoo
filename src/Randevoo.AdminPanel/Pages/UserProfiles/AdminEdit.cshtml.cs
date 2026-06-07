using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.UserProfiles;

[Authorize(Policy = Policies.AdminOnly)]
public class AdminEditModel : PageModel
{
    private readonly IAdminUserProfilesApiClient _adminProfilesApi;
    private readonly ILocationsApiClient _locationsApi;
    private readonly CurrentSessionState _session;

    public AdminEditModel(
        IAdminUserProfilesApiClient adminProfilesApi,
        ILocationsApiClient locationsApi,
        CurrentSessionState session)
    {
        _adminProfilesApi = adminProfilesApi;
        _locationsApi = locationsApi;
        _session = session;
    }

    public AdminUserProfileEditor Editor { get; private set; } = new();

    [BindProperty]
    public AdminUserProfileEditorInput ProfileInput { get; set; } = new();

    [BindProperty]
    public string DateOfBirthText { get; set; } = string.Empty;

    public IReadOnlyList<CountryOption> Countries { get; private set; } = Array.Empty<CountryOption>();
    public IReadOnlyList<CityOption> Cities { get; private set; } = Array.Empty<CityOption>();
    public IReadOnlyList<EducationLevelOption> EducationLevels { get; private set; } = Array.Empty<EducationLevelOption>();
    public IReadOnlyList<GenderOption> Genders { get; private set; } = Array.Empty<GenderOption>();
    public IReadOnlyList<ZodiacSignOption> ZodiacSigns { get; private set; } = Array.Empty<ZodiacSignOption>();

    public SelectList CountryOptions => new(Countries, nameof(CountryOption.Id), nameof(CountryOption.Name), ProfileInput.CountryId);
    public SelectList CityOptions => new(Cities, nameof(CityOption.Id), nameof(CityOption.Name), ProfileInput.CityId);
    public SelectList EducationOptions => new(EducationLevels, nameof(EducationLevelOption.Id), nameof(EducationLevelOption.Title), ProfileInput.EducationLevelId);
    public SelectList ZodiacSignOptions => new(ZodiacSigns, nameof(ZodiacSignOption.Id), nameof(ZodiacSignOption.Title), ProfileInput.ZodiacSignId);
    public SelectList GenderOptions => new(
        Genders.Select(gender => new
        {
            Value = gender.Id switch
            {
                2 => Randevoo.Domain.Enums.Gender.Male,
                3 => Randevoo.Domain.Enums.Gender.Female,
                _ => Randevoo.Domain.Enums.Gender.Unknown
            },
            gender.Title
        }),
        "Value",
        "Title",
        ProfileInput.Gender);

    [TempData]
    public string? StatusMessage { get; set; }

    public bool IsRtl => _session.IsRtl;

    public async Task OnGetAsync(long userId)
    {
        await LoadAsync(userId);
        ProfileInput = Editor.Input;
        SyncDateText();
    }

    public async Task<IActionResult> OnPostSaveAsync(long userId)
    {
        if (PersianDateFormatter.TryParseDate(DateOfBirthText, IsRtl, out var birthDate))
        {
            ProfileInput.DateOfBirth = DateOnly.FromDateTime(birthDate.DateTime);
        }
        else
        {
            ModelState.AddModelError(nameof(DateOfBirthText), "تاریخ تولد معتبر نیست.");
        }

        if (!ModelState.IsValid)
        {
            await LoadOptionsAsync();
            Editor = await _adminProfilesApi.GetEditorAsync(userId, CurrentAdmin());
            SyncDateText();
            return Page();
        }

        await _adminProfilesApi.SaveProfileAsync(userId, CurrentAdmin(), ProfileInput);
        StatusMessage = "پروفایل کاربر به روز شد.";
        return RedirectToPage(new { userId });
    }

    public async Task<IActionResult> OnPostAddImageAsync(long userId, AdminUserProfileImageInput imageInput)
    {
        if (!TryValidateModel(imageInput, nameof(imageInput)))
        {
            await LoadAsync(userId);
            ProfileInput = Editor.Input;
            return Page();
        }

        await _adminProfilesApi.AddImageAsync(userId, CurrentAdmin(), imageInput);
        StatusMessage = "تصویر پروفایل اضافه شد.";
        return RedirectToPage(new { userId });
    }

    public async Task<IActionResult> OnPostRemoveImageAsync(long userId, string imageUrl)
    {
        await _adminProfilesApi.RemoveImageAsync(userId, CurrentAdmin(), imageUrl);
        StatusMessage = "تصویر پروفایل حذف شد.";
        return RedirectToPage(new { userId });
    }

    public async Task<IActionResult> OnPostAddInterestAsync(long userId, AdminUserProfileInterestInput interestInput)
    {
        if (!TryValidateModel(interestInput, nameof(interestInput)))
        {
            await LoadAsync(userId);
            ProfileInput = Editor.Input;
            return Page();
        }

        await _adminProfilesApi.AddInterestAsync(userId, CurrentAdmin(), interestInput);
        StatusMessage = "علاقه کاربر اضافه شد.";
        return RedirectToPage(new { userId });
    }

    public async Task<IActionResult> OnPostRemoveInterestAsync(long userId, string interestName)
    {
        await _adminProfilesApi.RemoveInterestAsync(userId, CurrentAdmin(), interestName);
        StatusMessage = "علاقه کاربر حذف شد.";
        return RedirectToPage(new { userId });
    }

    public async Task<IActionResult> OnPostSendSmsAsync(long userId, AdminInstantSmsInput smsInput)
    {
        if (!TryValidateModel(smsInput, nameof(smsInput)))
        {
            await LoadAsync(userId);
            ProfileInput = Editor.Input;
            return Page();
        }

        await _adminProfilesApi.SendInstantSmsAsync(userId, CurrentAdmin(), smsInput);
        StatusMessage = "پیامک فوری در صف ثبت شد.";
        return RedirectToPage(new { userId });
    }

    private async Task LoadAsync(long userId)
    {
        Editor = await _adminProfilesApi.GetEditorAsync(userId, CurrentAdmin());
        await LoadOptionsAsync();
    }

    private async Task LoadOptionsAsync()
    {
        Countries = await _locationsApi.GetCountriesAsync();
        Cities = await _locationsApi.GetCitiesAsync();
        EducationLevels = await _locationsApi.GetEducationLevelsAsync();
        Genders = await _locationsApi.GetGendersAsync();
        ZodiacSigns = await _locationsApi.GetZodiacSignsAsync();
    }

    private MockUser CurrentAdmin() => _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");

    private void SyncDateText()
    {
        if (!string.IsNullOrWhiteSpace(DateOfBirthText))
            return;

        var offset = new DateTimeOffset(ProfileInput.DateOfBirth.ToDateTime(TimeOnly.MinValue), TimeSpan.FromHours(3.5));
        DateOfBirthText = PersianDateFormatter.FormatDate(offset, IsRtl);
    }
}
