using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Features.Moderation.Common;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Moderation.Commands.ReviewModerationReport;

public class ReviewModerationReportHandler : IRequestHandler<ReviewModerationReportCommand, ModerationReportDto>
{
    private readonly IModerationReportRepository _reports;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<ReviewModerationReportHandler> _logger;

    public ReviewModerationReportHandler(IModerationReportRepository reports, IUnitOfWork unitOfWork, IAuditLogger auditLogger, ILogger<ReviewModerationReportHandler> logger)
    {
        _reports = reports;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task<ModerationReportDto> Handle(ReviewModerationReportCommand request, CancellationToken cancellationToken)
    {
        var report = await _reports.GetByIdAsync(request.ReportId, cancellationToken)
            ?? throw new NotFoundException("ModerationReport", request.ReportId);

        var oldStatus = report.Status;
        report.Review(request.Status, request.AdminUserId, request.Note);
        await _auditLogger.LogAsync(new AuditLogEntry(
            request.AdminUserId,
            "ModerationReportReviewed",
            "ModerationReport",
            report.Id.ToString(),
            $"{{\"status\":\"{oldStatus}\"}}",
            $"{{\"status\":\"{report.Status}\"}}",
            request.Note), cancellationToken);

        await _reports.UpdateAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Admin {AdminUserId} reviewed moderation report {ReportId} from {OldStatus} to {NewStatus}", request.AdminUserId, report.Id, oldStatus, report.Status);
        return ModerationReportDto.FromEntity(report);
    }
}
