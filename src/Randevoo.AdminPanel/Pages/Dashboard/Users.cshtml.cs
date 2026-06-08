using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Dashboard;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Dashboard;

[Authorize(Policy = Policies.AdminOnly)]
public class UsersModel : PageModel
{
    private readonly IAdminAnalyticsApiClient _analyticsApi;
    private readonly CurrentSessionState _session;

    public UsersModel(IAdminAnalyticsApiClient analyticsApi, CurrentSessionState session)
    {
        _analyticsApi = analyticsApi;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public string RangeKey { get; set; } = DashboardDateRange.Default;

    public DashboardDateRangeValue Range { get; private set; } = DashboardDateRange.Resolve(DashboardDateRange.Default);

    public UserDashboardReport Report { get; private set; } = new();

    public DashboardFilterViewModel FilterModel => new()
    {
        Title = "داشبورد شرکت‌کنندگان",
        Description = "روند رشد، فعالیت و کیفیت تکمیل پروفایل شرکت‌کنندگان.",
        CurrentRangeKey = Range.Key,
        PagePath = "/Dashboard/Users"
    };

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Range = DashboardDateRange.Resolve(RangeKey);
        Report = await _analyticsApi.GetUserDashboardAsync(current, Range);
    }
}
