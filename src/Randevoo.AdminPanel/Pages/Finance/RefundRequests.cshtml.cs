using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Finance;

[Authorize(Policy = Policies.AdminPlannerOrSupport)]
public class RefundRequestsModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public RefundRequestsModel(IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _session = session;
    }

    public bool IsReviewer => CurrentUser().Role is AdminRole.Admin or AdminRole.SupportTeam;
    public bool IsRtl => _session.IsRtl;
    public IReadOnlyList<TicketRefundRequestItem> Requests { get; private set; } = Array.Empty<TicketRefundRequestItem>();

    [BindProperty]
    public TicketRefundReviewInput ReviewInput { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Requests = await _financeApi.GetTicketRefundRequestsAsync(CurrentUser(), cancellationToken);
    }

    public async Task<IActionResult> OnPostApproveAsync(long id, CancellationToken cancellationToken)
    {
        await _financeApi.ApproveTicketRefundRequestAsync(CurrentUser(), id, ReviewInput, cancellationToken);
        StatusMessage = "درخواست بازگشت وجه تایید شد و مبلغ به کیف پول خریدار اضافه شد.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(long id, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ReviewInput.ReviewNote))
        {
            ModelState.AddModelError(nameof(ReviewInput.ReviewNote), "برای رد درخواست، توضیح بررسی را وارد کنید.");
            Requests = await _financeApi.GetTicketRefundRequestsAsync(CurrentUser(), cancellationToken);
            return Page();
        }

        await _financeApi.RejectTicketRefundRequestAsync(CurrentUser(), id, ReviewInput.ReviewNote, cancellationToken);
        StatusMessage = "درخواست بازگشت وجه رد شد.";
        return RedirectToPage();
    }

    public static string StatusLabel(TicketRefundRequestStatus status) => status switch
    {
        TicketRefundRequestStatus.Approved => "تایید شده",
        TicketRefundRequestStatus.Rejected => "رد شده",
        TicketRefundRequestStatus.Cancelled => "لغو شده",
        _ => "در انتظار بررسی"
    };

    public static string StatusClass(TicketRefundRequestStatus status) => status switch
    {
        TicketRefundRequestStatus.Approved => "status-approved",
        TicketRefundRequestStatus.Rejected => "status-rejected",
        TicketRefundRequestStatus.Cancelled => "status-archived",
        _ => "status-pending"
    };

    private MockUser CurrentUser() => _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
}
