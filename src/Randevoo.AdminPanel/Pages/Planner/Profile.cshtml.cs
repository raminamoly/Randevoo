using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.Auth;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Planner;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class ProfileModel : PageModel
{
    private readonly IPlannerProfilesApiClient _profilesApi;
    private readonly CurrentSessionState _session;
    private readonly MockAuthService _authService;

    public ProfileModel(IPlannerProfilesApiClient profilesApi, CurrentSessionState session, MockAuthService authService)
    {
        _profilesApi = profilesApi;
        _session = session;
        _authService = authService;
    }

    [BindProperty]
    public PlannerProfileInput Input { get; set; } = new();

    [BindProperty]
    public IFormFile? ProfileImageFile { get; set; }

    public PlannerProfileViewModel? Profile { get; private set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        if (current.Role != AdminRole.EventPlanner)
        {
            return RedirectToPage("/Account/Forbidden");
        }

        Profile = await _profilesApi.GetCurrentAsync(current);
        if (Profile is not null)
        {
            Input = new PlannerProfileInput
            {
                FullName = Profile.FullName,
                City = Profile.City,
                Title = Profile.Title,
                PictureUrl = Profile.PictureUrl,
                Resume = Profile.Resume
            };
        }
        else
        {
            Input = new PlannerProfileInput
            {
                FullName = current.FullName,
                City = "تهران"
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        if (current.Role != AdminRole.EventPlanner)
        {
            return RedirectToPage("/Account/Forbidden");
        }

        if (ProfileImageFile is not null)
        {
            Input.PictureUrl = await ToDataUrlAsync(ProfileImageFile);
        }

        Profile = await _profilesApi.UpsertAsync(current, Input);
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
