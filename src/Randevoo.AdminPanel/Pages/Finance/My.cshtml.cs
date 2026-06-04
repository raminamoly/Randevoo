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

[Authorize(Policy = Policies.AdminOrPlanner)]
public class MyModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public MyModel(IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _session = session;
    }

    [BindProperty]
    public WithdrawalRequestInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public PlannerFinanceDashboard Finance { get; private set; } = new();

    public bool IsRtl => _session.IsRtl;

    public async Task<IActionResult> OnGetAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        if (current.Role != AdminRole.EventPlanner)
            return RedirectToPage("/Finance/Index");

        await LoadAsync(current);
        return Page();
    }

    public async Task<IActionResult> OnPostRequestWithdrawalAsync()
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("کاربر جاری شناسایی نشد.");
        if (current.Role != AdminRole.EventPlanner)
            return RedirectToPage("/Finance/Index");

        if (Input.Amount <= 0)
            ModelState.AddModelError(nameof(Input.Amount), "مبلغ برداشت باید بیشتر از صفر باشد.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(current);
            return Page();
        }

        try
        {
            await _financeApi.RequestWithdrawalAsync(current, Input.Amount);
            StatusMessage = "درخواست تسویه ثبت شد و پس از تایید مدیر پرداخت می شود.";
            return RedirectToPage("/Finance/My");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadAsync(current);
            return Page();
        }
    }

    public string WithdrawalStatusClass(PlannerWithdrawalRequestStatus status) => status switch
    {
        PlannerWithdrawalRequestStatus.Confirmed => "status-approved",
        PlannerWithdrawalRequestStatus.Rejected => "status-rejected",
        _ => "status-pending"
    };

    private async Task LoadAsync(MockUser current)
    {
        Finance = await _financeApi.GetPlannerFinanceAsync(current);
    }
}
