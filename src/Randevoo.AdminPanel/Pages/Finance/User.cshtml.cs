using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Pages.Finance;

[Authorize(Policy = Policies.AdminOnly)]
public class UserModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public UserModel(IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _session = session;
    }

    public UserFinanceOverview Overview { get; private set; } = new();

    public bool IsRtl => _session.IsRtl;

    public string ViewMode { get; private set; } = "balance";

    public async Task OnGetAsync(long userId, string? view = null)
    {
        var current = _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
        Overview = await _financeApi.GetUserFinanceAsync(current, userId);
        ViewMode = string.Equals(view, "payments", StringComparison.OrdinalIgnoreCase) ? "payments" : "balance";
    }

    public string TransactionTypeTitle(BalanceTransactionType type) => type switch
    {
        BalanceTransactionType.TicketPurchase => "خرید بلیت",
        BalanceTransactionType.TicketRefund => "بازگشت وجه",
        BalanceTransactionType.EventPlannerIncome => "درآمد برگزارکننده",
        BalanceTransactionType.EventPlannerIncomeReversal => "برگشت درآمد برگزارکننده",
        BalanceTransactionType.PlannerWithdrawalPayout => "تسویه برگزارکننده",
        BalanceTransactionType.EmergencyRemovalRefund => "بازگشت اضطراری",
        BalanceTransactionType.ManualReceiptWalletCredit => "اعتبار کیف پول بابت رسید دستی",
        BalanceTransactionType.OrganizerManualReceiptLiability => "بدهی برگزارکننده بابت رسید دستی",
        BalanceTransactionType.ManualWalletCredit => "شارژ دستی کیف پول",
        BalanceTransactionType.ManualWalletDebit => "کسر دستی کیف پول",
        BalanceTransactionType.ManualTicketPurchaseDebit => "کسر کیف پول بابت صدور دستی بلیت",
        BalanceTransactionType.AdminAdjustment => "اصلاح مدیر",
        _ => type.ToString()
    };

    public string PaymentStatusTitle(OnlinePaymentStatus status) => status switch
    {
        OnlinePaymentStatus.Pending => "در انتظار",
        OnlinePaymentStatus.Succeeded => "موفق",
        OnlinePaymentStatus.Failed => "ناموفق",
        OnlinePaymentStatus.Refunded => "برگشت خورده",
        _ => status.ToString()
    };

    public string PaymentStatusClass(OnlinePaymentStatus status) => status switch
    {
        OnlinePaymentStatus.Succeeded => "status-approved",
        OnlinePaymentStatus.Failed => "status-cancelled",
        OnlinePaymentStatus.Refunded => "status-closed",
        _ => "status-draft"
    };
}
