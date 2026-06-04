using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class MyModel : PageModel
{
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public MyModel(IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _eventsApi = eventsApi;
        _session = session;
    }

    public IReadOnlyList<DatingEvent> Events { get; private set; } = Array.Empty<DatingEvent>();

    public bool IsRtl => _session.IsRtl;

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        Events = await _eventsApi.GetEventsAsync(current);
    }

    public async Task<IActionResult> OnPostOpenSaleAsync(long id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ToggleSaleAsync(id, current, true);
        StatusMessage = "فروش رویداد باز شد.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCloseSaleAsync(long id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.ToggleSaleAsync(id, current, false);
        StatusMessage = "فروش رویداد بسته شد.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(long id)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        await _eventsApi.CancelAsync(id, current);
        StatusMessage = "رویداد لغو شد.";
        return RedirectToPage();
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
