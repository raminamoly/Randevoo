using MediatR;
using Randevoo.Application.Features.Moderation.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.Moderation.Queries.ListModerationReports;

public record ListModerationReportsQuery(
    long ActorUserId,
    bool IsAdmin,
    ModerationReportStatus? Status,
    int Limit = 50,
    long? AfterId = null,
    DateTime? CreatedAfter = null) : IRequest<IReadOnlyList<ModerationReportDto>>;
