using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Finance;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IFinanceApiClient
{
    Task<PlannerFinanceDashboard> GetPlannerFinanceAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task RequestWithdrawalAsync(MockUser currentUser, decimal amount, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlannerWithdrawalRequestItem>> GetWithdrawalRequestsAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task ConfirmWithdrawalAsync(MockUser currentUser, long requestId, string? reviewNote, CancellationToken cancellationToken = default);

    Task RejectWithdrawalAsync(MockUser currentUser, long requestId, string? reviewNote, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdminEventTicketTransactionGroup>> GetTicketPurchaseTransactionsByEventAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<UserFinanceOverview> GetUserFinanceAsync(MockUser currentUser, long userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<PlannerBankAccountItem>> GetPlannerBankAccountsAsync(MockUser currentUser, long plannerUserId, CancellationToken cancellationToken = default);

    Task SavePlannerBankAccountAsync(MockUser currentUser, long plannerUserId, PlannerBankAccountInput input, CancellationToken cancellationToken = default);

    Task TogglePlannerBankAccountAsync(MockUser currentUser, long plannerUserId, long bankAccountId, bool isActive, CancellationToken cancellationToken = default);
}
