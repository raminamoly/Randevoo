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

    public Task<bool> HasOpenDuplicateAsync(long reporterUserId, long reportedUserId, long? datingEventId, CancellationToken cancellationToken = default)
    {
        return _db.ModerationReports.AnyAsync(
            report =>
                report.ReporterUserId == reporterUserId
                && report.ReportedUserId == reportedUserId
                && report.DatingEventId == datingEventId
                && report.Status == ModerationReportStatus.Pending,
            cancellationToken);
    }

    public async Task<IReadOnlyList<ModerationReport>> ListByReporterAsync(long reporterUserId, int limit = 50, long? afterId = null, DateTime? createdAfter = null, CancellationToken cancellationToken = default)
    {
        var query = _db.ModerationReports
            .Include(report => report.DatingEvent)
            .Where(report => report.ReporterUserId == reporterUserId);

        if (afterId is not null)
            query = query.Where(report => report.Id < afterId);
        if (createdAfter is not null)
            query = query.Where(report => report.CreatedAt >= createdAfter);

        return await query
            .OrderByDescending(report => report.Id)
            .Take(Math.Clamp(limit, 1, 100))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ModerationReport>> ListByStatusAsync(ModerationReportStatus? status, int limit = 50, long? afterId = null, DateTime? createdAfter = null, CancellationToken cancellationToken = default)
    {
        var query = _db.ModerationReports
            .Include(report => report.ReporterUser)
            .Include(report => report.ReportedUser)
            .Include(report => report.DatingEvent)
            .AsQueryable();

        if (status is not null)
            query = query.Where(report => report.Status == status);
        if (afterId is not null)
            query = query.Where(report => report.Id < afterId);
        if (createdAfter is not null)
            query = query.Where(report => report.CreatedAt >= createdAfter);

        return await query
            .OrderByDescending(report => report.Id)
            .Take(Math.Clamp(limit, 1, 100))
            .ToListAsync(cancellationToken);
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
