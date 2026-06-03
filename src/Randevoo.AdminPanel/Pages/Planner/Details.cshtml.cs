using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;

namespace Randevoo.AdminPanel.Pages.Planner;

[AllowAnonymous]
public class DetailsModel : PageModel
{
    private readonly IPlannerProfilesApiClient _profilesApi;

    public DetailsModel(IPlannerProfilesApiClient profilesApi)
    {
        _profilesApi = profilesApi;
    }

    public PlannerProfileViewModel Profile { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var profile = await _profilesApi.GetByUserIdAsync(id);
        if (profile is null)
        {
            return NotFound();
        }

        Profile = profile;
        return Page();
    }
}
