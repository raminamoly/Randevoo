using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Randevoo.Web.Pages.Profile;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("/Platform/Profile/Index");
    }

    public IActionResult OnPost()
    {
        return RedirectToPage("/Platform/Profile/Index");
    }
}
