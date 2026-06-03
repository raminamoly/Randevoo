using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public IndexModel(IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _session = session;
    }

    public IReadOnlyList<DatingEvent> Events { get; private set; } = Array.Empty<DatingEvent>();

    public bool IsRtl => _session.IsRtl;

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("Current user was not resolved.");
        Events = await _eventsApi.GetEventsAsync(current);
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
