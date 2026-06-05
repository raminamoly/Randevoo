using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Entities;
using Randevoo.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Randevoo.Infrastructure.Services;

public class AuditLogger : IAuditLogger
{
    private readonly RandevooDbContext _db;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IAuditContextAccessor _auditContextAccessor;
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(
        RandevooDbContext db,
        IServiceScopeFactory scopeFactory,
        IAuditContextAccessor auditContextAccessor,
        ILogger<AuditLogger> logger)
    {
        _db = db;
        _scopeFactory = scopeFactory;
        _auditContextAccessor = auditContextAccessor;
        _logger = logger;
    }

    public Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        _db.AuditLogs.Add(BuildAuditLog(entry));
        return Task.CompletedTask;
    }

    public async Task TryLogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var scope = _scopeFactory.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            db.AuditLogs.Add(BuildAuditLog(entry));
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist activity log for action {Action} on target {TargetType}/{TargetId}", entry.Action, entry.TargetType, entry.TargetId);
        }
    }

    private AuditLog BuildAuditLog(AuditLogEntry entry)
    {
        return new AuditLog(
            entry.ActorUserId ?? _auditContextAccessor.CurrentUserId,
            entry.ActorDisplayName ?? _auditContextAccessor.CurrentUserName,
            entry.ActorRole ?? _auditContextAccessor.CurrentUserRole,
            entry.Action,
            entry.LogType ?? "audit",
            entry.Module,
            entry.Description,
            entry.TargetType,
            entry.TargetId,
            entry.BeforeJson,
            entry.AfterJson,
            entry.Reason,
            _auditContextAccessor.IpAddress,
            entry.RequestPath ?? _auditContextAccessor.RequestPath,
            entry.UserAgent ?? _auditContextAccessor.UserAgent,
            entry.Status ?? "success",
            entry.MetadataJson,
            _auditContextAccessor.CorrelationId);
    }
}
