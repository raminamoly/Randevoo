using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Entities;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Services;

public class AuditLogger : IAuditLogger
{
    private readonly RandevooDbContext _db;
    private readonly IAuditContextAccessor _auditContextAccessor;

    public AuditLogger(RandevooDbContext db, IAuditContextAccessor auditContextAccessor)
    {
        _db = db;
        _auditContextAccessor = auditContextAccessor;
    }

    public Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog(
            entry.ActorUserId,
            entry.Action,
            entry.TargetType,
            entry.TargetId,
            entry.BeforeJson,
            entry.AfterJson,
            entry.Reason,
            _auditContextAccessor.IpAddress,
            _auditContextAccessor.CorrelationId);

        _db.AuditLogs.Add(auditLog);
        return Task.CompletedTask;
    }
}
