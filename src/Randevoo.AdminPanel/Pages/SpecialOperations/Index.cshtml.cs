using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.SpecialOperations;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.Permissions;
using Randevoo.AdminPanel.Services.State;

namespace Randevoo.AdminPanel.Pages.SpecialOperations;

[Authorize(Policy = Policies.SupportOrAdmin)]
public class IndexModel : PageModel
{
    private const string PermissionEntity = "specialOperations";

    private readonly ISpecialOperationsApiClient _specialOperationsApi;
    private readonly IOperationPermissionService _permissions;
    private readonly CurrentSessionState _session;

    public IndexModel(
        ISpecialOperationsApiClient specialOperationsApi,
        IOperationPermissionService permissions,
        CurrentSessionState session)
    {
        _specialOperationsApi = specialOperationsApi;
        _permissions = permissions;
        _session = session;
    }

    [BindProperty]
    public CancelTicketRefundInput CancelTicketInput { get; set; } = new();

    [BindProperty]
    public ManualIssueTicketInput ManualIssueInput { get; set; } = new();

    [BindProperty]
    public ManualWalletAdjustmentInput WalletCreditInput { get; set; } = new();

    [BindProperty]
    public ManualWalletAdjustmentInput WalletDebitInput { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public UserReportListFilter ReportFilter { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public long? SelectedReportedUserId { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool ReportsTab { get; set; }

    [BindProperty]
    public ReviewUserReportInput ReviewReportInput { get; set; } = new();

    [BindProperty]
    public RestrictTicketPurchaseInput RestrictInput { get; set; } = new();

    [BindProperty]
    public RemoveTicketPurchaseRestrictionInput RemoveRestrictionInput { get; set; } = new();

    [BindProperty]
    public SendUserReportWarningInput WarningInput { get; set; } = new();

    [BindProperty]
    public SendUserReportNotificationInput NotificationInput { get; set; } = new();

    [BindProperty]
    public DeactivateReportedUserInput DeactivateUserInput { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public string? ErrorMessage { get; private set; }
    public string ActiveTab { get; private set; } = "ticket";
    public string? PreviewKey { get; private set; }
    public SpecialOperationPreview? Preview { get; private set; }
    public IReadOnlySet<string> AllowedActions { get; private set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public IReadOnlyList<SpecialOperationHistoryItem> History { get; private set; } = Array.Empty<SpecialOperationHistoryItem>();
    public ReportedUserListResult ReportedUsers { get; private set; } = new();
    public ReportedUserDetails? SelectedReportedUser { get; private set; }
    public bool IsRtl => _session.IsRtl;

    public bool CanCancelTicket => AllowedActions.Contains("cancelTicketRefundToWallet");
    public bool CanManualIssue => AllowedActions.Contains("manualIssueTicketWithWalletDebit");
    public bool CanWalletCredit => AllowedActions.Contains("manualWalletCredit");
    public bool CanWalletDebit => AllowedActions.Contains("manualWalletDebit");
    public bool CanViewHistory => AllowedActions.Contains("viewAuditLog");
    public bool CanViewUserReports => AllowedActions.Contains("userReportsView");
    public bool CanReviewUserReports => AllowedActions.Contains("userReportsReview");
    public bool CanRestrictTicketPurchase => AllowedActions.Contains("userReportsRestrictTicketPurchase");
    public bool CanRemoveTicketPurchaseRestriction => AllowedActions.Contains("userReportsRemoveRestriction");
    public bool CanSendUserReportWarning => AllowedActions.Contains("userReportsSendWarning");
    public bool CanSendUserReportNotification => AllowedActions.Contains("userReportsSendNotification");
    public bool CanDeactivateReportedUser => AllowedActions.Contains("userReportsDeactivateUser");

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (ReportsTab
            || SelectedReportedUserId is not null
            || !string.IsNullOrWhiteSpace(ReportFilter.SearchTerm)
            || ReportFilter.Status is not null
            || ReportFilter.MinimumOpenReports is not null)
        {
            ActiveTab = "reports";
        }

        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        return Page();
    }

    public async Task<IActionResult> OnPostPreviewCancelTicketAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "ticket";
        return await PreviewAsync(
            nameof(CancelTicketInput),
            async ct => Preview = await _specialOperationsApi.PreviewCancelTicketRefundAsync(CurrentUser(), CancelTicketInput, ct),
            "cancel",
            cancellationToken);
    }

    public async Task<IActionResult> OnPostExecuteCancelTicketAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "ticket";
        return await ExecuteAsync(
            nameof(CancelTicketInput),
            async ct => await _specialOperationsApi.ExecuteCancelTicketRefundAsync(CurrentUser(), CancelTicketInput, ct),
            cancellationToken);
    }

    public async Task<IActionResult> OnPostPreviewManualIssueAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "issue";
        return await PreviewAsync(
            nameof(ManualIssueInput),
            async ct => Preview = await _specialOperationsApi.PreviewManualIssueTicketAsync(CurrentUser(), ManualIssueInput, ct),
            "issue",
            cancellationToken);
    }

    public async Task<IActionResult> OnPostExecuteManualIssueAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "issue";
        return await ExecuteAsync(
            nameof(ManualIssueInput),
            async ct => await _specialOperationsApi.ExecuteManualIssueTicketAsync(CurrentUser(), ManualIssueInput, ct),
            cancellationToken);
    }

    public async Task<IActionResult> OnPostPreviewWalletCreditAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "wallet-credit";
        return await PreviewAsync(
            nameof(WalletCreditInput),
            async ct => Preview = await _specialOperationsApi.PreviewManualWalletCreditAsync(CurrentUser(), WalletCreditInput, ct),
            "credit",
            cancellationToken);
    }

    public async Task<IActionResult> OnPostExecuteWalletCreditAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "wallet-credit";
        return await ExecuteAsync(
            nameof(WalletCreditInput),
            async ct => await _specialOperationsApi.ExecuteManualWalletCreditAsync(CurrentUser(), WalletCreditInput, ct),
            cancellationToken);
    }

    public async Task<IActionResult> OnPostPreviewWalletDebitAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "wallet-debit";
        return await PreviewAsync(
            nameof(WalletDebitInput),
            async ct => Preview = await _specialOperationsApi.PreviewManualWalletDebitAsync(CurrentUser(), WalletDebitInput, ct),
            "debit",
            cancellationToken);
    }

    public async Task<IActionResult> OnPostExecuteWalletDebitAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "wallet-debit";
        return await ExecuteAsync(
            nameof(WalletDebitInput),
            async ct => await _specialOperationsApi.ExecuteManualWalletDebitAsync(CurrentUser(), WalletDebitInput, ct),
            cancellationToken);
    }

    public async Task<IActionResult> OnPostReviewUserReportAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "reports";
        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        if (!ValidateOnly(nameof(ReviewReportInput)))
            return await ReloadReportsPageAsync(cancellationToken);

        try
        {
            var result = await _specialOperationsApi.ReviewUserReportAsync(CurrentUser(), ReviewReportInput, cancellationToken);
            StatusMessage = $"عملیات #{DisplayFormatter.Count((int)result.OperationId)} ثبت شد. {result.Message}";
            return RedirectToPage(new { ReportsTab = true, SelectedReportedUserId });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await LoadReportedUsersAsync(cancellationToken);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostPreviewRestrictTicketPurchaseAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "reports";
        SelectedReportedUserId = RestrictInput.UserId;
        return await PreviewAsync(
            nameof(RestrictInput),
            async ct => Preview = await _specialOperationsApi.PreviewRestrictTicketPurchaseAsync(CurrentUser(), RestrictInput, ct),
            "restrict-ticket-purchase",
            cancellationToken);
    }

    public async Task<IActionResult> OnPostExecuteRestrictTicketPurchaseAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "reports";
        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        if (!ValidateOnly(nameof(RestrictInput)))
            return await ReloadReportsPageAsync(cancellationToken);

        try
        {
            var result = await _specialOperationsApi.ExecuteRestrictTicketPurchaseAsync(CurrentUser(), RestrictInput, cancellationToken);
            StatusMessage = $"عملیات #{DisplayFormatter.Count((int)result.OperationId)} ثبت شد. {result.Message}";
            return RedirectToPage(new { ReportsTab = true, SelectedReportedUserId = RestrictInput.UserId });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            SelectedReportedUserId = RestrictInput.UserId;
            await LoadReportedUsersAsync(cancellationToken);
            await LoadHistoryAsync(cancellationToken);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostRemoveTicketPurchaseRestrictionAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "reports";
        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        if (!ValidateOnly(nameof(RemoveRestrictionInput)))
            return await ReloadReportsPageAsync(cancellationToken);

        try
        {
            var result = await _specialOperationsApi.RemoveTicketPurchaseRestrictionAsync(CurrentUser(), RemoveRestrictionInput, cancellationToken);
            StatusMessage = $"عملیات #{DisplayFormatter.Count((int)result.OperationId)} ثبت شد. {result.Message}";
            return RedirectToPage(new { ReportsTab = true, SelectedReportedUserId = RemoveRestrictionInput.UserId });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            SelectedReportedUserId = RemoveRestrictionInput.UserId;
            await LoadReportedUsersAsync(cancellationToken);
            await LoadHistoryAsync(cancellationToken);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostSendUserReportWarningAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "reports";
        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        if (!ValidateOnly(nameof(WarningInput)))
            return await ReloadReportsPageAsync(cancellationToken);

        try
        {
            var result = await _specialOperationsApi.SendUserReportWarningAsync(CurrentUser(), WarningInput, cancellationToken);
            StatusMessage = $"عملیات #{DisplayFormatter.Count((int)result.OperationId)} ثبت شد. {result.Message}";
            return RedirectToPage(new { ReportsTab = true, SelectedReportedUserId = WarningInput.UserId });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            SelectedReportedUserId = WarningInput.UserId;
            await LoadReportedUsersAsync(cancellationToken);
            await LoadHistoryAsync(cancellationToken);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostSendUserReportNotificationAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "reports";
        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        if (!ValidateOnly(nameof(NotificationInput)))
            return await ReloadReportsPageAsync(cancellationToken);

        try
        {
            var result = await _specialOperationsApi.SendUserReportNotificationAsync(CurrentUser(), NotificationInput, cancellationToken);
            StatusMessage = $"عملیات #{DisplayFormatter.Count((int)result.OperationId)} ثبت شد. {result.Message}";
            return RedirectToPage(new { ReportsTab = true, SelectedReportedUserId = NotificationInput.UserId });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            SelectedReportedUserId = NotificationInput.UserId;
            await LoadReportedUsersAsync(cancellationToken);
            await LoadHistoryAsync(cancellationToken);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostDeactivateReportedUserAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "reports";
        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        if (!ValidateOnly(nameof(DeactivateUserInput)))
            return await ReloadReportsPageAsync(cancellationToken);

        try
        {
            var result = await _specialOperationsApi.DeactivateReportedUserAsync(CurrentUser(), DeactivateUserInput, cancellationToken);
            StatusMessage = $"عملیات #{DisplayFormatter.Count((int)result.OperationId)} ثبت شد. {result.Message}";
            return RedirectToPage(new { ReportsTab = true, SelectedReportedUserId = DeactivateUserInput.UserId });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            SelectedReportedUserId = DeactivateUserInput.UserId;
            await LoadReportedUsersAsync(cancellationToken);
            await LoadHistoryAsync(cancellationToken);
            return Page();
        }
    }

    public static string OperationTypeLabel(string operationType) => operationType switch
    {
        "CancelTicketRefundToWallet" => "کنسل بلیت و برگشت به کیف پول",
        "ManualIssueTicketWithWalletDebit" => "صدور دستی بلیت",
        "ManualWalletCredit" => "شارژ دستی کیف پول",
        "ManualWalletDebit" => "کسر دستی کیف پول",
        "UserReportReviewed" => "بررسی گزارش کاربر",
        "UserTicketPurchaseRestricted" => "محدود کردن خرید بلیت",
        "UserTicketPurchaseRestrictionRemoved" => "برداشتن محدودیت خرید",
        "UserWarningNotificationSent" => "ارسال هشدار کاربر",
        "UserReportNotificationSent" => "ارسال نوتیفیکیشن کاربر",
        "ReportedUserDeactivated" => "غیرفعال کردن کاربر",
        _ => operationType
    };

    public static string ReportStatusLabel(Domain.Enums.ModerationReportStatus status) => status switch
    {
        Domain.Enums.ModerationReportStatus.Pending => "باز",
        Domain.Enums.ModerationReportStatus.Reviewed => "بررسی‌شده",
        Domain.Enums.ModerationReportStatus.Dismissed => "رد شده",
        Domain.Enums.ModerationReportStatus.ActionTaken => "اقدام شده",
        _ => status.ToString()
    };

    public static string ReportReasonLabel(Domain.Enums.ModerationReportReason reason) => reason switch
    {
        Domain.Enums.ModerationReportReason.Harassment => "آزار یا مزاحمت",
        Domain.Enums.ModerationReportReason.UnsafeBehavior => "رفتار ناامن",
        Domain.Enums.ModerationReportReason.FakeProfile => "پروفایل جعلی",
        Domain.Enums.ModerationReportReason.Spam => "اسپم",
        Domain.Enums.ModerationReportReason.InappropriateContent => "محتوای نامناسب",
        Domain.Enums.ModerationReportReason.Other => "سایر",
        _ => reason.ToString()
    };

    public static string StatusClass(string status) => status switch
    {
        "Succeeded" => "status-approved",
        "Failed" => "status-rejected",
        "Pending" => "status-pending",
        _ => "status-draft"
    };

    public static string StatusLabel(string status) => status switch
    {
        "Succeeded" => "موفق",
        "Failed" => "ناموفق",
        "Pending" => "در حال اجرا",
        _ => status
    };

    private async Task<IActionResult> PreviewAsync(string modelPrefix, Func<CancellationToken, Task> previewAction, string previewKey, CancellationToken cancellationToken)
    {
        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        if (!ValidateOnly(modelPrefix))
            return Page();

        try
        {
            await previewAction(cancellationToken);
            PreviewKey = previewKey;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        await LoadHistoryAsync(cancellationToken);
        return Page();
    }

    private async Task<IActionResult> ExecuteAsync(string modelPrefix, Func<CancellationToken, Task<SpecialOperationExecuteResult>> executeAction, CancellationToken cancellationToken)
    {
        if (!await LoadPageStateAsync(cancellationToken))
            return Forbid();

        if (!ValidateOnly(modelPrefix))
            return Page();

        try
        {
            var result = await executeAction(cancellationToken);
            StatusMessage = $"عملیات #{DisplayFormatter.Count((int)result.OperationId)} ثبت شد. {result.Message}";
            return RedirectToPage();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await LoadHistoryAsync(cancellationToken);
            return Page();
        }
    }

    private bool ValidateOnly(string modelPrefix)
    {
        ModelState.Clear();
        object? model = modelPrefix switch
        {
            nameof(CancelTicketInput) => CancelTicketInput,
            nameof(ManualIssueInput) => ManualIssueInput,
            nameof(WalletCreditInput) => WalletCreditInput,
            nameof(WalletDebitInput) => WalletDebitInput,
            nameof(ReviewReportInput) => ReviewReportInput,
            nameof(RestrictInput) => RestrictInput,
            nameof(RemoveRestrictionInput) => RemoveRestrictionInput,
            nameof(WarningInput) => WarningInput,
            nameof(NotificationInput) => NotificationInput,
            nameof(DeactivateUserInput) => DeactivateUserInput,
            _ => null
        };

        return model is not null && TryValidateModel(model, modelPrefix);
    }

    private async Task<bool> LoadPageStateAsync(CancellationToken cancellationToken)
    {
        var current = CurrentUser();
        AllowedActions = await _permissions.GetAllowedActionsAsync(current, PermissionEntity, cancellationToken);
        if (!AllowedActions.Contains("view"))
            return false;

        await LoadHistoryAsync(cancellationToken);
        await LoadReportedUsersAsync(cancellationToken);
        return true;
    }

    private async Task LoadHistoryAsync(CancellationToken cancellationToken)
    {
        History = CanViewHistory
            ? await _specialOperationsApi.ListHistoryAsync(CurrentUser(), cancellationToken)
            : Array.Empty<SpecialOperationHistoryItem>();
    }

    private async Task LoadReportedUsersAsync(CancellationToken cancellationToken)
    {
        if (!CanViewUserReports)
        {
            ReportedUsers = new ReportedUserListResult();
            SelectedReportedUser = null;
            return;
        }

        ReportedUsers = await _specialOperationsApi.ListReportedUsersAsync(CurrentUser(), ReportFilter, cancellationToken);
        SelectedReportedUser = SelectedReportedUserId is null
            ? null
            : await _specialOperationsApi.GetReportedUserDetailsAsync(CurrentUser(), SelectedReportedUserId.Value, cancellationToken);
    }

    private async Task<IActionResult> ReloadReportsPageAsync(CancellationToken cancellationToken)
    {
        ActiveTab = "reports";
        await LoadReportedUsersAsync(cancellationToken);
        await LoadHistoryAsync(cancellationToken);
        return Page();
    }

    private MockUser CurrentUser() => _session.CurrentUser ?? throw new InvalidOperationException("حساب جاری شناسایی نشد.");
}
