using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Services.Auth;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly MockAuthService _authService;
    private readonly CurrentSessionState _session;

    public LoginModel(MockAuthService authService, CurrentSessionState session)
    {
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

    public SelectList RoleOptions => new(new[] { AdminRole.Admin, AdminRole.EventPlanner }
        .Select(role => new { Value = role, Text = DisplayFormatter.Role(role) }), "Value", "Text");

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
        Input ??= new LoginRequest();
        Step = 1;
        return Page();
    }

    public async Task<IActionResult> OnPostRequestCodeAsync()
    {
        Input ??= new LoginRequest();
        var result = await _authService.RequestCodeAsync(
            (Input.Mobile ?? string.Empty).Trim(),
            Input.Role);

        if (!result.Success || result.User is null)
        {
            ErrorMessage = result.ErrorMessage ?? "دریافت کد تایید انجام نشد.";
            Step = 1;
            return Page();
        }

        Step = 2;
        Message = "کد تایید آزمایشی: 123456";
        return Page();
    }

    public async Task<IActionResult> OnPostLoginAsync()
    {
        Input ??= new LoginRequest();
        var result = await _authService.VerifyLoginAsync(
            (Input.Mobile ?? string.Empty).Trim(),
            (Input.VerificationCode ?? string.Empty).Trim(),
            Input.Role);

        if (!result.Success || result.User is null)
        {
            ErrorMessage = result.ErrorMessage ?? "ورود انجام نشد.";
            Step = 2;
            return Page();
        }

        await _authService.SignInAsync(result.User);

        return RedirectToPage(result.User.Role == AdminRole.EventPlanner ? "/Dashboard/My" : "/Dashboard/Index");
    }
}
