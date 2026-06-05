using Randevoo.Application.Interfaces.Auditing;

namespace Randevoo.AdminPanel.Services.Infrastructure;

public sealed class AdminActivityLogMiddleware
{
    private readonly RequestDelegate _next;

    public AdminActivityLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogger auditLogger)
    {
        await _next(context);

        if (!ShouldTrack(context))
            return;

        var path = context.Request.Path.Value ?? "/";
        var module = ResolveModule(path);
        var status = context.Response.StatusCode >= 400 ? "failed" : "success";

        await auditLogger.TryLogAsync(new AuditLogEntry(
            ActorUserId: null,
            Action: "AdminPageViewed",
            TargetType: "Page",
            TargetId: path,
            LogType: "page_view",
            Module: module,
            Description: $"Viewed admin page {path}",
            RequestPath: path,
            Status: status), context.RequestAborted);
    }

    private static bool ShouldTrack(HttpContext context)
    {
        if (!HttpMethods.IsGet(context.Request.Method))
            return false;

        if (context.User.Identity?.IsAuthenticated != true)
            return false;

        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/lib/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/css/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/js/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/favicon", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/activity/track", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !path.StartsWith("/Account/Login", StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolveModule(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length == 0 ? "dashboard" : segments[0].ToLowerInvariant();
    }
}
