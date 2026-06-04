using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Planner;

[AllowAnonymous]
public class DetailsModel : PageModel
{
    private readonly IPlannerProfilesApiClient _profilesApi;
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public DetailsModel(IPlannerProfilesApiClient profilesApi, IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _profilesApi = profilesApi;
        _financeApi = financeApi;
        _session = session;
    }

    public PlannerProfileViewModel Profile { get; private set; } = new();

    public IReadOnlyList<PlannerBankAccountItem> BankAccounts { get; private set; } = Array.Empty<PlannerBankAccountItem>();

    public bool CanManageBankAccounts { get; private set; }

    public async Task<IActionResult> OnGetAsync(long id)
    {
        var profile = await _profilesApi.GetByUserIdAsync(id);
        if (profile is null)
        {
            return NotFound();
        }

        Profile = profile;
        CanManageBankAccounts = CanManagePrivatePlannerData(profile.UserId);
        if (CanManageBankAccounts && _session.CurrentUser is not null)
        {
            BankAccounts = await _financeApi.GetPlannerBankAccountsAsync(_session.CurrentUser, profile.UserId);
        }

        return Page();
    }

    private bool CanManagePrivatePlannerData(long plannerUserId)
    {
        var currentUser = _session.CurrentUser;
        return currentUser is not null
            && (currentUser.Role == AdminRole.Admin
                || (currentUser.Role == AdminRole.EventPlanner && currentUser.Id == plannerUserId));
    }
}
