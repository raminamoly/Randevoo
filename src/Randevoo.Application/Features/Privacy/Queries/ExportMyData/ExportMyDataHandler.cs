using MediatR;
using Randevoo.Application.Features.Privacy.Common;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Application.Interfaces.Privacy;
using Randevoo.Domain.Interfaces;

namespace Randevoo.Application.Features.Privacy.Queries.ExportMyData;

public class ExportMyDataHandler : IRequestHandler<ExportMyDataQuery, PrivacyExportDto>
{
    private readonly IPrivacyDataReader _privacyDataReader;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public ExportMyDataHandler(IPrivacyDataReader privacyDataReader, IAuditLogger auditLogger, IUnitOfWork unitOfWork)
    {
        _privacyDataReader = privacyDataReader;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public async Task<PrivacyExportDto> Handle(ExportMyDataQuery request, CancellationToken cancellationToken)
    {
        var result = await _privacyDataReader.ExportUserDataAsync(request.UserId, cancellationToken);
        await _auditLogger.LogAsync(new AuditLogEntry(
            request.UserId,
            "PrivacyDataExported",
            "User",
            request.UserId.ToString(),
            null,
            "{\"status\":\"exported\"}",
            "User requested privacy export"), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}
