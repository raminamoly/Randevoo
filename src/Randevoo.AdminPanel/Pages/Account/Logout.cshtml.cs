using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Services.Auth;

namespace Randevoo.AdminPanel.Pages.Account;

[AllowAnonymous]
public class LogoutModel : PageModel
{
    private readonly MockAuthService _authService;

    public LogoutModel(MockAuthService authService)
    {
        _authService = authService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        await _authService.SignOutAsync();
        return RedirectToPage("/Account/Login");
    }
}

