using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Platform.Notifications;

public class IndexModel : PageModel
{
    private readonly EndUserSessionService _session;

    public IndexModel(EndUserSessionService session)
    {
        _session = session;
    }

    public IActionResult OnGet()
    {
        if (!_session.IsSignedIn)
            return RedirectToPage("/Platform/Account/Login", new { returnUrl = "/platform/notifications" });

        return Page();
    }
}
