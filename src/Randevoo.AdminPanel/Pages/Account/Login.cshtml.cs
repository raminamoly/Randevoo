using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Services.Auth;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly MockAuthService _authService;
    private readonly CurrentSessionState _session;
    private readonly IWebHostEnvironment _environment;

    public LoginModel(MockAuthService authService, CurrentSessionState session, IWebHostEnvironment environment)
    {
        _authService = authService;
        _session = session;
        _environment = environment;
    }

    [BindProperty]
    public LoginRequest Input { get; set; } = new();

    [BindProperty]
    public int Step { get; set; } = 1;

    [BindProperty]
    public string? QuickLoginKey { get; set; }

    public string? Message { get; set; }

    public string? ErrorMessage { get; set; }

    public bool IsRtl => _session.IsRtl;

    public SelectList RoleOptions => new(new[] { AdminRole.Admin, AdminRole.EventPlanner, AdminRole.SupportTeam }
        .Select(role => new { Value = role, Text = DisplayFormatter.Role(role) }), "Value", "Text");

    public bool ShowQuickLogin => _environment.IsDevelopment() || _environment.IsEnvironment("Testing");

    public SelectList QuickLoginOptions => new(QuickLoginUsers
        .Select(user => new
        {
            Value = user.Key,
            Text = $"{user.Label} - {DisplayFormatter.Role(user.Role)} - {user.Mobile}"
        }), "Value", "Text");

    private static readonly IReadOnlyList<QuickLoginUser> QuickLoginUsers =
    [
        new("admin", "مدیر نمونه", "09125177721", AdminRole.Admin),
        new("planner", "برگزارکننده نمونه", "09125550000", AdminRole.EventPlanner),
        new("support", "پشتیبان نمونه", "09126660000", AdminRole.SupportTeam)
    ];

    public IActionResult OnGet()
    {
        Step = 1;
        Input.Role = AdminRole.Admin;
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToPage(ResolveHomePage(User));
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

        return RedirectToPage(ResolveHomePage(result.User.Role));
    }

    public async Task<IActionResult> OnPostQuickLoginAsync()
    {
        if (!ShowQuickLogin)
            return Forbid();

        var selected = QuickLoginUsers.FirstOrDefault(user =>
            string.Equals(user.Key, QuickLoginKey, StringComparison.OrdinalIgnoreCase));

        if (selected is null)
        {
            ErrorMessage = "حساب تستی انتخاب شده معتبر نیست.";
            Step = 1;
            return Page();
        }

        var result = await _authService.VerifyLoginAsync(selected.Mobile, "123456", selected.Role);
        if (!result.Success || result.User is null)
        {
            ErrorMessage = result.ErrorMessage ?? "ورود سریع انجام نشد.";
            Step = 1;
            Input.Mobile = selected.Mobile;
            Input.Role = selected.Role;
            return Page();
        }

        await _authService.SignInAsync(result.User);
        return RedirectToPage(ResolveHomePage(result.User.Role));
    }

    private static string ResolveHomePage(ClaimsPrincipal user)
    {
        if (user.IsInRole(AdminRole.SupportTeam.ToString()))
            return "/Support/Index";

        return user.IsInRole(AdminRole.EventPlanner.ToString()) ? "/Dashboard/My" : "/Dashboard/Index";
    }

    private static string ResolveHomePage(AdminRole role) => role switch
    {
        AdminRole.SupportTeam => "/Support/Index",
        AdminRole.EventPlanner => "/Dashboard/My",
        _ => "/Dashboard/Index"
    };

    private sealed record QuickLoginUser(string Key, string Label, string Mobile, AdminRole Role);
}
