using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Randevoo.Application;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Infrastructure;
using Randevoo.Infrastructure.Data;
using Randevoo.WebApi.Endpoints;
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
if (builder.Environment.IsDevelopment())
    builder.Services.Replace(ServiceDescriptor.Singleton<ICodeGenerator, DevelopmentFixedCodeGenerator>());
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
});

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();

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
    app.MapGet("/api-docs", () => Results.Redirect("/scalar/v1"))
        .ExcludeFromDescription();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ActivityLogMiddleware>();
app.MapAuthEndpoints();
app.MapDatingProfileEndpoints();
app.MapDatingEventEndpoints();
app.MapEndUserEventEndpoints();
app.MapEventParticipantEndpoints();

try
{
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
