using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.Auth;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Exceptions;

namespace Randevoo.AdminPanel.Pages.Planner;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class ProfileModel : PageModel
{
    private readonly IPlannerProfilesApiClient _profilesApi;
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;
    private readonly MockAuthService _authService;

    public ProfileModel(IPlannerProfilesApiClient profilesApi, IFinanceApiClient financeApi, CurrentSessionState session, MockAuthService authService)
    {
        _profilesApi = profilesApi;
        _financeApi = financeApi;
        _session = session;
        _authService = authService;
    }

    [BindProperty]
    public PlannerProfileInput Input { get; set; } = new();

    [BindProperty]
    public IFormFile? ProfileImageFile { get; set; }

    public PlannerProfileViewModel? Profile { get; private set; }

    public IReadOnlyList<PlannerBankAccountItem> BankAccounts { get; private set; } = Array.Empty<PlannerBankAccountItem>();

    public async Task<IActionResult> OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        if (current.Role != AdminRole.EventPlanner)
        {
            return RedirectToPage("/Account/Forbidden");
        }

        Profile = await _profilesApi.GetCurrentAsync(current);
        if (Profile is not null)
        {
            BankAccounts = await _financeApi.GetPlannerBankAccountsAsync(current, Profile.UserId);
            Input = new PlannerProfileInput
            {
                FullName = Profile.FullName,
                City = Profile.City,
                Title = Profile.Title,
                PictureUrl = Profile.PictureUrl,
                Resume = Profile.Resume,
                SettlementCurrencyCode = Profile.SettlementCurrencyCode
            };
        }
        else
        {
            Input = new PlannerProfileInput
            {
                FullName = current.FullName,
                City = "تهران",
                SettlementCurrencyCode = "IRR"
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        if (current.Role != AdminRole.EventPlanner)
        {
            return RedirectToPage("/Account/Forbidden");
        }

        if (ProfileImageFile is not null)
        {
            Input.PictureUrl = await ToDataUrlAsync(ProfileImageFile);
        }

        var existingProfile = await _profilesApi.GetCurrentAsync(current);
        Input.SettlementCurrencyCode = existingProfile?.SettlementCurrencyCode ?? "IRR";

        try
        {
            Profile = await _profilesApi.UpsertAsync(current, Input);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            Profile = await _profilesApi.GetCurrentAsync(current);
            if (Profile is not null)
                BankAccounts = await _financeApi.GetPlannerBankAccountsAsync(current, Profile.UserId);
            return Page();
        }
        catch (BusinessRuleViolationException)
        {
            ModelState.AddModelError(string.Empty, "اطلاعات پروفایل معتبر نیست.");
            Profile = await _profilesApi.GetCurrentAsync(current);
            if (Profile is not null)
                BankAccounts = await _financeApi.GetPlannerBankAccountsAsync(current, Profile.UserId);
            return Page();
        }
        await _authService.SignInAsync(new MockUser
        {
            Id = current.Id,
            FullName = Profile.FullName,
            Mobile = current.Mobile,
            Role = current.Role,
            IsActive = current.IsActive
        });
        TempData["PlannerProfileSaved"] = Profile.HasPendingChanges
            ? "تغییرات پروفایل برای تایید مدیر ثبت شد و هنوز منتشر نشده است."
            : "پروفایل برگزارکننده ذخیره شد.";
        return RedirectToPage();
    }

    private static async Task<string> ToDataUrlAsync(IFormFile file)
    {
        await using var memory = new MemoryStream();
        await file.CopyToAsync(memory);
        var base64 = Convert.ToBase64String(memory.ToArray());
        return $"data:{file.ContentType};base64,{base64}";
    }

}
