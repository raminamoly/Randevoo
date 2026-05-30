using MediatR;
using Randevoo.Application.Features.Moderation.Common;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Moderation.Commands.ReviewModerationReport;

public class ReviewModerationReportHandler : IRequestHandler<ReviewModerationReportCommand, ModerationReportDto>
{
    private readonly IModerationReportRepository _reports;
    private readonly IUnitOfWork _unitOfWork;

    public ReviewModerationReportHandler(IModerationReportRepository reports, IUnitOfWork unitOfWork)
    {
        _reports = reports;
        _unitOfWork = unitOfWork;
    }

    public async Task<ModerationReportDto> Handle(ReviewModerationReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reports.GetByIdAsync(request.ReportId, cancellationToken)
            ?? throw new NotFoundException("ModerationReport", request.ReportId);

        report.Review(request.Status, request.AdminUserId, request.Note);
        await _reports.UpdateAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ModerationReportDto.FromEntity(report);
    }
}
