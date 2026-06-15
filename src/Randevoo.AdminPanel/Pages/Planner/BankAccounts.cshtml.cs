using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    private readonly IEventsApiClient _eventsApi;
    private readonly CurrentSessionState _session;

    public BankAccountsModel(IFinanceApiClient financeApi, IEventsApiClient eventsApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _eventsApi = eventsApi;
        _session = session;
    }

    public long PlannerUserId { get; private set; }

    public IReadOnlyList<PlannerBankAccountItem> Accounts { get; private set; } = Array.Empty<PlannerBankAccountItem>();

    public SelectList CurrencyOptions { get; private set; } = new(Array.Empty<object>());

    public bool IsIrrAccount => string.Equals(Input.CurrencyCode, "IRR", StringComparison.OrdinalIgnoreCase);

    [BindProperty(SupportsGet = true)]
    public long? EditAccountId { get; set; }

    [BindProperty]
    public PlannerBankAccountInput Input { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(long? plannerUserId)
    {
        PlannerUserId = ResolvePlannerUserId(plannerUserId);
        await LoadCurrencyOptionsAsync();
        Accounts = await _financeApi.GetPlannerBankAccountsAsync(CurrentUser(), PlannerUserId);
        Input.CurrencyCode = "IRR";
        Input.PayoutMethod = PlannerPayoutMethod.IranianBankCard;
        LoadEditingAccount();
        await LoadCurrencyOptionsAsync();
    }

    public async Task<IActionResult> OnPostSaveAsync(long? plannerUserId)
    {
        PlannerUserId = ResolvePlannerUserId(plannerUserId);
        await LoadCurrencyOptionsAsync();
        NormalizePaymentInput();
        ValidatePaymentInput();
        if (!ModelState.IsValid)
        {
            Accounts = await _financeApi.GetPlannerBankAccountsAsync(CurrentUser(), PlannerUserId);
            await LoadCurrencyOptionsAsync();
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

    private async Task LoadCurrencyOptionsAsync()
    {
        var currencies = await _eventsApi.GetCurrencyOptionsAsync();
        CurrencyOptions = new SelectList(
            currencies.Select(item => new { Code = item.Name, Title = $"{item.DisplayNameFa} ({item.Name})" }),
            "Code",
            "Title",
            Input.CurrencyCode);
    }

    private void NormalizePaymentInput()
    {
        Input.CurrencyCode = string.IsNullOrWhiteSpace(Input.CurrencyCode)
            ? "IRR"
            : Input.CurrencyCode.Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(Input.AccountHolderName))
            Input.AccountHolderName = CurrentUser().FullName;

        if (IsIrrAccount)
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

        if (IsIrrAccount)
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
