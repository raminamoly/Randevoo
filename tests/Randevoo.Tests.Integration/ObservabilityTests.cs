using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;
using Randevoo.WebApi.Middleware;
using Xunit;

namespace Randevoo.Tests.Integration;

public class ObservabilityTests
{
    [Fact]
    public async Task GlobalExceptionMiddleware_ReturnsSafeProblem_WithCorrelationId()
    {
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "exception-correlation-123";
        var middleware = new GlobalExceptionMiddleware(
            _ => throw new InvalidOperationException("sensitive internal detail"),
            NullLogger<GlobalExceptionMiddleware>.Instance,
            new TestEnvironment("Production"));

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
    }

    [Fact]
    public async Task AdminRoleChange_CreatesAuditLog_WithCorrelationId()
    {
        await using var factory = new RandevooObservabilityFactory();
        var (adminId, targetUserId) = await factory.SeedAdminAndUserAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(adminId, "+989121234500", UserRole.Admin));
        client.DefaultRequestHeaders.Add("X-Correlation-ID", "test-correlation-123");

        var response = await client.PutAsJsonAsync($"/api/admin/users/{targetUserId}/role", new
        {
            Role = UserRole.EventPlanner
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
        var auditLog = await db.AuditLogs.SingleAsync();
        Assert.Equal("UserRoleChanged", auditLog.Action);
        Assert.Equal(adminId, auditLog.ActorUserId);
        Assert.Equal("User", auditLog.TargetType);
        Assert.Equal(targetUserId.ToString(), auditLog.TargetId);
        Assert.Equal("test-correlation-123", auditLog.CorrelationId);
    }

    private sealed class RandevooObservabilityFactory : WebApplicationFactory<Program>
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

                services.AddDbContext<RandevooDbContext>(options =>
                    options.UseInMemoryDatabase(_databaseName));
            });
        }

        public async Task<(long AdminId, long TargetUserId)> SeedAdminAndUserAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            var admin = new User("+989121234500");
            admin.ChangeUserRole(UserRole.Admin);
            var target = new User("+989121234501");
            db.Users.AddRange(admin, target);
            await db.SaveChangesAsync();
            return (admin.Id, target.Id);
        }
    }

    private static string CreateToken(long userId, string mobileNumber, UserRole role)
    {
        const string secret = "development-secret-key-change-me-with-at-least-32-chars";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            "Randevoo",
            "Randevoo",
            new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("mobile_number", mobileNumber),
                new Claim(ClaimTypes.Role, role.ToString())
            },
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private sealed class TestEnvironment : Microsoft.AspNetCore.Hosting.IWebHostEnvironment
    {
        public TestEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "Randevoo.Tests";
        public string WebRootPath { get; set; } = string.Empty;
        public Microsoft.Extensions.FileProviders.IFileProvider WebRootFileProvider { get; set; } = new Microsoft.Extensions.FileProviders.NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public Microsoft.Extensions.FileProviders.IFileProvider ContentRootFileProvider { get; set; } = new Microsoft.Extensions.FileProviders.NullFileProvider();
    }
}
