using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Randevoo.AdminPanel.Pages.Settings;

[AllowAnonymous]
public class IndexModel : PageModel
{
    public string? ReturnUrl { get; private set; } = "/Dashboard/Index";

    public IActionResult OnGet(string? lang, string? returnUrl = null)
    {
        ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/Dashboard/Index" : returnUrl;
        return Page();
    }
}
