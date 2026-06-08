using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.Planner;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class BankAccountsModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public BankAccountsModel(IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _session = session;
    }

    public long PlannerUserId { get; private set; }

    public IReadOnlyList<PlannerBankAccountItem> Accounts { get; private set; } = Array.Empty<PlannerBankAccountItem>();

    [BindProperty(SupportsGet = true)]
    public long? EditAccountId { get; set; }

    [BindProperty]
    public PlannerBankAccountInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(long? plannerUserId)
    {
        PlannerUserId = ResolvePlannerUserId(plannerUserId);
        Accounts = await _financeApi.GetPlannerBankAccountsAsync(CurrentUser(), PlannerUserId);
        LoadEditingAccount();
    }

    public async Task<IActionResult> OnPostSaveAsync(long? plannerUserId)
    {
        PlannerUserId = ResolvePlannerUserId(plannerUserId);
        if (!ModelState.IsValid)
        {
            Accounts = await _financeApi.GetPlannerBankAccountsAsync(CurrentUser(), PlannerUserId);
            return Page();
        }

        await _financeApi.SavePlannerBankAccountAsync(CurrentUser(), PlannerUserId, Input);
        StatusMessage = Input.Id is null ? "حساب بانکی ذخیره شد." : "حساب بانکی ویرایش شد.";
        return RedirectToPage(new { plannerUserId = PlannerUserId });
    }

    public async Task<IActionResult> OnPostToggleAsync(long? plannerUserId, long bankAccountId, bool isActive)
    {
        PlannerUserId = ResolvePlannerUserId(plannerUserId);
        await _financeApi.TogglePlannerBankAccountAsync(CurrentUser(), PlannerUserId, bankAccountId, isActive);
        StatusMessage = isActive ? "حساب بانکی فعال شد." : "حساب بانکی غیرفعال شد.";
        return RedirectToPage(new { plannerUserId = PlannerUserId });
    }

    private long ResolvePlannerUserId(long? plannerUserId)
    {
        var current = CurrentUser();
        if (current.Role == AdminRole.EventPlanner)
            return current.Id;

        return plannerUserId ?? current.Id;
    }

    private MockUser CurrentUser() => _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");

    private void LoadEditingAccount()
    {
        if (EditAccountId is not long editAccountId)
            return;

        var account = Accounts.FirstOrDefault(item => item.Id == editAccountId);
        if (account is null)
            return;

        Input = new PlannerBankAccountInput
        {
            Id = account.Id,
            CardNumber = account.CardNumber,
            Iban = account.Iban,
            BankName = account.BankName,
            IsActive = account.IsActive
        };
    }
}
