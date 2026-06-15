using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Notifications;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface INotificationsApiClient
{
    Task<IReadOnlyList<NotificationItem>> GetMyNotificationsAsync(MockUser currentUser, bool unreadOnly = false, CancellationToken cancellationToken = default);

    Task<NotificationListResult> GetMyNotificationsAsync(MockUser currentUser, NotificationListFilter filter, CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationMessageTypeOption>> GetMessageTypeOptionsAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationPriorityOption>> GetPriorityOptionsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationTargetOption>> GetTargetOptionsAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationEventOption>> SearchEventOptionsAsync(MockUser currentUser, string? search, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationUserOption>> SearchUserOptionsAsync(MockUser currentUser, long? eventId, string? search, CancellationToken cancellationToken = default);

    Task MarkAsReadAsync(MockUser currentUser, long notificationId, CancellationToken cancellationToken = default);

    Task MarkAllAsReadAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<NotificationItem>> GetPendingApprovalsAsync(MockUser currentUser, CancellationToken cancellationToken = default);

    Task CreateNotificationAsync(MockUser currentUser, NotificationCreateInput input, CancellationToken cancellationToken = default);

    Task ApproveNotificationAsync(MockUser currentUser, long notificationId, string? reviewNote, CancellationToken cancellationToken = default);

    Task RejectNotificationAsync(MockUser currentUser, long notificationId, string reviewNote, CancellationToken cancellationToken = default);
}
