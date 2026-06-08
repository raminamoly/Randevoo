using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Users;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IUsersApiClient _usersApi;
    private readonly ILocationsApiClient _locationsApi;

    public IndexModel(IUsersApiClient usersApi, ILocationsApiClient locationsApi)
    {
        _usersApi = usersApi;
        _locationsApi = locationsApi;
    }

    [BindProperty]
    public UserUpsertInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public long? Id { get; set; }

    public IReadOnlyList<MockUser> Users { get; private set; } = Array.Empty<MockUser>();

    public long? UserId
    {
        get => Id;
        set => Id = value;
    }

    public SelectList RoleOptions { get; private set; } = new(Array.Empty<object>());

    public async Task<IActionResult> OnGetAsync()
    {
        await LoadOptionsAsync();
        Users = await _usersApi.GetUsersAsync();

        if (Id is long userId)
        {
            var user = await _usersApi.GetUserAsync(userId);
            if (user is null)
            {
                return NotFound();
            }

            Input = new UserUpsertInput
            {
                FullName = user.FullName,
                Mobile = user.Mobile,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadOptionsAsync();
        await _usersApi.UpsertUserAsync(Input, UserId);
        return RedirectToPage("/Users/Index");
    }

    private async Task LoadOptionsAsync()
    {
        var roles = (await _locationsApi.GetUserRolesAsync())
            .Where(role => role.Name is nameof(AdminRole.Admin) or nameof(AdminRole.EventPlanner))
            .ToList();
        RoleOptions = new SelectList(roles, nameof(SystemLookupOption.Value), nameof(SystemLookupOption.DisplayNameFa), Input.Role.ToString());
    }
}
