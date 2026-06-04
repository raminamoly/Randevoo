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
public class WithdrawalsModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public WithdrawalsModel(IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _session = session;
    }

    [BindProperty]
    public WithdrawalReviewInput ReviewInput { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public PlannerWithdrawalRequestStatus? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [TempData]
    public string? StatusMessage { get; set; }

    public bool IsRtl => _session.IsRtl;
    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search) || Status is not null;
    public int PageSize { get; } = 12;
    public int TotalCount { get; private set; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public IReadOnlyList<PlannerWithdrawalRequestItem> WithdrawalRequests { get; private set; } = Array.Empty<PlannerWithdrawalRequestItem>();

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostConfirmWithdrawalAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        try
        {
            await _financeApi.ConfirmWithdrawalAsync(current, ReviewInput.RequestId, ReviewInput.ReviewNote);
            StatusMessage = "درخواست تسویه تایید و تراکنش پرداخت ثبت شد.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage(new { Search, Status, PageNumber });
    }

    public async Task<IActionResult> OnPostRejectWithdrawalAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        try
        {
            await _financeApi.RejectWithdrawalAsync(current, ReviewInput.RequestId, ReviewInput.ReviewNote);
            StatusMessage = "درخواست تسویه رد شد.";
        }
        catch (InvalidOperationException ex)
        {
            StatusMessage = ex.Message;
        }

        return RedirectToPage(new { Search, Status, PageNumber });
    }

    public string WithdrawalStatusClass(PlannerWithdrawalRequestStatus status) => status switch
    {
        PlannerWithdrawalRequestStatus.Confirmed => "status-approved",
        PlannerWithdrawalRequestStatus.Rejected => "status-rejected",
        _ => "status-pending"
    };

    private async Task LoadAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        var requests = (await _financeApi.GetWithdrawalRequestsAsync(current)).AsEnumerable();
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var query = Search.Trim();
            requests = requests.Where(item =>
                item.PlannerName.Contains(query, StringComparison.OrdinalIgnoreCase)
                || item.PlannerMobile.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        if (Status is PlannerWithdrawalRequestStatus status)
            requests = requests.Where(item => item.Status == status);

        var filtered = requests
            .OrderBy(item => item.Status)
            .ThenByDescending(item => item.RequestedAtUtc)
            .ToList();
        TotalCount = filtered.Count;
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        WithdrawalRequests = filtered
            .Skip((PageNumber - 1) * PageSize)
            .Take(PageSize)
            .ToList();
    }
}
