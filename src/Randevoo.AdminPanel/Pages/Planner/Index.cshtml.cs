using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.ApiClients;

namespace Randevoo.AdminPanel.Pages.Planner;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IUsersApiClient _usersApi;

    public IndexModel(IUsersApiClient usersApi)
    {
        _usersApi = usersApi;
    }

    public IReadOnlyList<MockUser> Planners { get; private set; } = Array.Empty<MockUser>();

    public async Task OnGetAsync()
    {
        var users = await _usersApi.GetUsersAsync();
        Planners = users
            .Where(user => user.Role == AdminRole.EventPlanner)
            .OrderBy(user => user.FullName)
            .ToList();
    }
}
