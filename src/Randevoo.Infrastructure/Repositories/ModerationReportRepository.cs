using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class ModerationReportRepository : IModerationReportRepository
{
    private readonly RandevooDbContext _db;

    public ModerationReportRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<ModerationReport?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _db.ModerationReports
            .Include(report => report.ReporterUser)
            .Include(report => report.ReportedUser)
            .Include(report => report.DatingEvent)
            .Include(report => report.EventConversation)
            .FirstOrDefaultAsync(report => report.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<ModerationReport>> ListByReporterAsync(long reporterUserId, CancellationToken cancellationToken = default)
    {
        return await _db.ModerationReports
            .Include(report => report.DatingEvent)
            .Where(report => report.ReporterUserId == reporterUserId)
            .OrderByDescending(report => report.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ModerationReport>> ListByStatusAsync(ModerationReportStatus? status, CancellationToken cancellationToken = default)
    {
        var query = _db.ModerationReports
            .Include(report => report.ReporterUser)
            .Include(report => report.ReportedUser)
            .Include(report => report.DatingEvent)
            .AsQueryable();

        if (status is not null)
            query = query.Where(report => report.Status == status);

        return await query.OrderByDescending(report => report.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(ModerationReport report, CancellationToken cancellationToken = default)
    {
        _db.ModerationReports.Add(report);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(ModerationReport report, CancellationToken cancellationToken = default)
    {
        _db.ModerationReports.Update(report);
        await Task.CompletedTask;
    }
}
