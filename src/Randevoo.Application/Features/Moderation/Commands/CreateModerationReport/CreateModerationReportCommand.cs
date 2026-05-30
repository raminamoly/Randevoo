using MediatR;
using Randevoo.Application.Features.Moderation.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.Moderation.Commands.CreateModerationReport;

public record CreateModerationReportCommand(
    long ReporterUserId,
    long ReportedUserId,
    long? DatingEventId,
    long? EventConversationId,
    ModerationReportReason Reason,
    string Description) : IRequest<ModerationReportDto>;
