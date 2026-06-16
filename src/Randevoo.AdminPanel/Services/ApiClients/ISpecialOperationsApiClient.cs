using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.SpecialOperations;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface ISpecialOperationsApiClient
{
    Task<SpecialOperationPreview> PreviewCancelTicketRefundAsync(MockUser currentUser, CancelTicketRefundInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> ExecuteCancelTicketRefundAsync(MockUser currentUser, CancelTicketRefundInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationPreview> PreviewManualIssueTicketAsync(MockUser currentUser, ManualIssueTicketInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> ExecuteManualIssueTicketAsync(MockUser currentUser, ManualIssueTicketInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationPreview> PreviewManualWalletCreditAsync(MockUser currentUser, ManualWalletAdjustmentInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> ExecuteManualWalletCreditAsync(MockUser currentUser, ManualWalletAdjustmentInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationPreview> PreviewManualWalletDebitAsync(MockUser currentUser, ManualWalletAdjustmentInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> ExecuteManualWalletDebitAsync(MockUser currentUser, ManualWalletAdjustmentInput input, CancellationToken cancellationToken = default);

    Task<ReportedUserListResult> ListReportedUsersAsync(MockUser currentUser, UserReportListFilter filter, CancellationToken cancellationToken = default);

    Task<ReportedUserDetails?> GetReportedUserDetailsAsync(MockUser currentUser, long userId, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> ReviewUserReportAsync(MockUser currentUser, ReviewUserReportInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationPreview> PreviewRestrictTicketPurchaseAsync(MockUser currentUser, RestrictTicketPurchaseInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> ExecuteRestrictTicketPurchaseAsync(MockUser currentUser, RestrictTicketPurchaseInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> RemoveTicketPurchaseRestrictionAsync(MockUser currentUser, RemoveTicketPurchaseRestrictionInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> SendUserReportWarningAsync(MockUser currentUser, SendUserReportWarningInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> SendUserReportNotificationAsync(MockUser currentUser, SendUserReportNotificationInput input, CancellationToken cancellationToken = default);

    Task<SpecialOperationExecuteResult> DeactivateReportedUserAsync(MockUser currentUser, DeactivateReportedUserInput input, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<SpecialOperationHistoryItem>> ListHistoryAsync(MockUser currentUser, CancellationToken cancellationToken = default);
}
