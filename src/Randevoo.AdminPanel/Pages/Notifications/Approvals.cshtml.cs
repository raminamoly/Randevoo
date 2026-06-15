using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Notifications;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Notifications;

[Authorize(Policy = Policies.SupportOrAdmin)]
public class ApprovalsModel : PageModel
{
    private readonly INotificationsApiClient _notificationsApi;
    private readonly CurrentSessionState _session;

    public ApprovalsModel(INotificationsApiClient notificationsApi, CurrentSessionState session)
    {
        _notificationsApi = notificationsApi;
        _session = session;
    }

    public IReadOnlyList<NotificationItem> Notifications { get; private set; } = Array.Empty<NotificationItem>();

    [BindProperty]
    public NotificationReviewInput ReviewInput { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Notifications = await _notificationsApi.GetPendingApprovalsAsync(CurrentUser(), cancellationToken);
    }

    public async Task<IActionResult> OnPostApproveAsync(long id, CancellationToken cancellationToken)
    {
        await _notificationsApi.ApproveNotificationAsync(CurrentUser(), id, ReviewInput.ReviewNote, cancellationToken);
        StatusMessage = "پیام تایید و برای گیرندگان فعال شد.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(long id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ReviewInput.ReviewNote))
        {
            ModelState.AddModelError(nameof(ReviewInput.ReviewNote), "برای رد پیام، دلیل را وارد کنید.");
            Notifications = await _notificationsApi.GetPendingApprovalsAsync(CurrentUser(), cancellationToken);
            return Page();
        }

        await _notificationsApi.RejectNotificationAsync(CurrentUser(), id, ReviewInput.ReviewNote, cancellationToken);
        StatusMessage = "پیام رد شد.";
        return RedirectToPage();
    }

    private MockUser CurrentUser() => _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
}
