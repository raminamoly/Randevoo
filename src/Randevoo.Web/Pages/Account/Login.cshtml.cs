using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Randevoo.Web.Pages.Account;

public class LoginModel : PageModel
{
    public IActionResult OnGet(string? returnUrl)
    {
        return RedirectToPage("/Platform/Account/Login", new { returnUrl });
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/Platform/Account/Login");
    }
}
