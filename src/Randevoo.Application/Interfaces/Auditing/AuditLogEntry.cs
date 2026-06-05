namespace Randevoo.Application.Interfaces.Auditing;

public sealed record AuditLogEntry(
    long? ActorUserId,
    string Action,
    string TargetType,
    string TargetId,
    string? BeforeJson = null,
    string? AfterJson = null,
    string? Reason = null,
    string? ActorDisplayName = null,
    string? ActorRole = null,
    string? LogType = null,
    string? Module = null,
    string? Description = null,
    string? RequestPath = null,
    string? UserAgent = null,
    string? Status = null,
    string? MetadataJson = null);
