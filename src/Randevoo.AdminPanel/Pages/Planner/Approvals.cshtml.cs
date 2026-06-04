using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Planner;

[Authorize(Policy = Policies.AdminOnly)]
public class ApprovalsModel : PageModel
{
    private readonly IPlannerProfilesApiClient _profilesApi;
    private readonly CurrentSessionState _session;

    public ApprovalsModel(IPlannerProfilesApiClient profilesApi, CurrentSessionState session)
    {
        _profilesApi = profilesApi;
        _session = session;
    }

    public IReadOnlyList<PlannerProfileApprovalItem> Profiles { get; private set; } = Array.Empty<PlannerProfileApprovalItem>();

    public bool IsRtl => _session.IsRtl;

    [TempData]
    public string? PlannerProfileReviewMessage { get; set; }

    public async Task OnGetAsync()
    {
        Profiles = await _profilesApi.ListForApprovalAsync();
    }
}
