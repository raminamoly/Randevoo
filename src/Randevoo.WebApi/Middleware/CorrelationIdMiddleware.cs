using Serilog.Context;

namespace Randevoo.WebApi.Middleware;

public class CorrelationIdMiddleware
{
    public const string HeaderName = "X-Correlation-ID";
    private const int MaxCorrelationIdLength = 100;

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = GetOrCreateCorrelationId(context);
        context.TraceIdentifier = correlationId;
        context.Response.Headers.TryAdd(HeaderName, correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        var value = context.Request.Headers[HeaderName].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(value))
            return Guid.NewGuid().ToString("N");

        var trimmed = value.Trim();
        return trimmed.Length <= MaxCorrelationIdLength ? trimmed : trimmed[..MaxCorrelationIdLength];
    }
}
