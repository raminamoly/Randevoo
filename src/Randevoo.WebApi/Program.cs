using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Randevoo.Application;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Infrastructure;
using Randevoo.Infrastructure.Data;
using Randevoo.WebApi.Endpoints;
using Randevoo.WebApi.Hubs;
using Randevoo.WebApi.Middleware;
using Randevoo.WebApi.Services;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "Randevoo.WebApi")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
});

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditContextAccessor, HttpAuditContextAccessor>();

var allowDevelopmentFallbacks = builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    if (!allowDevelopmentFallbacks)
        throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required outside Development/Testing.");

    connectionString = "Server=localhost;Database=Randevoo;Trusted_Connection=True;TrustServerCertificate=True;";
}

builder.Services.AddRandevooInfrastructure(connectionString);
builder.Services.AddRandevooApplication();

var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret))
{
    if (!allowDevelopmentFallbacks)
        throw new InvalidOperationException("Jwt:Secret is required outside Development/Testing.");

    jwtSecret = "development-secret-key-change-me-with-at-least-32-chars";
}

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Randevoo";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Randevoo";
var enableSampleData = builder.Configuration.GetValue<bool>("SampleData:Enabled");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EndUserOnly", policy => policy.RequireRole("EndUser", "Admin"));
    options.AddPolicy("EventPlannerOnly", policy => policy.RequireRole("EventPlanner", "Admin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SupportOrAdmin", policy => policy.RequireRole("platform-support-team", "PlatformSupportTeam", "Admin"));
});

var app = builder.Build();

if (enableSampleData)
{
    await app.Services.MigrateAndSeedSampleDataAsync();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/v1", out var remaining))
        context.Request.Path = $"/api{remaining}";

    await next();
});

app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("CorrelationId", httpContext.TraceIdentifier);
        diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
        diagnosticContext.Set("RequestPath", httpContext.Request.Path.Value);
        diagnosticContext.Set("ClientIp", httpContext.Connection.RemoteIpAddress?.ToString());

        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            if (long.TryParse(httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                diagnosticContext.Set("UserId", userId);

            diagnosticContext.Set("UserRole", string.Join(",", httpContext.User.FindAll(ClaimTypes.Role).Select(claim => claim.Value)));
        }
    };
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ActivityLogMiddleware>();
app.MapAuthEndpoints();
app.MapDatingProfileEndpoints();
app.MapEventPlannerProfileEndpoints();
app.MapBalanceEndpoints();
app.MapDatingEventEndpoints();
app.MapUserAdminEndpoints();
app.MapEventParticipantEndpoints();
app.MapEventChatEndpoints();
app.MapEventSurveyEndpoints();
app.MapEventTypeEndpoints();
app.MapModerationEndpoints();
app.MapSupportTicketEndpoints();
app.MapPrivacyEndpoints();
app.MapHub<EventChatHub>("/hubs/event-chat");

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
