using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class ModerationReport : BaseEntity, IAggregateRoot
{
    public long ReporterUserId { get; private set; }
    public User ReporterUser { get; private set; } = null!;
    public long ReportedUserId { get; private set; }
    public User ReportedUser { get; private set; } = null!;
    public long? DatingEventId { get; private set; }
    public DatingEvent? DatingEvent { get; private set; }
    public long? EventConversationId { get; private set; }
    public EventConversation? EventConversation { get; private set; }
    public ModerationReportReason Reason { get; private set; }
    public string Description { get; private set; } = null!;
    public ModerationReportStatus Status { get; private set; }
    public string? AdminReviewNote { get; private set; }
    public long? ReviewedByAdminUserId { get; private set; }
    public User? ReviewedByAdminUser { get; private set; }
    public DateTime? ReviewedAt { get; private set; }

    private ModerationReport() { }

    public ModerationReport(
        User reporterUser,
        User reportedUser,
        ModerationReportReason reason,
        string description,
        long? datingEventId = null,
        long? eventConversationId = null)
    {
        ReporterUser = GuardAgainst.Object.Null(reporterUser, nameof(reporterUser));
        ReportedUser = GuardAgainst.Object.Null(reportedUser, nameof(reportedUser));
        if (reporterUser.Id == reportedUser.Id)
            throw new BusinessRuleViolationException("Invalid report", "User cannot report themselves");

        ReporterUserId = reporterUser.Id;
        ReportedUserId = reportedUser.Id;
        Reason = reason;
        Description = GuardAgainst.String.InvalidLength(description, nameof(description), 5, 2000);
        DatingEventId = datingEventId;
        EventConversationId = eventConversationId;
        Status = ModerationReportStatus.Pending;
        AddDomainEvent(new EntityCreatedEvent<ModerationReport>(this));
    }

    public void Review(ModerationReportStatus status, long adminUserId, string? note)
    {
        if (status == ModerationReportStatus.Pending)
            throw new BusinessRuleViolationException("Invalid report status", "Admin review must move the report out of pending status");

        Status = status;
        ReviewedByAdminUserId = adminUserId;
        ReviewedAt = DateTime.UtcNow;
        AdminReviewNote = string.IsNullOrWhiteSpace(note) ? null : GuardAgainst.String.MaxLength(note, nameof(note), 2000);
        UpdateTimestamp();
    }
}
