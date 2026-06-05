using System.Diagnostics;
using System.Text.Json;
using Randevoo.Application.Interfaces.Auditing;

namespace Randevoo.WebApi.Middleware;

public sealed class ActivityLogMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly RequestDelegate _next;

    public ActivityLogMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IAuditLogger auditLogger)
    {
        var stopwatch = Stopwatch.StartNew();
        await _next(context);
        stopwatch.Stop();

        if (!ShouldTrack(context))
            return;

        var path = context.Request.Path.Value ?? "/";
        var module = ResolveModule(path);
        var logType = ResolveLogType(context);
        var status = context.Response.StatusCode >= 400 ? "failed" : "success";
        var action = ResolveAction(module, logType, status);
        var (targetType, targetId) = ResolveTarget(context, module, path);
        var description = $"{context.Request.Method} {path}";
        var metadata = JsonSerializer.Serialize(new
        {
            method = context.Request.Method,
            statusCode = context.Response.StatusCode,
            elapsedMs = stopwatch.ElapsedMilliseconds
        }, JsonOptions);

        await auditLogger.TryLogAsync(new AuditLogEntry(
            ActorUserId: null,
            Action: action,
            TargetType: targetType,
            TargetId: targetId,
            LogType: logType,
            Module: module,
            Description: description,
            RequestPath: path,
            Status: status,
            MetadataJson: metadata), context.RequestAborted);
    }

    private static bool ShouldTrack(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (!path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return false;

        if (HttpMethods.IsOptions(context.Request.Method))
            return false;

        if (path.StartsWith("/api/auth/", StringComparison.OrdinalIgnoreCase))
            return true;

        return context.User.Identity?.IsAuthenticated == true;
    }

    private static string ResolveLogType(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.Contains("/auth/mobile/request-code", StringComparison.OrdinalIgnoreCase))
            return "login_code_request";
        if (path.Contains("/auth/mobile/verify-code", StringComparison.OrdinalIgnoreCase))
            return context.Response.StatusCode >= 400 ? "failed_login" : "login";
        if (path.Contains("/auth/logout", StringComparison.OrdinalIgnoreCase))
            return context.Response.StatusCode >= 400 ? "logout_failed" : "logout";
        if (path.Contains("/auth/refresh-token", StringComparison.OrdinalIgnoreCase))
            return context.Response.StatusCode >= 400 ? "token_refresh_failed" : "token_refresh";

        if (HttpMethods.IsGet(context.Request.Method))
            return "view";
        if (HttpMethods.IsPost(context.Request.Method))
            return "create";
        if (HttpMethods.IsPut(context.Request.Method) || HttpMethods.IsPatch(context.Request.Method))
            return "update";
        if (HttpMethods.IsDelete(context.Request.Method))
            return "delete";

        return "action";
    }

    private static string ResolveAction(string module, string logType, string status)
    {
        if (logType == "failed_login")
            return "ApiLoginFailed";
        if (logType == "login")
            return "ApiLoginSucceeded";
        if (logType == "logout")
            return "ApiLogout";
        if (logType == "token_refresh")
            return "ApiTokenRefreshed";

        var safeModule = string.IsNullOrWhiteSpace(module) ? "System" : module.Replace("-", "_", StringComparison.Ordinal);
        var safeLogType = logType.Replace("-", "_", StringComparison.Ordinal);
        return $"{safeModule}_{safeLogType}_{status}";
    }

    private static (string TargetType, string TargetId) ResolveTarget(HttpContext context, string module, string fallbackPath)
    {
        var targetId = context.Request.RouteValues
            .FirstOrDefault(item => item.Key.EndsWith("id", StringComparison.OrdinalIgnoreCase))
            .Value?.ToString();

        return (string.IsNullOrWhiteSpace(module) ? "ApiRequest" : module, targetId ?? fallbackPath);
    }

    private static string ResolveModule(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length >= 2 ? segments[1] : "system";
    }
}
