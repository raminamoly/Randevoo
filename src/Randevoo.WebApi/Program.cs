using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Randevoo.Application;
using Randevoo.Infrastructure;
using Randevoo.WebApi.Endpoints;
using Randevoo.WebApi.Hubs;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();

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
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/v1", out var remaining))
        context.Request.Path = $"/api{remaining}";

    await next();
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
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
app.MapPrivacyEndpoints();
app.MapHub<EventChatHub>("/hubs/event-chat");

app.Run();

public partial class Program;
