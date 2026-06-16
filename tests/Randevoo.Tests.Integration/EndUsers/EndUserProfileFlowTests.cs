using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Randevoo.Application.Features.DatingProfile.Common;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;
using Xunit;

namespace Randevoo.Tests.Integration.EndUsers;

public class EndUserProfileFlowTests
{
    [Fact]
    public async Task CreateProfile_WithEducation_ReturnsReadyToBuyStatus()
    {
        await using var factory = new EndUserProfileApiFactory();
        var userId = await factory.SeedUserAsync("+989188000001");
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CreateToken(userId, "+989188000001", UserRole.EndUser));

        var response = await client.PostAsJsonAsync("/api/v1/platform/profile/me", new
        {
            DisplayName = "Ready Buyer",
            DateOfBirth = new DateOnly(1994, 1, 1),
            Gender = Gender.Male,
            Country = "Iran",
            City = "Tehran",
            Latitude = 35.6895m,
            Longitude = 51.3890m,
            HeightCm = 178,
            EducationLevel = EducationLevel.Graduated,
            Smoking = false,
            Region = "Vanak"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var profile = await response.Content.ReadFromJsonAsync<DatingProfileDto>();
        Assert.NotNull(profile);
        Assert.Equal(UserProfileStatus.ReadyToBuy, profile.ProfileStatus);

        var byUser = await client.GetFromJsonAsync<DatingProfileDto>("/api/v1/platform/profile/me");
        Assert.NotNull(byUser);
        Assert.Equal(UserProfileStatus.ReadyToBuy, byUser.ProfileStatus);
    }

    private static string CreateToken(long userId, string mobileNumber, UserRole role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("development-secret-key-change-me-with-at-least-32-chars"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "Randevoo",
            audience: "Randevoo",
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("mobile_number", mobileNumber),
                new Claim(ClaimTypes.Role, role.ToString())
            ],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private sealed class EndUserProfileApiFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = Guid.NewGuid().ToString("N");

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

                services.AddDbContext<RandevooDbContext>(options => options.UseInMemoryDatabase(_databaseName));
                services.AddSingleton<ICodeGenerator, FixedCodeGenerator>();
                services.AddSingleton<ISmsSender, NoopNotifications>();
                services.AddSingleton<IEmailSender, NoopNotifications>();
            });
        }

        public async Task<long> SeedUserAsync(string mobileNumber)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            var user = new User(mobileNumber);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user.Id;
        }
    }

    private sealed class FixedCodeGenerator : ICodeGenerator
    {
        public string GenerateNumericCode(int length) => "123456";
        public string GenerateToken() => "email-token";
    }

    private sealed class NoopNotifications : ISmsSender, IEmailSender
    {
        public Task SendLoginCodeAsync(string mobileNumber, string code, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendMessageAsync(string mobileNumber, string message, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendEmailConfirmationAsync(string email, string confirmationLink, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
