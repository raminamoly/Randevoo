using System.Security.Claims;
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

    public string? RequestPath => _httpContextAccessor.HttpContext?.Request.Path.Value;

    public string? UserAgent => _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

    public long? CurrentUserId
        => long.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId
            : null;

    public string? CurrentUserName => _httpContextAccessor.HttpContext?.User.Identity?.Name;

    public string? CurrentUserRole => _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}
