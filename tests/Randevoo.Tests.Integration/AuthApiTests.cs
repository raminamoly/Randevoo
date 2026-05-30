using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Randevoo.Application.Features.Auth.Common;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Infrastructure.Data;
using Xunit;

namespace Randevoo.Tests.Integration;

public class AuthApiTests
{
    [Fact]
    public async Task MobileLogin_ThenEmailConfirmation_CompletesPasswordlessAuthFlow()
    {
        await using var factory = new RandevooAuthApiFactory();
        var client = factory.CreateClient();

        var requestCodeResponse = await client.PostAsJsonAsync("/api/auth/mobile/request-code", new
        {
            MobileNumber = "+989121234567"
        });

        Assert.Equal(HttpStatusCode.Accepted, requestCodeResponse.StatusCode);
        Assert.Equal("123456", factory.Notifications.LastLoginCode);

        var verifyResponse = await client.PostAsJsonAsync("/api/auth/mobile/verify-code", new
        {
            MobileNumber = "+989121234567",
            Code = "123456"
        });

        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
        var auth = await verifyResponse.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        var emailResponse = await client.PostAsJsonAsync("/api/auth/email/request-confirmation", new
        {
            Email = "Ramin.Amoly@gmail.com"
        });

        Assert.Equal(HttpStatusCode.Accepted, emailResponse.StatusCode);
        Assert.NotNull(factory.Notifications.LastConfirmationLink);

        var confirmationUri = new Uri(factory.Notifications.LastConfirmationLink!);
        var confirmResponse = await client.GetAsync(confirmationUri.PathAndQuery);

        Assert.Equal(HttpStatusCode.OK, confirmResponse.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
        var user = await db.Users.SingleAsync();
        Assert.Equal("ramin.amoly@gmail.com", user.Email);
        Assert.True(user.IsEmailConfirmed);
    }

    [Fact]
    public async Task EmailConfirmationRequest_WithoutJwt_ReturnsUnauthorized()
    {
        await using var factory = new RandevooAuthApiFactory();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/email/request-confirmation", new
        {
            Email = "ramin.amoly@gmail.com"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private sealed class RandevooAuthApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = Guid.NewGuid().ToString("N");

        public NotificationCapture Notifications { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<RandevooDbContext>>();
                services.RemoveAll<DbContextOptions>();
                services.RemoveAll<IDbContextOptionsConfiguration<RandevooDbContext>>();
                services.RemoveAll<RandevooDbContext>();
                services.RemoveAll<ICodeGenerator>();
                services.RemoveAll<ISmsSender>();
                services.RemoveAll<IEmailSender>();

                services.AddDbContext<RandevooDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
                services.AddSingleton<ICodeGenerator, FixedCodeGenerator>();
                services.AddSingleton(Notifications);
                services.AddSingleton<ISmsSender>(sp => sp.GetRequiredService<NotificationCapture>());
                services.AddSingleton<IEmailSender>(sp => sp.GetRequiredService<NotificationCapture>());
            });
        }
    }

    private sealed class NotificationCapture : ISmsSender, IEmailSender
    {
        public string? LastLoginCode { get; private set; }
        public string? LastConfirmationLink { get; private set; }

        public Task SendLoginCodeAsync(string mobileNumber, string code, CancellationToken cancellationToken = default)
        {
            LastLoginCode = code;
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(string mobileNumber, string message, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default)
        {
            LastConfirmationLink = confirmationLink;
            return Task.CompletedTask;
        }
    }

    private sealed class FixedCodeGenerator : ICodeGenerator
    {
        public string GenerateNumericCode(int length) => "123456";

        public string GenerateToken() => "email-confirm-token";
    }
}
