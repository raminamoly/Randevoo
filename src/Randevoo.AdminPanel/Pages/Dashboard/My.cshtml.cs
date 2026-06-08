using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Dashboard;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class MyModel : PageModel
{
    private readonly IDashboardApiClient _dashboardApi;
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public MyModel(IDashboardApiClient dashboardApi, IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _dashboardApi = dashboardApi;
        _eventsApi = eventsApi;
        _session = session;
    }

    public DashboardStats Stats { get; private set; } = new();

    public IReadOnlyList<DatingEvent> Events { get; private set; } = Array.Empty<DatingEvent>();

    public bool IsRtl => _session.IsRtl;

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Stats = await _dashboardApi.GetStatsAsync(current);
        Events = (await _eventsApi.GetEventsAsync(current)).Take(8).ToList();
    }

    public string GetOperationalStatusClass(EventOperationalStatus status) => DisplayFormatter.OperationalStatusClass(status);

    public string GetReviewStatusClass(EventReviewStatus status) => DisplayFormatter.ReviewStatusClass(status);
}
