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
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;
using Xunit;

namespace Randevoo.Tests.Integration;

public class DatingProfileApiTests
{
    [Fact]
    public async Task CreateAndGetDatingProfile_ReturnsCreatedProfile()
    {
        await using var factory = new RandevooApiFactory();
        var userId = await factory.SeedUserAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(userId, "+989121234567", UserRole.EndUser));

        var response = await client.PostAsJsonAsync("/api/dating-profiles", new
        {
            UserId = userId,
            DisplayName = "Ramin",
            DateOfBirth = new DateOnly(1995, 1, 1),
            Gender = Gender.Male,
            Country = "Iran",
            City = "Tehran",
            Latitude = 35.6895m,
            Longitude = 51.3890m,
            HeightCm = 177
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<DatingProfileResponse>();
        Assert.NotNull(created);
        Assert.Equal("Ramin", created.DisplayName);
        Assert.Equal(userId, created.UserId);

        var getResponse = await client.GetAsync($"/api/dating-profiles/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateDatingProfile_WithMissingUser_ReturnsNotFound()
    {
        await using var factory = new RandevooApiFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(999, "+989129999999", UserRole.EndUser));

        var response = await client.PostAsJsonAsync("/api/dating-profiles", new
        {
            UserId = 999,
            DisplayName = "MissingUser",
            DateOfBirth = new DateOnly(1995, 1, 1),
            Gender = Gender.Male,
            Country = "Iran",
            City = "Tehran",
            Latitude = 35.6895m,
            Longitude = 51.3890m,
            HeightCm = 177
        });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAndDeleteDatingProfile_ChangesProfileThenHidesDeletedProfile()
    {
        await using var factory = new RandevooApiFactory();
        var userId = await factory.SeedUserAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(userId, "+989121234567", UserRole.EndUser));
        var profile = await CreateProfileAsync(client, userId, "OriginalName");

        var updateResponse = await client.PutAsJsonAsync($"/api/dating-profiles/{profile.Id}", new
        {
            DisplayName = "UpdatedName",
            Gender = Gender.Female,
            Country = "Iran",
            City = "Shiraz",
            Latitude = 29.5918m,
            Longitude = 52.5837m,
            HeightCm = 168,
            EducationLevel = EducationLevel.Graduated,
            Smoking = false,
            Region = "Fars"
        });

        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        var updated = await client.GetFromJsonAsync<DatingProfileResponse>($"/api/dating-profiles/{profile.Id}");
        Assert.NotNull(updated);
        Assert.Equal("UpdatedName", updated.DisplayName);

        var deleteResponse = await client.DeleteAsync($"/api/dating-profiles/{profile.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getDeletedResponse = await client.GetAsync($"/api/dating-profiles/{profile.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
    }

    [Fact]
    public async Task CreateDatingProfile_WithoutJwt_ReturnsUnauthorized()
    {
        await using var factory = new RandevooApiFactory();
        var userId = await factory.SeedUserAsync();
        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/dating-profiles", new
        {
            UserId = userId,
            DisplayName = "Anonymous",
            DateOfBirth = new DateOnly(1995, 1, 1),
            Gender = Gender.Male,
            Country = "Iran",
            City = "Tehran",
            Latitude = 35.6895m,
            Longitude = 51.3890m,
            HeightCm = 177
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDatingProfile_ForAnotherUser_ReturnsForbidden()
    {
        await using var factory = new RandevooApiFactory();
        var ownerUserId = await factory.SeedUserAsync("+989121234567");
        var otherUserId = await factory.SeedUserAsync("+989121234568", resetDatabase: false);
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(ownerUserId, "+989121234567", UserRole.EndUser));
        var profile = await CreateProfileAsync(client, ownerUserId, "OwnerProfile");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(otherUserId, "+989121234568", UserRole.EndUser));
        var updateResponse = await client.PutAsJsonAsync($"/api/dating-profiles/{profile.Id}", new
        {
            DisplayName = "BadUpdate",
            Gender = Gender.Female,
            Country = "Iran",
            City = "Shiraz",
            Latitude = 29.5918m,
            Longitude = 52.5837m,
            HeightCm = 168,
            EducationLevel = EducationLevel.Graduated,
            Smoking = false,
            Region = "Fars"
        });

        Assert.Equal(HttpStatusCode.Forbidden, updateResponse.StatusCode);
    }

    [Fact]
    public async Task AdminCanReadUserDatingProfile()
    {
        await using var factory = new RandevooApiFactory();
        var ownerUserId = await factory.SeedUserAsync("+989121234567");
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(ownerUserId, "+989121234567", UserRole.EndUser));
        var profile = await CreateProfileAsync(client, ownerUserId, "OwnerProfile");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(999, "+989129999999", UserRole.Admin));
        var response = await client.GetAsync($"/api/dating-profiles/{profile.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private sealed record DatingProfileResponse(long Id, long UserId, string DisplayName);

    private static async Task<DatingProfileResponse> CreateProfileAsync(HttpClient client, long userId, string displayName)
    {
        var response = await client.PostAsJsonAsync("/api/dating-profiles", new
        {
            UserId = userId,
            DisplayName = displayName,
            DateOfBirth = new DateOnly(1995, 1, 1),
            Gender = Gender.Male,
            Country = "Iran",
            City = "Tehran",
            Latitude = 35.6895m,
            Longitude = 51.3890m,
            HeightCm = 177
        });

        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<DatingProfileResponse>())!;
    }

    private sealed class RandevooApiFactory : WebApplicationFactory<Program>
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

        public async Task<long> SeedUserAsync(string mobileNumber = "+989121234567", bool resetDatabase = true)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            if (resetDatabase)
            {
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();
            }

            var user = new User(mobileNumber);
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user.Id;
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
}
