using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Randevoo.Web.Pages.Tickets;

public class IndexModel : PageModel
{
    public IActionResult OnGet(string? purchase)
    {
        return RedirectToPage("/Platform/Tickets/Index", new { purchase });
    }
}
