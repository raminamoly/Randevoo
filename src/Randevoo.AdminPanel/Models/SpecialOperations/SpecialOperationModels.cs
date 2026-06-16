using System.ComponentModel.DataAnnotations;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.SpecialOperations;

public sealed class CancelTicketRefundInput
{
    [Range(1, long.MaxValue)]
    public long TicketId { get; set; }

    [Range(1, long.MaxValue)]
    public long BuyerUserId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(80)]
    public string? SupportTicketNumber { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class ManualIssueTicketInput
{
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }

    [Range(1, long.MaxValue)]
    public long EventId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(80)]
    public string? SupportTicketNumber { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class ManualWalletAdjustmentInput
{
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }

    [Range(1, 1_000_000_000_000)]
    public decimal Amount { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(80)]
    public string? SupportTicketNumber { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class UserReportListFilter
{
    [StringLength(100)]
    public string? SearchTerm { get; set; }

    public ModerationReportStatus? Status { get; set; }

    [Range(0, 1000)]
    public int? MinimumOpenReports { get; set; }

    [Range(1, 1000)]
    public int Page { get; set; } = 1;

    [Range(5, 100)]
    public int PageSize { get; set; } = 20;
}

public sealed class ReviewUserReportInput
{
    [Range(1, long.MaxValue)]
    public long ReportId { get; set; }

    [Required]
    public ModerationReportStatus Status { get; set; } = ModerationReportStatus.Reviewed;

    [StringLength(2000)]
    public string? Note { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class RestrictTicketPurchaseInput
{
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(80)]
    public string? SupportTicketNumber { get; set; }

    public DateTime? ExpiresAtUtc { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class RemoveTicketPurchaseRestrictionInput
{
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string Reason { get; set; } = string.Empty;

    [StringLength(80)]
    public string? SupportTicketNumber { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class SendUserReportWarningInput
{
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 5)]
    public string Message { get; set; } = "درباره رفتار شما در یک رویداد گزارش‌هایی دریافت شده است. لطفاً قوانین استفاده از راندوو را رعایت کنید.";

    [StringLength(80)]
    public string? SupportTicketNumber { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class SendUserReportNotificationInput
{
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }

    [Required]
    [StringLength(180, MinimumLength = 2)]
    public string Title { get; set; } = "پیام پشتیبانی راندوو";

    [Required]
    [StringLength(2000, MinimumLength = 5)]
    public string Message { get; set; } = string.Empty;

    [StringLength(80)]
    public string? SupportTicketNumber { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed class DeactivateReportedUserInput
{
    [Range(1, long.MaxValue)]
    public long UserId { get; set; }

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string Reason { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 5)]
    public string NotificationMessage { get; set; } = "حساب شما به دلیل گزارش‌های دریافت‌شده و بررسی پشتیبانی راندوو غیرفعال شد. برای پیگیری با پشتیبانی تماس بگیرید.";

    [StringLength(80)]
    public string? SupportTicketNumber { get; set; }

    public string IdempotencyKey { get; set; } = Guid.NewGuid().ToString("N");
}

public sealed record SpecialOperationPreview(
    string OperationType,
    string Title,
    IReadOnlyList<SpecialOperationPreviewRow> Rows,
    IReadOnlyList<string> Warnings);

public sealed record SpecialOperationPreviewRow(string Label, string Value, bool IsDanger = false);

public sealed record SpecialOperationExecuteResult(
    long OperationId,
    string OperationType,
    string Message,
    bool AlreadyApplied);

public sealed class ReportedUserListResult
{
    public IReadOnlyList<ReportedUserSummaryItem> Items { get; init; } = Array.Empty<ReportedUserSummaryItem>();
    public int TotalCount { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public sealed class ReportedUserSummaryItem
{
    public long UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
    public string? ProfileImageUrl { get; init; }
    public int TotalReports { get; init; }
    public int OpenReports { get; init; }
    public DateTime LastReportedAtUtc { get; init; }
    public ModerationReportReason LatestReason { get; init; }
    public string LatestDescription { get; init; } = string.Empty;
    public bool HasActiveTicketPurchaseRestriction { get; init; }
    public string? ActiveRestrictionReason { get; init; }
    public DateTime? ActiveRestrictionExpiresAtUtc { get; init; }
}

public sealed class ReportedUserDetails
{
    public long UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string MobileNumber { get; init; } = string.Empty;
    public string? ProfileImageUrl { get; init; }
    public bool IsUserActive { get; init; }
    public bool HasActiveTicketPurchaseRestriction { get; init; }
    public string? ActiveRestrictionReason { get; init; }
    public DateTime? ActiveRestrictionExpiresAtUtc { get; init; }
    public IReadOnlyList<UserReportDetailItem> Reports { get; init; } = Array.Empty<UserReportDetailItem>();
}

public sealed class UserReportDetailItem
{
    public long Id { get; init; }
    public long ReporterUserId { get; init; }
    public string ReporterName { get; init; } = string.Empty;
    public long? EventId { get; init; }
    public string? EventTitle { get; init; }
    public ModerationReportReason Reason { get; init; }
    public string Description { get; init; } = string.Empty;
    public ModerationReportStatus Status { get; init; }
    public string? AdminReviewNote { get; init; }
    public string? ReviewedByName { get; init; }
    public DateTime? ReviewedAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed class SpecialOperationHistoryItem
{
    public long Id { get; init; }
    public string OperationType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string PerformedByName { get; init; } = string.Empty;
    public string? TargetUserName { get; init; }
    public long? TargetUserId { get; init; }
    public long? RelatedTicketId { get; init; }
    public long? RelatedOrderId { get; init; }
    public long? RelatedEventId { get; init; }
    public long? RelatedWalletTransactionId { get; init; }
    public decimal? Amount { get; init; }
    public string? CurrencyCode { get; init; }
    public string Reason { get; init; } = string.Empty;
    public string? SupportTicketNumber { get; init; }
    public string? FailureMessage { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
}
