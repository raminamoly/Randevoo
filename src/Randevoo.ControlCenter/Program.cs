using Randevoo.ControlCenter.Components;
using Randevoo.ControlCenter.Services.ApiClients;
using Randevoo.ControlCenter.Services.Auth;
using Randevoo.ControlCenter.Services.MockData;
using Randevoo.ControlCenter.Services.State;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddScoped<MockAuthState>();
builder.Services.AddScoped<AppUiState>();
builder.Services.AddScoped<LanguageState>();
builder.Services.AddSingleton<ControlCenterMockData>();
builder.Services.AddScoped<IControlCenterAuthClient, MockControlCenterAuthClient>();
builder.Services.AddScoped<IDashboardApiClient, MockDashboardApiClient>();
builder.Services.AddScoped<IEventsApiClient, MockEventsApiClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
