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

[Authorize(Policy = Policies.SupportOrAdmin)]
public class PaymentReceiptsModel : PageModel
{
    private readonly IFinanceApiClient _financeApi;
    private readonly CurrentSessionState _session;

    public PaymentReceiptsModel(IFinanceApiClient financeApi, CurrentSessionState session)
    {
        _financeApi = financeApi;
        _session = session;
    }

    public bool IsRtl => _session.IsRtl;

    public IReadOnlyList<ManualPaymentReceiptItem> Receipts { get; private set; } = Array.Empty<ManualPaymentReceiptItem>();

    [BindProperty]
    public ManualPaymentReceiptReviewInput ReviewInput { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostApproveAsync(CancellationToken cancellationToken)
    {
        await _financeApi.ApproveManualPaymentReceiptAsync(CurrentUser(), ReviewInput.ReceiptId, cancellationToken);
        StatusMessage = "رسید تایید شد؛ اگر رویداد فعال باشد بلیت ثبت می‌شود و اگر لغو شده باشد مبلغ به کیف پول کاربر اضافه می‌شود.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ReviewInput.RejectReason))
        {
            ModelState.AddModelError(nameof(ReviewInput.RejectReason), "برای رد رسید، دلیل را وارد کنید.");
            await LoadAsync(cancellationToken);
            return Page();
        }

        await _financeApi.RejectManualPaymentReceiptAsync(CurrentUser(), ReviewInput.ReceiptId, ReviewInput.RejectReason, cancellationToken);
        StatusMessage = "رسید رد شد.";
        return RedirectToPage();
    }

    public static string StatusLabel(ManualPaymentReceiptStatus status) => status switch
    {
        ManualPaymentReceiptStatus.Approved => "تایید شده",
        ManualPaymentReceiptStatus.Rejected => "رد شده",
        _ => "در انتظار بررسی"
    };

    public static string StatusClass(ManualPaymentReceiptStatus status) => status switch
    {
        ManualPaymentReceiptStatus.Approved => "status-approved",
        ManualPaymentReceiptStatus.Rejected => "status-rejected",
        _ => "status-pending"
    };

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        Receipts = await _financeApi.GetManualPaymentReceiptsAsync(CurrentUser(), ManualPaymentDestinationType.Platform, cancellationToken);
    }

    private MockUser CurrentUser() => _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
}
