using Randevoo.Application.Features.Privacy.Common;

namespace Randevoo.Application.Interfaces.Privacy;

public interface IPrivacyDataReader
{
    Task<PrivacyExportDto> ExportUserDataAsync(long userId, CancellationToken cancellationToken = default);
}
