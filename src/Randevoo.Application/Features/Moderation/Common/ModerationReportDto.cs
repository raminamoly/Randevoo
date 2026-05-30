using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.Moderation.Common;

public record ModerationReportDto(
    long Id,
    long ReporterUserId,
    long ReportedUserId,
    long? DatingEventId,
    long? EventConversationId,
    ModerationReportReason Reason,
    string Description,
    ModerationReportStatus Status,
    string? AdminReviewNote,
    long? ReviewedByAdminUserId,
    DateTime? ReviewedAt,
    DateTime CreatedAt)
{
    public static ModerationReportDto FromEntity(ModerationReport report) =>
        new(
            report.Id,
            report.ReporterUserId,
            report.ReportedUserId,
            report.DatingEventId,
            report.EventConversationId,
            report.Reason,
            report.Description,
            report.Status,
            report.AdminReviewNote,
            report.ReviewedByAdminUserId,
            report.ReviewedAt,
            report.CreatedAt);
}
