using System.Net;
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
    public async Task MobileLogin_CompletesPasswordlessAuthFlow()
    {
        await using var factory = new RandevooAuthApiFactory();
        var client = factory.CreateClient();

        var requestCodeResponse = await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/request-code", new
        {
            MobileNumber = "+989121234567"
        });

        Assert.Equal(HttpStatusCode.Accepted, requestCodeResponse.StatusCode);
        Assert.Equal("123456", factory.Notifications.LastLoginCode);

        var verifyResponse = await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/verify", new
        {
            MobileNumber = "+989121234567",
            Code = "123456"
        });

        Assert.Equal(HttpStatusCode.OK, verifyResponse.StatusCode);
        var auth = await verifyResponse.Content.ReadFromJsonAsync<AuthResult>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth.Token));
        Assert.True(auth.AccessTokenExpiresAtUtc > DateTime.UtcNow);
    }

    [Fact]
    public async Task MobileLoginRequest_WhenRequestedTooOften_ReturnsBadRequest()
    {
        await using var factory = new RandevooAuthApiFactory();
        var client = factory.CreateClient();
        var body = new { MobileNumber = "+989121234570" };

        Assert.Equal(HttpStatusCode.Accepted, (await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/request-code", body)).StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, (await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/request-code", body)).StatusCode);
        Assert.Equal(HttpStatusCode.Accepted, (await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/request-code", body)).StatusCode);

        var limitedResponse = await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/request-code", body);

        Assert.Equal(HttpStatusCode.BadRequest, limitedResponse.StatusCode);
    }

    [Fact]
    public async Task MobileLoginVerify_WhenWrongCodeRepeatedly_LocksLogin()
    {
        await using var factory = new RandevooAuthApiFactory();
        var client = factory.CreateClient();
        var mobileNumber = "+989121234571";

        await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/request-code", new { MobileNumber = mobileNumber });

        for (var i = 0; i < 5; i++)
        {
            var wrongResponse = await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/verify", new
            {
                MobileNumber = mobileNumber,
                Code = "000000"
            });
            Assert.Equal(HttpStatusCode.BadRequest, wrongResponse.StatusCode);
        }

        var lockedResponse = await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/verify", new
        {
            MobileNumber = mobileNumber,
            Code = "123456"
        });

        Assert.Equal(HttpStatusCode.BadRequest, lockedResponse.StatusCode);
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

        public Task SendLoginCodeAsync(string mobileNumber, string code, CancellationToken cancellationToken = default)
        {
            LastLoginCode = code;
            return Task.CompletedTask;
        }

        public Task SendMessageAsync(string mobileNumber, string message, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FixedCodeGenerator : ICodeGenerator
    {
        public string GenerateNumericCode(int length) => "123456";
        public string GenerateToken() => $"token-{Guid.NewGuid():N}";
    }
}
