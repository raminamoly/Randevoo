using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class BuyersModel : PageModel
{
    public IActionResult OnGet(long eventId)
        => RedirectToPage("/Buyers/Index", new { EventId = eventId });
}
