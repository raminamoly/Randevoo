using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Platform;

public class IndexModel : PageModel
{
    private readonly EndUserSessionService _session;

    public IndexModel(EndUserSessionService session)
    {
        _session = session;
    }

    public IActionResult OnGet()
    {
        return _session.IsSignedIn
            ? RedirectToPage("/Platform/Events/Index")
            : RedirectToPage("/Platform/Account/Login", new { returnUrl = "/platform" });
    }
}
