using Randevoo.Application.Interfaces.Auditing;

namespace Randevoo.AdminPanel.Services.Infrastructure;

public sealed class AdminPanelAuditContextAccessor : IAuditContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AdminPanelAuditContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public string? CorrelationId => _httpContextAccessor.HttpContext?.TraceIdentifier;
}
