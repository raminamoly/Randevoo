using Randevoo.Web.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<EndUserEventsApiClient>();
builder.Services.AddScoped<EndUserAuthApiClient>();
builder.Services.AddScoped<EndUserProfileApiClient>();
builder.Services.AddScoped<EndUserTicketsApiClient>();
builder.Services.AddScoped<EndUserSessionService>();
builder.Services.AddHttpClient("RandevooApi", client =>
{
    var baseAddress = builder.Configuration["Api:BaseUrl"];
    if (!string.IsNullOrWhiteSpace(baseAddress))
        client.BaseAddress = new Uri(baseAddress);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseProxy = false
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

var adminUploadsPath = Path.GetFullPath(Path.Combine(
    builder.Environment.ContentRootPath,
    "..",
    "Randevoo.AdminPanel",
    "wwwroot",
    "uploads"));
if (Directory.Exists(adminUploadsPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(adminUploadsPath),
        RequestPath = builder.Configuration["Assets:AdminUploadsRequestPath"] ?? "/admin-uploads"
    });
}

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
