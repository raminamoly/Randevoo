using Microsoft.AspNetCore.Authentication.Cookies;
using Randevoo.Application;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.Auth;
using Randevoo.AdminPanel.Services.Infrastructure;
using Randevoo.AdminPanel.Services.State;
using Randevoo.Infrastructure;
using Randevoo.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToFolder("/Account");
    options.Conventions.AllowAnonymousToPage("/Settings/Index");
    options.Conventions.AuthorizeFolder("/Dashboard");
    options.Conventions.AuthorizeFolder("/Events");
    options.Conventions.AuthorizeFolder("/EventTypes");
    options.Conventions.AuthorizeFolder("/Tags");
    options.Conventions.AuthorizeFolder("/Finance");
    options.Conventions.AuthorizeFolder("/Users");
    options.Conventions.AuthorizeFolder("/UserProfiles");
    options.Conventions.AuthorizeFolder("/Settings");
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
builder.Services.AddScoped<IEventTypesApiClient, DatabaseEventTypesApiClient>();
builder.Services.AddScoped<IEventTagsApiClient, DatabaseEventTagsApiClient>();
builder.Services.AddScoped<IUsersApiClient, DatabaseUsersApiClient>();
builder.Services.AddScoped<IUserProfilesApiClient, DatabaseUserProfilesApiClient>();
builder.Services.AddScoped<IAdminUserProfilesApiClient, DatabaseAdminUserProfilesApiClient>();
builder.Services.AddScoped<IDashboardApiClient, DatabaseDashboardApiClient>();
builder.Services.AddScoped<IPlannerProfilesApiClient, DatabasePlannerProfilesApiClient>();
builder.Services.AddScoped<IFinanceApiClient, DatabaseFinanceApiClient>();
builder.Services.AddScoped<ILocationsApiClient, DatabaseLocationsApiClient>();

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
});

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("SampleData:Enabled"))
{
    await app.Services.MigrateAndSeedSampleDataAsync();
}

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

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
