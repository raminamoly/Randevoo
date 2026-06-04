using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.UserProfiles;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class DetailsModel : PageModel
{
    private readonly IUserProfilesApiClient _profilesApi;
    private readonly CurrentSessionState _session;

    public DetailsModel(IUserProfilesApiClient profilesApi, CurrentSessionState session)
    {
        _profilesApi = profilesApi;
        _session = session;
    }

    public UserProfileDetailsViewModel Profile { get; private set; } = new();

    public bool IsRtl => _session.IsRtl;

    public async Task<IActionResult> OnGetAsync(long userId)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        var profile = await _profilesApi.GetProfileAsync(userId, current);
        if (profile is null)
            return NotFound();

        Profile = profile;
        return Page();
    }

    public string TicketStatus(UserProfileTicketItem ticket)
    {
        if (ticket.IsRemoved)
            return "حذف اضطراری";

        return ticket.IsRefunded ? "بازگشت وجه" : "فعال";
    }

    public string TicketStatusClass(UserProfileTicketItem ticket)
    {
        if (ticket.IsRemoved)
            return "status-cancelled";

        return ticket.IsRefunded ? "status-closed" : "status-approved";
    }
}
