using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IModerationReportRepository
{
    Task<ModerationReport?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ModerationReport>> ListByReporterAsync(long reporterUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ModerationReport>> ListByStatusAsync(ModerationReportStatus? status, CancellationToken cancellationToken = default);
    Task AddAsync(ModerationReport report, CancellationToken cancellationToken = default);
    Task UpdateAsync(ModerationReport report, CancellationToken cancellationToken = default);
}
