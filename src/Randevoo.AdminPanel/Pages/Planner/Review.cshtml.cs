using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Planner;

[Authorize(Policy = Policies.AdminOnly)]
public class ReviewModel : PageModel
{
    private readonly IPlannerProfilesApiClient _profilesApi;
    private readonly CurrentSessionState _session;

    public ReviewModel(IPlannerProfilesApiClient profilesApi, CurrentSessionState session)
    {
        _profilesApi = profilesApi;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public long Id { get; set; }

    [BindProperty]
    public PlannerProfileApprovalInput Input { get; set; } = new();

    public PlannerProfileViewModel Profile { get; private set; } = new();

    public bool IsRtl => _session.IsRtl;

    public async Task<IActionResult> OnGetAsync()
    {
        var profile = await _profilesApi.GetByUserIdAsync(Id);
        if (profile is null)
            return NotFound();

        Profile = profile;
        Input = BuildApprovalInput(profile);
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        ValidateInput();
        if (!ModelState.IsValid)
        {
            await LoadProfileAsync();
            return Page();
        }

        await _profilesApi.ApproveAsync(current, Id, Input);
        TempData["PlannerProfileReviewMessage"] = "پروفایل برگزارکننده تایید و منتشر شد.";
        return RedirectToPage("/Planner/Approvals");
    }

    public async Task<IActionResult> OnPostRejectAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        await _profilesApi.RejectAsync(current, Id, Input.ReviewNote);
        TempData["PlannerProfileReviewMessage"] = "درخواست تغییر پروفایل رد شد.";
        return RedirectToPage("/Planner/Approvals");
    }

    private async Task LoadProfileAsync()
    {
        Profile = await _profilesApi.GetByUserIdAsync(Id) ?? new PlannerProfileViewModel();
    }

    private void ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(Input.FullName) || Input.FullName.Trim().Length is < 2 or > 100)
            ModelState.AddModelError(nameof(Input.FullName), "نام برگزارکننده باید بین 2 تا 100 کاراکتر باشد.");

        if (string.IsNullOrWhiteSpace(Input.City) || Input.City.Trim().Length is < 2 or > 100)
            ModelState.AddModelError(nameof(Input.City), "شهر فعالیت باید بین 2 تا 100 کاراکتر باشد.");

        if (string.IsNullOrWhiteSpace(Input.Title) || Input.Title.Trim().Length is < 2 or > 100)
            ModelState.AddModelError(nameof(Input.Title), "عنوان حرفه ای باید بین 2 تا 100 کاراکتر باشد.");

        if (!string.IsNullOrWhiteSpace(Input.PictureUrl) && Input.PictureUrl.Trim().Length > 500)
            ModelState.AddModelError(nameof(Input.PictureUrl), "آدرس تصویر نباید بیشتر از 500 کاراکتر باشد.");

        if (string.IsNullOrWhiteSpace(Input.Resume) || Input.Resume.Trim().Length is < 10 or > 4000)
            ModelState.AddModelError(nameof(Input.Resume), "متن معرفی باید بین 10 تا 4000 کاراکتر باشد.");

        if (!string.IsNullOrWhiteSpace(Input.ReviewNote) && Input.ReviewNote.Trim().Length > 1000)
            ModelState.AddModelError(nameof(Input.ReviewNote), "یادداشت مدیر نباید بیشتر از 1000 کاراکتر باشد.");
    }

    private static PlannerProfileApprovalInput BuildApprovalInput(PlannerProfileViewModel profile) => new()
    {
        FullName = profile.PendingFullName ?? profile.FullName,
        City = profile.PendingCity ?? profile.City,
        Title = profile.PendingTitle ?? profile.Title,
        PictureUrl = profile.PendingPictureUrl ?? profile.PictureUrl,
        Resume = profile.PendingResume ?? profile.Resume,
        ReviewNote = profile.PendingReviewNote
    };
}
