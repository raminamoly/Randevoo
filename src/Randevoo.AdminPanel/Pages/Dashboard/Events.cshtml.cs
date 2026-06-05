using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Dashboard;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Dashboard;

[Authorize(Policy = Policies.AdminOnly)]
public class EventsModel : PageModel
{
    private readonly IAdminAnalyticsApiClient _analyticsApi;
    private readonly CurrentSessionState _session;

    public EventsModel(IAdminAnalyticsApiClient analyticsApi, CurrentSessionState session)
    {
        _analyticsApi = analyticsApi;
        _session = session;
    }

    [BindProperty(SupportsGet = true)]
    public string RangeKey { get; set; } = DashboardDateRange.Default;

    public DashboardDateRangeValue Range { get; private set; } = DashboardDateRange.Resolve(DashboardDateRange.Default);

    public EventDashboardReport Report { get; private set; } = new();

    public DashboardFilterViewModel FilterModel => new()
    {
        Title = "داشبورد رویدادها",
        Description = "نمای کلی وضعیت ایجاد، فروش و عملکرد برگزارکنندگان.",
        CurrentRangeKey = Range.Key,
        PagePath = "/Dashboard/Events"
    };

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        Range = DashboardDateRange.Resolve(RangeKey);
        Report = await _analyticsApi.GetEventDashboardAsync(current, Range);
    }
}
