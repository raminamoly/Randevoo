using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Notifications;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabaseNotificationsApiClient : INotificationsApiClient
{
    private readonly RandevooDbContext _db;
    private static readonly IReadOnlyDictionary<NotificationType, string> FallbackTypeLabels = new Dictionary<NotificationType, string>
    {
        [NotificationType.System] = "پیام سیستمی",
        [NotificationType.AdminToPlanner] = "پیام مدیر به برگزارکننده",
        [NotificationType.PlannerToParticipant] = "پیام برگزارکننده به شرکت‌کننده",
        [NotificationType.AdminToUser] = "پیام مدیر به کاربر",
        [NotificationType.EventUpdate] = "اطلاع‌رسانی رویداد",
        [NotificationType.Finance] = "پیام مالی",
        [NotificationType.Refund] = "بازگشت وجه"
    };
    private static readonly IReadOnlyDictionary<NotificationPriority, string> FallbackPriorityLabels = new Dictionary<NotificationPriority, string>
    {
        [NotificationPriority.Normal] = "عادی",
        [NotificationPriority.Important] = "مهم",
        [NotificationPriority.Critical] = "فوری"
    };

    public DatabaseNotificationsApiClient(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<NotificationItem>> GetMyNotificationsAsync(MockUser currentUser, bool unreadOnly = false, CancellationToken cancellationToken = default)
    {
        var result = await GetMyNotificationsAsync(
            currentUser,
            new NotificationListFilter
            {
                ReadState = unreadOnly ? "Unread" : "All",
                Page = 1,
                PageSize = 50
            },
            cancellationToken);

        return result.Items;
    }

    public async Task<NotificationListResult> GetMyNotificationsAsync(MockUser currentUser, NotificationListFilter filter, CancellationToken cancellationToken = default)
    {
        var query = _db.NotificationRecipients
            .AsNoTracking()
            .Include(item => item.Notification)
            .ThenInclude(notification => notification.DatingEvent)
            .Include(item => item.Notification)
            .ThenInclude(notification => notification.CreatedByUser)
            .ThenInclude(user => user.Profile)
            .Where(item => item.RecipientUserId == currentUser.Id
                && item.Channel == NotificationDeliveryChannel.InApp
                && (item.Status == NotificationRecipientStatus.Delivered || item.Status == NotificationRecipientStatus.Read)
                && item.Notification.ApprovalStatus != NotificationApprovalStatus.Rejected);

        if (string.Equals(filter.ReadState, "Unread", StringComparison.OrdinalIgnoreCase))
            query = query.Where(item => item.ReadAtUtc == null);
        else if (string.Equals(filter.ReadState, "Read", StringComparison.OrdinalIgnoreCase))
            query = query.Where(item => item.ReadAtUtc != null);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            query = query.Where(item => item.Notification.Title.Contains(search) || item.Notification.Body.Contains(search));
        }

        if (filter.Type is NotificationType type)
            query = query.Where(item => item.Notification.Type == type);

        if (filter.Priority is NotificationPriority priority)
            query = query.Where(item => item.Notification.Priority == priority);

        if (filter.EventId is long eventId)
            query = query.Where(item => item.Notification.DatingEventId == eventId);

        if (filter.FromUtc is DateTime fromUtc)
            query = query.Where(item => item.Notification.CreatedAt >= fromUtc);

        if (filter.ToUtc is DateTime toUtc)
            query = query.Where(item => item.Notification.CreatedAt <= toUtc);

        query = filter.SortBy switch
        {
            "created_asc" => query.OrderBy(item => item.Notification.CreatedAt),
            "priority_desc" => query.OrderByDescending(item => item.Notification.Priority).ThenByDescending(item => item.Notification.CreatedAt),
            "unread_first" => query.OrderBy(item => item.ReadAtUtc != null).ThenByDescending(item => item.Notification.CreatedAt),
            _ => query.OrderByDescending(item => item.Notification.CreatedAt)
        };

        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 10, 100);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(item => new NotificationItem
            {
                Id = item.NotificationId,
                Type = item.Notification.Type,
                Priority = item.Notification.Priority,
                ApprovalStatus = item.Notification.ApprovalStatus,
                Title = item.Notification.Title,
                Body = item.Notification.Body,
                EventId = item.Notification.DatingEventId,
                EventTitle = item.Notification.DatingEvent == null ? null : item.Notification.DatingEvent.Title,
                CreatedByName = item.Notification.CreatedByUser.Profile == null ? item.Notification.CreatedByUser.MobileNumber : item.Notification.CreatedByUser.Profile.DisplayName,
                CreatedAtUtc = item.Notification.CreatedAt,
                ReadAtUtc = item.ReadAtUtc,
                RecipientCount = item.Notification.Recipients.Count
            })
            .ToListAsync(cancellationToken);

        await ApplyDisplayLabelsAsync(items, cancellationToken);
        return new NotificationListResult
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public Task<int> GetUnreadCountAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        return _db.NotificationRecipients
            .AsNoTracking()
            .CountAsync(item => item.RecipientUserId == currentUser.Id
                && item.Channel == NotificationDeliveryChannel.InApp
                && item.ReadAtUtc == null
                && item.Status == NotificationRecipientStatus.Delivered
                && item.Notification.ApprovalStatus != NotificationApprovalStatus.Rejected,
                cancellationToken);
    }

    public async Task MarkAsReadAsync(MockUser currentUser, long notificationId, CancellationToken cancellationToken = default)
    {
        var recipient = await _db.NotificationRecipients
            .Include(item => item.Notification)
            .FirstOrDefaultAsync(item => item.NotificationId == notificationId
                && item.RecipientUserId == currentUser.Id
                && item.Channel == NotificationDeliveryChannel.InApp,
                cancellationToken)
            ?? throw new InvalidOperationException("پیام پیدا نشد.");

        recipient.MarkRead();
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        var recipients = await _db.NotificationRecipients
            .Include(item => item.Notification)
            .Where(item => item.RecipientUserId == currentUser.Id
                && item.Channel == NotificationDeliveryChannel.InApp
                && item.ReadAtUtc == null
                && item.Status == NotificationRecipientStatus.Delivered)
            .ToListAsync(cancellationToken);

        foreach (var recipient in recipients)
            recipient.MarkRead();

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationItem>> GetPendingApprovalsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        EnsureAdminOrSupport(currentUser);

        var items = await _db.Notifications
            .AsNoTracking()
            .Include(item => item.DatingEvent)
            .Include(item => item.CreatedByUser)
            .ThenInclude(user => user.Profile)
            .Where(item => item.ApprovalStatus == NotificationApprovalStatus.Pending)
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new NotificationItem
            {
                Id = item.Id,
                Type = item.Type,
                Priority = item.Priority,
                ApprovalStatus = item.ApprovalStatus,
                Title = item.Title,
                Body = item.Body,
                EventId = item.DatingEventId,
                EventTitle = item.DatingEvent == null ? null : item.DatingEvent.Title,
                CreatedByName = item.CreatedByUser.Profile == null ? item.CreatedByUser.MobileNumber : item.CreatedByUser.Profile.DisplayName,
                CreatedAtUtc = item.CreatedAt,
                RecipientCount = item.Recipients.Count
            })
            .ToListAsync(cancellationToken);
        await ApplyDisplayLabelsAsync(items, cancellationToken);
        return items;
    }

    public async Task<IReadOnlyList<NotificationMessageTypeOption>> GetMessageTypeOptionsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        var roleKey = RoleKey(currentUser.Role);
        var records = await _db.NotificationMessageTypes
            .AsNoTracking()
            .Where(item => item.IsActive && item.AllowedSenderRoles.Contains(roleKey))
            .OrderBy(item => item.DisplayOrder)
            .ToListAsync(cancellationToken);

        return records
            .Select(item => new NotificationMessageTypeOption
            {
                Type = item.Type,
                Label = item.DisplayNameFa,
                Description = item.DescriptionFa,
                RequiresApproval = item.RequiresApproval,
                SupportsSms = item.SupportsSms,
                DefaultPriority = item.DefaultPriority,
                AllowedTargets = item.AllowedTargets.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            })
            .ToList();
    }

    public async Task<IReadOnlyList<NotificationPriorityOption>> GetPriorityOptionsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.NotificationPriorities
            .AsNoTracking()
            .Where(item => item.IsActive)
            .OrderBy(item => item.DisplayOrder)
            .Select(item => new NotificationPriorityOption
            {
                Priority = item.Priority,
                Label = item.DisplayNameFa,
                Description = item.DescriptionFa
            })
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<NotificationTargetOption>> GetTargetOptionsAsync(MockUser currentUser, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<NotificationTargetOption> options = currentUser.Role == AdminRole.EventPlanner
            ?
            [
                new() { Value = "EventParticipants", Label = "شرکت‌کنندگان رویداد من", Description = "افرادی که برای رویداد انتخاب‌شده بلیت معتبر دارند." },
                new() { Value = "EventBuyers", Label = "خریداران رویداد من", Description = "افرادی که خرید پرداخت‌شده برای رویداد انتخاب‌شده دارند." },
                new() { Value = "User", Label = "یک کاربر مرتبط با رویداد من", Description = "فقط خریدار یا شرکت‌کننده همان رویداد." }
            ]
            :
            [
                new() { Value = "User", Label = "یک کاربر", Description = "ارسال مستقیم به یک کاربر فعال." },
                new() { Value = "EventParticipants", Label = "شرکت‌کنندگان رویداد", Description = "شرکت‌کنندگان دارای بلیت معتبر در رویداد انتخاب‌شده." },
                new() { Value = "EventBuyers", Label = "خریداران رویداد", Description = "خریداران دارای سفارش پرداخت‌شده در رویداد انتخاب‌شده." },
                new() { Value = "Planners", Label = "همه برگزارکنندگان", Description = "ارسال فقط توسط مدیر یا پشتیبان به برگزارکنندگان فعال." }
            ];

        return Task.FromResult(options);
    }

    public async Task<IReadOnlyList<NotificationEventOption>> SearchEventOptionsAsync(MockUser currentUser, string? search, CancellationToken cancellationToken = default)
    {
        var query = _db.DatingEvents
            .AsNoTracking()
            .Where(item => !item.IsCancelled)
            .Where(item => item.DateTimeEnd >= DateTime.UtcNow.AddDays(-90));

        if (currentUser.Role == AdminRole.EventPlanner)
            query = query.Where(item => item.EventPlannerUserId == currentUser.Id);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(item => item.Title.Contains(term) || item.EventCode.ToString().Contains(term));
        }

        return await query
            .OrderByDescending(item => item.DateTimeStart)
            .Take(80)
            .Select(item => new NotificationEventOption
            {
                Id = item.Id,
                EventCode = item.EventCode,
                Title = item.Title,
                StartAtUtc = item.DateTimeStart,
                StatusLabel = item.LifecycleStatus == EventLifecycleStatus.Cancelled
                    ? "لغو شده"
                    : item.LifecycleStatus == EventLifecycleStatus.Completed
                        ? "تمام شده"
                        : item.SaleStatus == EventSaleStatus.Open ? "فروش باز" : "فروش بسته"
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationUserOption>> SearchUserOptionsAsync(MockUser currentUser, long? eventId, string? search, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query;
        if (eventId is long selectedEventId)
        {
            var canUseEvent = await _db.DatingEvents.AnyAsync(item => item.Id == selectedEventId
                && (currentUser.Role != AdminRole.EventPlanner || item.EventPlannerUserId == currentUser.Id),
                cancellationToken);
            if (!canUseEvent)
                return Array.Empty<NotificationUserOption>();

            var participantIds = _db.EventTickets
                .Where(ticket => ticket.DatingEventId == selectedEventId && !ticket.IsRefunded && !ticket.IsRemoved)
                .Select(ticket => ticket.UserId);
            query = _db.Users.Where(user => participantIds.Contains(user.Id));
        }
        else if (currentUser.Role == AdminRole.EventPlanner)
        {
            return Array.Empty<NotificationUserOption>();
        }
        else
        {
            query = _db.Users;
        }

        query = query.AsNoTracking().Include(item => item.Profile).Where(item => item.IsActive);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(item => item.MobileNumber.Contains(term) || (item.Profile != null && item.Profile.DisplayName.Contains(term)));
        }

        return await query
            .OrderBy(item => item.Profile == null ? item.MobileNumber : item.Profile.DisplayName)
            .Take(80)
            .Select(item => new NotificationUserOption
            {
                Id = item.Id,
                DisplayName = item.Profile == null ? item.MobileNumber : item.Profile.DisplayName,
                Mobile = item.MobileNumber
            })
            .ToListAsync(cancellationToken);
    }

    public async Task CreateNotificationAsync(MockUser currentUser, NotificationCreateInput input, CancellationToken cancellationToken = default)
    {
        var messageType = await _db.NotificationMessageTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Type == input.Type && item.IsActive, cancellationToken)
            ?? throw new InvalidOperationException("نوع پیام معتبر نیست.");
        var roleKey = RoleKey(currentUser.Role);
        if (!ContainsCsv(messageType.AllowedSenderRoles, roleKey))
            throw new InvalidOperationException("شما به ارسال این نوع پیام دسترسی ندارید.");
        if (!ContainsCsv(messageType.AllowedTargets, input.Target))
            throw new InvalidOperationException("گیرنده انتخاب‌شده برای این نوع پیام مجاز نیست.");
        if (input.SendSms && !messageType.SupportsSms)
            throw new InvalidOperationException("این نوع پیام قابلیت ثبت پیامک ندارد.");

        var sender = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("کاربر جاری پیدا نشد.");

        var datingEvent = input.EventId is long eventId
            ? await _db.DatingEvents.FirstOrDefaultAsync(item => item.Id == eventId, cancellationToken)
                ?? throw new InvalidOperationException("رویداد پیدا نشد.")
            : null;

        if (currentUser.Role == AdminRole.EventPlanner && datingEvent?.EventPlannerUserId != currentUser.Id)
            throw new InvalidOperationException("برگزارکننده فقط برای رویدادهای خودش می‌تواند پیام ارسال کند.");

        var recipients = await ResolveRecipientsAsync(currentUser, input, cancellationToken);
        if (recipients.Count == 0)
            throw new InvalidOperationException("هیچ گیرنده‌ای برای پیام پیدا نشد.");

        var requiresApproval = messageType.RequiresApproval || currentUser.Role == AdminRole.EventPlanner && input.SendSms;
        var notification = new Notification(
            sender,
            input.Type,
            input.Title,
            input.Body,
            input.Priority,
            requiresApproval,
            datingEvent,
            "Notification",
            null);

        foreach (var recipient in recipients)
        {
            notification.AddRecipient(recipient, NotificationDeliveryChannel.InApp);
            if (input.SendSms && datingEvent is not null)
                notification.AddRecipient(recipient, NotificationDeliveryChannel.Sms);
        }

        _db.Notifications.Add(notification);
        if (!requiresApproval)
            AddSmsQueueItems(notification, datingEvent);

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveNotificationAsync(MockUser currentUser, long notificationId, string? reviewNote, CancellationToken cancellationToken = default)
    {
        EnsureAdminOrSupport(currentUser);

        var reviewer = await _db.Users.FirstOrDefaultAsync(item => item.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("کاربر بررسی‌کننده پیدا نشد.");
        var notification = await _db.Notifications
            .Include(item => item.DatingEvent)
            .Include(item => item.Recipients)
            .ThenInclude(recipient => recipient.RecipientUser)
            .FirstOrDefaultAsync(item => item.Id == notificationId, cancellationToken)
            ?? throw new InvalidOperationException("پیام پیدا نشد.");

        notification.Approve(reviewer, reviewNote);
        AddSmsQueueItems(notification, notification.DatingEvent);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectNotificationAsync(MockUser currentUser, long notificationId, string reviewNote, CancellationToken cancellationToken = default)
    {
        EnsureAdminOrSupport(currentUser);

        var reviewer = await _db.Users.FirstOrDefaultAsync(item => item.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("کاربر بررسی‌کننده پیدا نشد.");
        var notification = await _db.Notifications
            .Include(item => item.Recipients)
            .FirstOrDefaultAsync(item => item.Id == notificationId, cancellationToken)
            ?? throw new InvalidOperationException("پیام پیدا نشد.");

        notification.Reject(reviewer, reviewNote);
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<User>> ResolveRecipientsAsync(MockUser currentUser, NotificationCreateInput input, CancellationToken cancellationToken)
    {
        var target = (input.Target ?? "User").Trim();
        if (target.Equals("User", StringComparison.OrdinalIgnoreCase))
        {
            if (input.TargetUserId is not long userId)
                throw new InvalidOperationException("کاربر گیرنده را انتخاب کنید.");

            if (currentUser.Role == AdminRole.EventPlanner)
            {
                if (input.EventId is not long eventId)
                    throw new InvalidOperationException("برای ارسال مستقیم توسط برگزارکننده، رویداد را انتخاب کنید.");

                var isRelatedUser = await _db.DatingEvents.AnyAsync(item => item.Id == eventId && item.EventPlannerUserId == currentUser.Id, cancellationToken)
                    && (await _db.EventTickets.AnyAsync(ticket => ticket.DatingEventId == eventId && ticket.UserId == userId && !ticket.IsRefunded && !ticket.IsRemoved, cancellationToken)
                        || await _db.TicketOrders.AnyAsync(order => order.DatingEventId == eventId && order.BuyerUserId == userId && order.PaymentStatus == TicketOrderPaymentStatus.Paid, cancellationToken));
                if (!isRelatedUser)
                    throw new InvalidOperationException("برگزارکننده فقط می‌تواند به کاربران مرتبط با رویداد خودش پیام بدهد.");
            }

            return await _db.Users.Where(user => user.Id == userId && user.IsActive).ToListAsync(cancellationToken);
        }

        if (target.Equals("EventParticipants", StringComparison.OrdinalIgnoreCase))
        {
            if (input.EventId is not long eventId)
                throw new InvalidOperationException("برای ارسال به شرکت‌کنندگان، رویداد را انتخاب کنید.");

            return await _db.EventTickets
                .Where(ticket => ticket.DatingEventId == eventId && !ticket.IsRefunded && !ticket.IsRemoved)
                .Where(ticket => currentUser.Role != AdminRole.EventPlanner || ticket.DatingEvent.EventPlannerUserId == currentUser.Id)
                .Select(ticket => ticket.User)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        if (target.Equals("EventBuyers", StringComparison.OrdinalIgnoreCase))
        {
            if (input.EventId is not long eventId)
                throw new InvalidOperationException("برای ارسال به خریداران، رویداد را انتخاب کنید.");

            return await _db.TicketOrders
                .Where(order => order.DatingEventId == eventId && order.PaymentStatus == TicketOrderPaymentStatus.Paid)
                .Where(order => currentUser.Role != AdminRole.EventPlanner || order.DatingEvent.EventPlannerUserId == currentUser.Id)
                .Select(order => order.BuyerUser)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        if (target.Equals("Planners", StringComparison.OrdinalIgnoreCase))
        {
            EnsureAdminOrSupport(currentUser);
            return await _db.Users.Where(user => user.Role == UserRole.EventPlanner && user.IsActive).ToListAsync(cancellationToken);
        }

        throw new InvalidOperationException("نوع گیرنده پیام معتبر نیست.");
    }

    private void AddSmsQueueItems(Notification notification, DatingEvent? datingEvent)
    {
        if (datingEvent is null)
            return;

        foreach (var recipient in notification.Recipients.Where(item => item.Channel == NotificationDeliveryChannel.Sms && item.Status != NotificationRecipientStatus.Rejected))
            _db.SmsQueueItems.Add(new SmsQueueItem(recipient.RecipientUser, datingEvent, notification.Body));
    }

    private static void EnsureAdminOrSupport(MockUser currentUser)
    {
        if (currentUser.Role is AdminRole.Admin or AdminRole.SupportTeam)
            return;

        throw new InvalidOperationException("این عملیات فقط برای مدیر یا پشتیبان فعال است.");
    }

    private async Task ApplyDisplayLabelsAsync(IReadOnlyList<NotificationItem> items, CancellationToken cancellationToken)
    {
        if (items.Count == 0)
            return;

        var typeLabels = await _db.NotificationMessageTypes
            .AsNoTracking()
            .Where(item => item.IsActive)
            .ToDictionaryAsync(item => item.Type, item => item.DisplayNameFa, cancellationToken);
        var priorityLabels = await _db.NotificationPriorities
            .AsNoTracking()
            .Where(item => item.IsActive)
            .ToDictionaryAsync(item => item.Priority, item => item.DisplayNameFa, cancellationToken);

        foreach (var item in items)
        {
            item.TypeLabel = typeLabels.TryGetValue(item.Type, out var typeLabel) ? typeLabel : FallbackTypeLabels.GetValueOrDefault(item.Type, item.Type.ToString());
            item.PriorityLabel = priorityLabels.TryGetValue(item.Priority, out var priorityLabel) ? priorityLabel : FallbackPriorityLabels.GetValueOrDefault(item.Priority, item.Priority.ToString());
        }
    }

    private static bool ContainsCsv(string csv, string value)
    {
        return csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(item => item.Equals(value, StringComparison.OrdinalIgnoreCase));
    }

    private static string RoleKey(AdminRole role) => role switch
    {
        AdminRole.Admin => nameof(UserRole.Admin),
        AdminRole.EventPlanner => nameof(UserRole.EventPlanner),
        AdminRole.SupportTeam => nameof(UserRole.PlatformSupportTeam),
        _ => role.ToString()
    };
}
