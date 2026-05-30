using MediatR;
using Randevoo.Application.Features.Moderation.Common;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Moderation.Queries.ListModerationReports;

public class ListModerationReportsHandler : IRequestHandler<ListModerationReportsQuery, IReadOnlyList<ModerationReportDto>>
{
    private readonly IModerationReportRepository _reports;

    public ListModerationReportsHandler(IModerationReportRepository reports)
    {
        _reports = reports;
    }

    public async Task<IReadOnlyList<ModerationReportDto>> Handle(ListModerationReportsQuery request, CancellationToken cancellationToken)
    {
        var reports = request.IsAdmin
            ? await _reports.ListByStatusAsync(request.Status, cancellationToken)
            : await _reports.ListByReporterAsync(request.ActorUserId, cancellationToken);

        return reports.Select(ModerationReportDto.FromEntity).ToList();
    }
}
