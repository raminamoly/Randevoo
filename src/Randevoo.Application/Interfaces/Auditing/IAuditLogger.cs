namespace Randevoo.Application.Interfaces.Auditing;

public interface IAuditLogger
{
    Task LogAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
}
