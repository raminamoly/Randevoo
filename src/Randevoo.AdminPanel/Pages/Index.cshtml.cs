using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;

namespace Randevoo.AdminPanel.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return User.IsInRole(AdminRole.EventPlanner.ToString())
                ? RedirectToPage("/Dashboard/My")
                : RedirectToPage("/Dashboard/Index");
        }

        return RedirectToPage("/Account/Login");
    }
}

