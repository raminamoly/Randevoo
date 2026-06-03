using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Dashboard;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IDashboardApiClient _dashboardApi;
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public IndexModel(IDashboardApiClient dashboardApi, IEventsApiClient eventsApi, CurrentSessionState session)
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
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        Stats = await _dashboardApi.GetStatsAsync(current);
        Events = (await _eventsApi.GetEventsAsync(current)).Take(6).ToList();
    }

    public string GetStatusClass(EventApprovalState state) => state switch
    {
        EventApprovalState.Approved => "status-approved",
        EventApprovalState.PendingAdminReview => "status-pending",
        EventApprovalState.Rejected => "status-rejected",
        EventApprovalState.Closed => "status-closed",
        EventApprovalState.Cancelled => "status-cancelled",
        _ => "status-draft"
    };
}
