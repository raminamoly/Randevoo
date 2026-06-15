using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Finance;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IFinanceApiClient
{
    Task<PlannerFinanceDashboard> GetPlannerFinanceAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task RequestWithdrawalAsync(MockUser currentUser, decimal amount, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlannerWithdrawalRequestItem>> GetWithdrawalRequestsAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task ConfirmWithdrawalAsync(MockUser currentUser, long requestId, string? reviewNote, CancellationToken cancellationToken = default);

    Task RejectWithdrawalAsync(MockUser currentUser, long requestId, string? reviewNote, CancellationToken cancellationToken = default);

    Task RequestEventSettlementAsync(MockUser currentUser, long eventId, string? note = null, CancellationToken cancellationToken = default);

    Task ConfirmEventSettlementAsync(MockUser currentUser, long requestId, string? reviewNote = null, CancellationToken cancellationToken = default);

    Task RejectEventSettlementAsync(MockUser currentUser, long requestId, string? reviewNote = null, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdminEventTicketTransactionGroup>> GetTicketPurchaseTransactionsByEventAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ManualPaymentReceiptItem>> GetManualPaymentReceiptsAsync(MockUser currentUser, ManualPaymentDestinationType destinationType, CancellationToken cancellationToken = default);

    Task ApproveManualPaymentReceiptAsync(MockUser currentUser, long receiptId, CancellationToken cancellationToken = default);

    Task RejectManualPaymentReceiptAsync(MockUser currentUser, long receiptId, string reason, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TicketRefundRequestItem>> GetTicketRefundRequestsAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task RequestTicketRefundAsync(MockUser currentUser, long ticketId, string reason, CancellationToken cancellationToken = default);

    Task ApproveTicketRefundRequestAsync(MockUser currentUser, long requestId, TicketRefundReviewInput input, CancellationToken cancellationToken = default);

    Task RejectTicketRefundRequestAsync(MockUser currentUser, long requestId, string reviewNote, CancellationToken cancellationToken = default);

    Task<UserFinanceOverview> GetUserFinanceAsync(MockUser currentUser, long userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlannerBankAccountItem>> GetPlannerBankAccountsAsync(MockUser currentUser, long plannerUserId, CancellationToken cancellationToken = default);

    Task SavePlannerBankAccountAsync(MockUser currentUser, long plannerUserId, PlannerBankAccountInput input, CancellationToken cancellationToken = default);

    Task TogglePlannerBankAccountAsync(MockUser currentUser, long plannerUserId, long bankAccountId, bool isActive, CancellationToken cancellationToken = default);
}
