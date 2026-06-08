using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Planner;

[Authorize(Policy = Policies.AdminOrPlanner)]
public class BankAccountsModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly IPlannerProfilesApiClient _profilesApi;
    private readonly CurrentSessionState _session;

    public BankAccountsModel(IFinanceApiClient financeApi, IPlannerProfilesApiClient profilesApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _profilesApi = profilesApi;
        _session = session;
    }

    public long PlannerUserId { get; private set; }

    public IReadOnlyList<PlannerBankAccountItem> Accounts { get; private set; } = Array.Empty<PlannerBankAccountItem>();

    public string SettlementCurrencyCode { get; private set; } = "IRR";

    public bool IsIrrSettlement => string.Equals(SettlementCurrencyCode, "IRR", StringComparison.OrdinalIgnoreCase);

    [BindProperty(SupportsGet = true)]
    public long? EditAccountId { get; set; }

    [BindProperty]
    public PlannerBankAccountInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(long? plannerUserId)
    {
        PlannerUserId = ResolvePlannerUserId(plannerUserId);
        await LoadPlannerCurrencyAsync();
        Accounts = await _financeApi.GetPlannerBankAccountsAsync(CurrentUser(), PlannerUserId);
        Input.CurrencyCode = SettlementCurrencyCode;
        Input.PayoutMethod = IsIrrSettlement ? PlannerPayoutMethod.IranianBankCard : PlannerPayoutMethod.IbanSwift;
        LoadEditingAccount();
    }

    public async Task<IActionResult> OnPostSaveAsync(long? plannerUserId)
    {
        PlannerUserId = ResolvePlannerUserId(plannerUserId);
        await LoadPlannerCurrencyAsync();
        NormalizePaymentInput();
        ValidatePaymentInput();
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
            CurrencyCode = account.CurrencyCode,
            PayoutMethod = Enum.TryParse<PlannerPayoutMethod>(account.PayoutMethod, out var payoutMethod) ? payoutMethod : PlannerPayoutMethod.IranianBankCard,
            AccountHolderName = account.AccountHolderName,
            Country = account.Country,
            CardNumber = account.CardNumber,
            Iban = account.Iban,
            BankName = account.BankName,
            AccountNumber = account.AccountNumber,
            SwiftCode = account.SwiftCode,
            AccountIdentifier = account.AccountIdentifier,
            PublicPaymentInstructions = account.PublicPaymentInstructions,
            IsActive = account.IsActive
        };
    }

    private async Task LoadPlannerCurrencyAsync()
    {
        var profile = await _profilesApi.GetByUserIdAsync(PlannerUserId);
        SettlementCurrencyCode = profile?.SettlementCurrencyCode ?? "IRR";
    }

    private void NormalizePaymentInput()
    {
        Input.CurrencyCode = SettlementCurrencyCode;
        if (string.IsNullOrWhiteSpace(Input.AccountHolderName))
            Input.AccountHolderName = CurrentUser().FullName;

        if (IsIrrSettlement)
        {
            Input.PayoutMethod = PlannerPayoutMethod.IranianBankCard;
            Input.Country = "ایران";
            Input.AccountIdentifier = null;
            Input.SwiftCode = null;
            Input.PublicPaymentInstructions = null;
            return;
        }

        Input.CardNumber = null;
    }

    private void ValidatePaymentInput()
    {
        if (string.IsNullOrWhiteSpace(Input.AccountHolderName))
            ModelState.AddModelError(nameof(Input.AccountHolderName), "نام صاحب حساب الزامی است.");

        if (IsIrrSettlement)
        {
            if (string.IsNullOrWhiteSpace(Input.CardNumber))
                ModelState.AddModelError(nameof(Input.CardNumber), "شماره کارت الزامی است.");
            if (string.IsNullOrWhiteSpace(Input.Iban))
                ModelState.AddModelError(nameof(Input.Iban), "شماره شبا الزامی است.");
            if (string.IsNullOrWhiteSpace(Input.BankName))
                ModelState.AddModelError(nameof(Input.BankName), "نام بانک الزامی است.");
            return;
        }

        var hasForeignPaymentTarget = !string.IsNullOrWhiteSpace(Input.Iban)
            || !string.IsNullOrWhiteSpace(Input.SwiftCode)
            || !string.IsNullOrWhiteSpace(Input.AccountIdentifier)
            || !string.IsNullOrWhiteSpace(Input.PublicPaymentInstructions);

        if (!hasForeignPaymentTarget)
            ModelState.AddModelError(nameof(Input.PublicPaymentInstructions), "برای ارز خارجی، IBAN/SWIFT، شناسه حساب یا توضیحات پرداخت را وارد کنید.");
    }
}
