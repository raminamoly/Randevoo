using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Finance;

[Authorize(Policy = Policies.AdminOnly)]
public class IndexModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public IndexModel(IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _session = session;
    }

    [BindProperty]
    public WithdrawalReviewInput ReviewInput { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public IReadOnlyList<PlannerWithdrawalRequestItem> WithdrawalRequests { get; private set; } = Array.Empty<PlannerWithdrawalRequestItem>();

    public IReadOnlyList<AdminEventTicketTransactionGroup> EventTransactions { get; private set; } = Array.Empty<AdminEventTicketTransactionGroup>();

    public bool IsRtl => _session.IsRtl;

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostConfirmWithdrawalAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");

        try
        {
            await _financeApi.ConfirmWithdrawalAsync(current, ReviewInput.RequestId, ReviewInput.ReviewNote);
            StatusMessage = "درخواست تسویه تایید و تراکنش پرداخت ثبت شد.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/Finance/Index");
    }

    public async Task<IActionResult> OnPostRejectWithdrawalAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");

        try
        {
            await _financeApi.RejectWithdrawalAsync(current, ReviewInput.RequestId, ReviewInput.ReviewNote);
            StatusMessage = "درخواست تسویه رد شد.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage("/Finance/Index");
    }

    public string WithdrawalStatusClass(PlannerWithdrawalRequestStatus status) => status switch
    {
        PlannerWithdrawalRequestStatus.Confirmed => "status-approved",
        PlannerWithdrawalRequestStatus.Rejected => "status-rejected",
        _ => "status-pending"
    };

    private async Task LoadAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        WithdrawalRequests = await _financeApi.GetWithdrawalRequestsAsync(current);
        EventTransactions = await _financeApi.GetTicketPurchaseTransactionsByEventAsync(current);
    }
}
