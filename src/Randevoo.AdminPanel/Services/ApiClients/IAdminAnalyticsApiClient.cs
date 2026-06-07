using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Models.Dashboard;
using Randevoo.AdminPanel.Models.Logs;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IAdminAnalyticsApiClient
{
    Task<UserDashboardReport> GetUserDashboardAsync(MockUser currentUser, DashboardDateRangeValue range, CancellationToken cancellationToken = default);

    Task<SalesDashboardReport> GetSalesDashboardAsync(MockUser currentUser, DashboardDateRangeValue range, CancellationToken cancellationToken = default);

    Task<EventDashboardReport> GetEventDashboardAsync(MockUser currentUser, DashboardDateRangeValue range, CancellationToken cancellationToken = default);

    Task<MoneyDashboardReport> GetMoneyDashboardAsync(MockUser currentUser, DashboardDateRangeValue range, CancellationToken cancellationToken = default);

    Task<AuditLogListResult> GetAuditLogsAsync(MockUser currentUser, AuditLogFilter filter, CancellationToken cancellationToken = default);

    Task<SmsQueueListResult> GetSmsQueueAsync(MockUser currentUser, SmsQueueListFilter filter, CancellationToken cancellationToken = default);
}
