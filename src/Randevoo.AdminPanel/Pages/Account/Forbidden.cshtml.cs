using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Randevoo.AdminPanel.Pages.Account;

[AllowAnonymous]
public class ForbiddenModel : PageModel
{
}
