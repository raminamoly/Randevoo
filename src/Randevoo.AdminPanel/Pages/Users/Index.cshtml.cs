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

    public IndexModel(IUsersApiClient usersApi)
    {
        _usersApi = usersApi;
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

    public SelectList RoleOptions => new(new[] { AdminRole.Admin, AdminRole.EventPlanner }
        .Select(role => new { Value = role, Text = DisplayFormatter.Role(role) }), "Value", "Text");

    public async Task<IActionResult> OnGetAsync()
    {
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
        await _usersApi.UpsertUserAsync(Input, UserId);
        return RedirectToPage("/Users/Index");
    }
}
