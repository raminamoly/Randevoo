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
            if (User.IsInRole(AdminRole.Admin.ToString()))
                return RedirectToPage("/Events/Index");

            if (User.IsInRole(AdminRole.EventPlanner.ToString()))
                return RedirectToPage("/Events/My");

            if (User.IsInRole(AdminRole.SupportTeam.ToString()))
                return RedirectToPage("/Participants/Index");

            return RedirectToPage("/Account/Forbidden");
        }

        return RedirectToPage("/Account/Login");
    }
}
