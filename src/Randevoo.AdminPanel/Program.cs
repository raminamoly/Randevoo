using Microsoft.AspNetCore.Authentication.Cookies;
using Randevoo.Application;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.Auth;
using Randevoo.AdminPanel.Services.Infrastructure;
using Randevoo.AdminPanel.Services.Permissions;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Infrastructure;
using Randevoo.Infrastructure.Data;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToFolder("/Account");
    options.Conventions.AuthorizeFolder("/Dashboard");
    options.Conventions.AuthorizeFolder("/Events");
    options.Conventions.AuthorizeFolder("/EventTypes");
    options.Conventions.AuthorizeFolder("/DiscountCodes");
    options.Conventions.AuthorizeFolder("/Tags");
    options.Conventions.AuthorizeFolder("/Finance");
    options.Conventions.AuthorizeFolder("/SpecialOperations", Policies.SupportOrAdmin);
    options.Conventions.AuthorizeFolder("/Support", Policies.AdminPlannerOrSupport);
    options.Conventions.AuthorizeFolder("/Participants", Policies.AdminPlannerOrSupport);
    options.Conventions.AuthorizeFolder("/Buyers", Policies.AdminPlannerOrSupport);
    options.Conventions.AuthorizeFolder("/Notifications", Policies.AdminPlannerOrSupport);
    options.Conventions.AuthorizeFolder("/Logs");
    options.Conventions.AuthorizeFolder("/Users");
    options.Conventions.AuthorizeFolder("/UserProfiles");
    options.Conventions.AuthorizeFolder("/Settings", Policies.AdminOnly);
});

builder.Services.AddHttpContextAccessor();

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
builder.Services.AddScoped<IAuditContextAccessor, AdminPanelAuditContextAccessor>();
builder.Services.AddScoped<CurrentSessionState>();
builder.Services.AddScoped<MockAuthService>();
builder.Services.AddScoped<IEventsApiClient, DatabaseEventsApiClient>();
builder.Services.AddScoped<IEventDiscountCodesApiClient, DatabaseEventDiscountCodesApiClient>();
builder.Services.AddScoped<IEventTypesApiClient, DatabaseEventTypesApiClient>();
builder.Services.AddScoped<IEventTagsApiClient, DatabaseEventTagsApiClient>();
builder.Services.AddScoped<IUsersApiClient, DatabaseUsersApiClient>();
builder.Services.AddScoped<IUserProfilesApiClient, DatabaseUserProfilesApiClient>();
builder.Services.AddScoped<IAdminUserProfilesApiClient, DatabaseAdminUserProfilesApiClient>();
builder.Services.AddScoped<IDashboardApiClient, DatabaseDashboardApiClient>();
builder.Services.AddScoped<IAdminAnalyticsApiClient, DatabaseAdminAnalyticsApiClient>();
builder.Services.AddScoped<IPlannerProfilesApiClient, DatabasePlannerProfilesApiClient>();
builder.Services.AddScoped<IFinanceApiClient, DatabaseFinanceApiClient>();
builder.Services.AddScoped<ISpecialOperationsApiClient, DatabaseSpecialOperationsApiClient>();
builder.Services.AddScoped<INotificationsApiClient, DatabaseNotificationsApiClient>();
builder.Services.AddScoped<ILocationsApiClient, DatabaseLocationsApiClient>();
builder.Services.AddScoped<ISupportTicketsApiClient, DatabaseSupportTicketsApiClient>();
builder.Services.AddScoped<IOperationPermissionService, DatabaseOperationPermissionService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Randevoo.AdminPanel.Auth";
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Forbidden";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminOnly, policy =>
        policy.RequireRole(AdminRole.Admin.ToString()));

    options.AddPolicy(Policies.AdminOrPlanner, policy =>
        policy.RequireRole(
            AdminRole.Admin.ToString(),
            AdminRole.EventPlanner.ToString()));

    options.AddPolicy(Policies.SupportOrAdmin, policy =>
        policy.RequireRole(
            AdminRole.Admin.ToString(),
            AdminRole.SupportTeam.ToString()));

    options.AddPolicy(Policies.AdminPlannerOrSupport, policy =>
        policy.RequireRole(
            AdminRole.Admin.ToString(),
            AdminRole.EventPlanner.ToString(),
            AdminRole.SupportTeam.ToString()));
});

var app = builder.Build();

await app.Services.InitializeDatabaseAsync();

await app.Services.SyncOperationPermissionCatalogAsync();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    var state = context.RequestServices.GetRequiredService<CurrentSessionState>();
    var cookieValue = context.Request.Cookies["randevoo.admin.lang"];
    state.Refresh(context.User, cookieValue);
    await next();
});
app.UseMiddleware<AdminActivityLogMiddleware>();

app.MapPost("/activity/track", async (
    AdminActivityTrackRequest request,
    HttpContext context,
    IAuditLogger auditLogger,
    CurrentSessionState session) =>
{
    var currentUser = session.CurrentUser;
    if (currentUser is null)
        return Results.Unauthorized();

    var path = string.IsNullOrWhiteSpace(request.Path) ? context.Request.Headers.Referer.ToString() : request.Path!.Trim();
    var logType = string.IsNullOrWhiteSpace(request.Type) ? "click" : request.Type.Trim().ToLowerInvariant();
    var action = string.IsNullOrWhiteSpace(request.Action) ? $"Admin{char.ToUpperInvariant(logType[0])}{logType[1..]}" : request.Action.Trim();
    var module = string.IsNullOrWhiteSpace(request.Module)
        ? path.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "dashboard"
        : request.Module.Trim().ToLowerInvariant();

    var metadata = request.Metadata is null
        ? new Dictionary<string, object?>()
        : request.Metadata.ToDictionary(item => item.Key, item => (object?)item.Value);

    if (request.DurationSeconds.HasValue)
        metadata["durationSeconds"] = request.DurationSeconds.Value;

    var metadataJson = metadata.Count == 0 ? null : JsonSerializer.Serialize(metadata);

    await auditLogger.TryLogAsync(new AuditLogEntry(
        ActorUserId: currentUser.Id,
        Action: action,
        TargetType: "Page",
        TargetId: string.IsNullOrWhiteSpace(path) ? "/" : path,
        ActorDisplayName: currentUser.FullName,
        ActorRole: currentUser.Role.ToString(),
        LogType: logType,
        Module: module,
        Description: request.Description,
        RequestPath: string.IsNullOrWhiteSpace(path) ? "/" : path,
        Status: "success",
        MetadataJson: metadataJson), context.RequestAborted);

    return Results.Accepted();
}).RequireAuthorization(Policies.AdminOrPlanner);

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
