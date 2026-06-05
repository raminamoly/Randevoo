namespace Randevoo.Application.Interfaces.Auditing;

public interface IAuditContextAccessor
{
    string? IpAddress { get; }
    string? CorrelationId { get; }
    string? RequestPath { get; }
    string? UserAgent { get; }
    long? CurrentUserId { get; }
    string? CurrentUserName { get; }
    string? CurrentUserRole { get; }
}
