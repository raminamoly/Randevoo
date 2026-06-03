using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Services.Auth;
using Randevoo.AdminPanel.Services.MockData;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly AdminPanelStore _store;
    private readonly MockAuthService _authService;
    private readonly CurrentSessionState _session;

    public LoginModel(AdminPanelStore store, MockAuthService authService, CurrentSessionState session)
    {
        _store = store;
        _authService = authService;
        _session = session;
    }

    [BindProperty]
    public LoginRequest Input { get; set; } = new();

    [BindProperty]
    public int Step { get; set; } = 1;

    public string? Message { get; set; }

    public string? ErrorMessage { get; set; }

    public bool IsRtl => _session.IsRtl;

    public SelectList RoleOptions => new(Enum.GetValues<AdminRole>()
        .Select(role => new { Value = role, Text = role.ToString() }), "Value", "Text");

    public IActionResult OnGet()
    {
        Step = 1;
        Input.Role = AdminRole.Admin;
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage(User.IsInRole(AdminRole.EventPlanner.ToString()) ? "/Dashboard/My" : "/Dashboard/Index");
        }

        return Page();
    }

    public IActionResult OnPostBack()
    {
        Step = 1;
        return Page();
    }

    public IActionResult OnPostRequestCode()
    {
        var user = _store.FindUserByMobile(Input.Mobile.Trim());
        if (user is null)
        {
            ErrorMessage = "This mobile number is not registered yet.";
            Step = 1;
            Input.Role = AdminRole.Admin;
            return Page();
        }

        if (user.Role != Input.Role)
        {
            ErrorMessage = "The selected role does not match this account.";
            Step = 1;
            return Page();
        }

        Step = 2;
        Message = "Mock SMS code: 123456";
        return Page();
    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        var result = await _authService.VerifyLoginAsync(Input.Mobile.Trim(), Input.VerificationCode.Trim(), Input.Role);
        if (!result.Success || result.User is null)
        {
            ErrorMessage = result.ErrorMessage ?? "Unable to sign in.";
            Step = 2;
            return Page();
        }

        await _authService.SignInAsync(result.User);

        return RedirectToPage(result.User.Role == AdminRole.EventPlanner ? "/Dashboard/My" : "/Dashboard/Index");
    }
}
