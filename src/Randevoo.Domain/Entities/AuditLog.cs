using Randevoo.Domain.Common;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class AuditLog : BaseEntity
{
    public long? ActorUserId { get; private set; }
    public string Action { get; private set; } = null!;
    public string TargetType { get; private set; } = null!;
    public string TargetId { get; private set; } = null!;
    public string? BeforeJson { get; private set; }
    public string? AfterJson { get; private set; }
    public string? Reason { get; private set; }
    public string? IpAddress { get; private set; }
    public string? CorrelationId { get; private set; }

    private AuditLog() { }

    public AuditLog(
        long? actorUserId,
        string action,
        string targetType,
        string targetId,
        string? beforeJson = null,
        string? afterJson = null,
        string? reason = null,
        string? ipAddress = null,
        string? correlationId = null)
    {
        ActorUserId = actorUserId;
        Action = GuardAgainst.String.NullOrWhiteSpace(action, nameof(action));
        TargetType = GuardAgainst.String.NullOrWhiteSpace(targetType, nameof(targetType));
        TargetId = GuardAgainst.String.NullOrWhiteSpace(targetId, nameof(targetId));
        BeforeJson = beforeJson;
        AfterJson = afterJson;
        Reason = reason;
        IpAddress = ipAddress;
        CorrelationId = correlationId;
    }

    public override void SoftDelete()
    {
        throw new BusinessRuleViolationException("Audit logs are append-only", "Audit log records cannot be deleted");
    }
}
