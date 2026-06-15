using System.ComponentModel.DataAnnotations;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Notifications;

public sealed class NotificationItem
{
    public long Id { get; set; }
    public NotificationType Type { get; set; }
    public string TypeLabel { get; set; } = "";
    public NotificationPriority Priority { get; set; }
    public string PriorityLabel { get; set; } = "";
    public NotificationApprovalStatus ApprovalStatus { get; set; }
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public long? EventId { get; set; }
    public string? EventTitle { get; set; }
    public string CreatedByName { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public int RecipientCount { get; set; }
}

public sealed class NotificationListFilter
{
    public string? Search { get; set; }
    public string? ReadState { get; set; }
    public NotificationType? Type { get; set; }
    public NotificationPriority? Priority { get; set; }
    public long? EventId { get; set; }
    public DateTime? FromUtc { get; set; }
    public DateTime? ToUtc { get; set; }
    public string SortBy { get; set; } = "created_desc";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public sealed class NotificationListResult
{
    public IReadOnlyList<NotificationItem> Items { get; set; } = Array.Empty<NotificationItem>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize <= 0 ? 1 : Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
}

public sealed class NotificationCreateInput
{
    [Required(ErrorMessage = "عنوان پیام الزامی است.")]
    [StringLength(180, MinimumLength = 2, ErrorMessage = "عنوان باید بین ۲ تا ۱۸۰ کاراکتر باشد.")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "متن پیام الزامی است.")]
    [StringLength(2000, MinimumLength = 2, ErrorMessage = "متن پیام باید بین ۲ تا ۲۰۰۰ کاراکتر باشد.")]
    public string Body { get; set; } = "";

    public NotificationType Type { get; set; } = NotificationType.AdminToUser;

    public NotificationPriority Priority { get; set; } = NotificationPriority.Normal;

    public string Target { get; set; } = "User";

    public long? TargetUserId { get; set; }

    public long? EventId { get; set; }

    public bool SendSms { get; set; }
}

public sealed class NotificationMessageTypeOption
{
    public NotificationType Type { get; set; }
    public string Label { get; set; } = "";
    public string Description { get; set; } = "";
    public bool RequiresApproval { get; set; }
    public bool SupportsSms { get; set; }
    public NotificationPriority DefaultPriority { get; set; }
    public IReadOnlyList<string> AllowedTargets { get; set; } = Array.Empty<string>();
}

public sealed class NotificationPriorityOption
{
    public NotificationPriority Priority { get; set; }
    public string Label { get; set; } = "";
    public string Description { get; set; } = "";
}

public sealed class NotificationTargetOption
{
    public string Value { get; set; } = "";
    public string Label { get; set; } = "";
    public string Description { get; set; } = "";
}

public sealed class NotificationEventOption
{
    public long Id { get; set; }
    public int EventCode { get; set; }
    public string Title { get; set; } = "";
    public string StatusLabel { get; set; } = "";
    public DateTime StartAtUtc { get; set; }
    public string DisplayText => $"#{EventCode} - {Title} ({StatusLabel})";
}

public sealed class NotificationUserOption
{
    public long Id { get; set; }
    public string DisplayName { get; set; } = "";
    public string Mobile { get; set; } = "";
    public string DisplayText => string.IsNullOrWhiteSpace(Mobile) ? DisplayName : $"{DisplayName} - {Mobile}";
}

public sealed class NotificationReviewInput
{
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "یادداشت بررسی باید بین ۳ تا ۱۰۰۰ کاراکتر باشد.")]
    public string? ReviewNote { get; set; }
}
