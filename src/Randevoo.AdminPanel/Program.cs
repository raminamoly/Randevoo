using Microsoft.AspNetCore.Authentication.Cookies;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.AdminPanel.Services.Auth;
using Randevoo.AdminPanel.Services.MockData;
using Randevoo.AdminPanel.Services.State;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AllowAnonymousToPage("/Index");
    options.Conventions.AllowAnonymousToFolder("/Account");
    options.Conventions.AllowAnonymousToPage("/Settings/Index");
    options.Conventions.AuthorizeFolder("/Dashboard");
    options.Conventions.AuthorizeFolder("/Events");
    options.Conventions.AuthorizeFolder("/Users");
    options.Conventions.AuthorizeFolder("/Settings");
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<AdminPanelStore>();
builder.Services.AddScoped<CurrentSessionState>();
builder.Services.AddScoped<MockAuthService>();
builder.Services.AddScoped<IEventsApiClient, MockEventsApiClient>();
builder.Services.AddScoped<IUsersApiClient, MockUsersApiClient>();
builder.Services.AddScoped<IDashboardApiClient, MockDashboardApiClient>();
builder.Services.AddScoped<IPlannerProfilesApiClient, MockPlannerProfilesApiClient>();

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
        policy.RequireRole(
            AdminRole.Admin.ToString(),
            AdminRole.SupportTeam.ToString()));

    options.AddPolicy(Policies.AdminOrPlanner, policy =>
        policy.RequireRole(
            AdminRole.Admin.ToString(),
            AdminRole.EventPlanner.ToString(),
            AdminRole.SupportTeam.ToString()));
});

var app = builder.Build();

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
