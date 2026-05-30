using MediatR;
using Randevoo.Application.Features.Moderation.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.Moderation.Commands.ReviewModerationReport;

public record ReviewModerationReportCommand(long AdminUserId, long ReportId, ModerationReportStatus Status, string? Note) : IRequest<ModerationReportDto>;
