namespace Randevoo.Application.Interfaces.Auditing;

public sealed record AuditLogEntry(
    long? ActorUserId,
    string Action,
    string TargetType,
    string TargetId,
    string? BeforeJson = null,
    string? AfterJson = null,
    string? Reason = null);
