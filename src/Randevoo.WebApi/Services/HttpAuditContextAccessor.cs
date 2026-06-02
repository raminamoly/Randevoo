using Randevoo.Application.Interfaces.Auditing;

namespace Randevoo.WebApi.Services;

public class HttpAuditContextAccessor : IAuditContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpAuditContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? CorrelationId => _httpContextAccessor.HttpContext?.TraceIdentifier;
}
