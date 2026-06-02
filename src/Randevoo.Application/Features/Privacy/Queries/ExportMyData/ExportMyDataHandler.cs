using MediatR;
using Randevoo.Application.Features.Privacy.Common;
using Randevoo.Application.Interfaces.Privacy;

namespace Randevoo.Application.Features.Privacy.Queries.ExportMyData;

public class ExportMyDataHandler : IRequestHandler<ExportMyDataQuery, PrivacyExportDto>
{
    private readonly IPrivacyDataReader _privacyDataReader;

    public ExportMyDataHandler(IPrivacyDataReader privacyDataReader)
    {
        _privacyDataReader = privacyDataReader;
    }

    public Task<PrivacyExportDto> Handle(ExportMyDataQuery request, CancellationToken cancellationToken)
    {
        return _privacyDataReader.ExportUserDataAsync(request.UserId, cancellationToken);
    }
}
