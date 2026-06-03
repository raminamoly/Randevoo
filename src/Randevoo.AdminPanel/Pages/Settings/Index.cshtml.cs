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

        if (!string.IsNullOrWhiteSpace(lang))
        {
            Response.Cookies.Append("randevoo.admin.lang", lang, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });

            return LocalRedirect(ReturnUrl);
        }

        return Page();
    }
}

