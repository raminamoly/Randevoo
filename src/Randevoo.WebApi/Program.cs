using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Randevoo.Application;
using Randevoo.Infrastructure;
using Randevoo.WebApi.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Server=DESKTOP-5QNHMHJ\\SQL2019;Database=Randevoo;Trusted_Connection=True;TrustServerCertificate=True;";

builder.Services.AddRandevooInfrastructure(connectionString);
builder.Services.AddRandevooApplication();

var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "development-secret-key-change-me-with-at-least-32-chars";
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

app.Run();

public partial class Program;
