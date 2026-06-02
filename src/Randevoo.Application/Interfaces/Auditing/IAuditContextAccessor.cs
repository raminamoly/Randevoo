namespace Randevoo.Application.Interfaces.Auditing;

public interface IAuditContextAccessor
{
    string? IpAddress { get; }
    string? CorrelationId { get; }
}
