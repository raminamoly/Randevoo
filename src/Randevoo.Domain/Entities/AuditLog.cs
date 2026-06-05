using Randevoo.Domain.Common;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class AuditLog : BaseEntity
{
    public long? ActorUserId { get; private set; }
    public string? ActorDisplayName { get; private set; }
    public string? ActorRole { get; private set; }
    public string Action { get; private set; } = null!;
    public string LogType { get; private set; } = null!;
    public string? Module { get; private set; }
    public string? Description { get; private set; }
    public string TargetType { get; private set; } = null!;
    public string TargetId { get; private set; } = null!;
    public string? BeforeJson { get; private set; }
    public string? AfterJson { get; private set; }
    public string? Reason { get; private set; }
    public string? IpAddress { get; private set; }
    public string? RequestPath { get; private set; }
    public string? UserAgent { get; private set; }
    public string Status { get; private set; } = null!;
    public string? MetadataJson { get; private set; }
    public string? CorrelationId { get; private set; }

    private AuditLog() { }

    public AuditLog(
        long? actorUserId,
        string? actorDisplayName,
        string? actorRole,
        string action,
        string logType,
        string? module,
        string? description,
        string targetType,
        string targetId,
        string? beforeJson = null,
        string? afterJson = null,
        string? reason = null,
        string? ipAddress = null,
        string? requestPath = null,
        string? userAgent = null,
        string? status = null,
        string? metadataJson = null,
        string? correlationId = null)
    {
        ActorUserId = actorUserId;
        ActorDisplayName = actorDisplayName;
        ActorRole = actorRole;
        Action = GuardAgainst.String.NullOrWhiteSpace(action, nameof(action));
        LogType = GuardAgainst.String.NullOrWhiteSpace(logType, nameof(logType));
        Module = module;
        Description = description;
        TargetType = GuardAgainst.String.NullOrWhiteSpace(targetType, nameof(targetType));
        TargetId = GuardAgainst.String.NullOrWhiteSpace(targetId, nameof(targetId));
        BeforeJson = beforeJson;
        AfterJson = afterJson;
        Reason = reason;
        IpAddress = ipAddress;
        RequestPath = requestPath;
        UserAgent = userAgent;
        Status = GuardAgainst.String.NullOrWhiteSpace(status ?? "success", nameof(status));
        MetadataJson = metadataJson;
        CorrelationId = correlationId;
    }

    public override void SoftDelete()
    {
        throw new BusinessRuleViolationException("Audit logs are append-only", "Audit log records cannot be deleted");
    }
}
