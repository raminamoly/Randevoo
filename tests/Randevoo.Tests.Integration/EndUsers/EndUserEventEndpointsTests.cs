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
using Randevoo.Application.EndUsers.Events;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;
using Xunit;

namespace Randevoo.Tests.Integration.EndUsers;

public class EndUserEventEndpointsTests
{
    [Fact]
    public async Task ListEndUserEvents_ReturnsEligibleRecommendedEvents()
    {
        await using var factory = new EndUserEventApiFactory();
        var seed = await factory.SeedCatalogAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CreateToken(seed.UserId, "+989199000001", UserRole.EndUser));

        var response = await client.GetAsync("/api/v1/platform/events?onlyEligibleForMe=true&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<EndUserEventCatalogPageDto>();
        Assert.NotNull(page);
        Assert.NotEmpty(page.Items);
        var item = Assert.Single(page.Items);
        Assert.Equal(seed.EventId, item.Id);
        Assert.True(item.IsEligibleForCurrentUser);
        Assert.True(item.RecommendationScore > 0);
        Assert.Equal(UserProfileStatus.Complete, item.CurrentUserProfileStatus);
    }

    [Fact]
    public async Task GetEndUserEventDetails_ReturnsPurchaseReadiness()
    {
        await using var factory = new EndUserEventApiFactory();
        var seed = await factory.SeedCatalogAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CreateToken(seed.UserId, "+989199000001", UserRole.EndUser));

        var details = await client.GetFromJsonAsync<EndUserEventDetailsDto>($"/api/v1/platform/events/{seed.EventId}");

        Assert.NotNull(details);
        Assert.Equal(seed.EventId, details.Id);
        Assert.True(details.CanBuyTicket, $"{details.EligibilityReasonCode}: {details.EligibilityMessage}");
        Assert.Equal("eligible", details.EligibilityReasonCode);
        Assert.Equal("Randevoo Organizer", details.Organizer.Title);
    }

    [Fact]
    public async Task ListEndUserEvents_WithOnlyEligibleForMe_RequiresLogin()
    {
        await using var factory = new EndUserEventApiFactory();
        await factory.SeedCatalogAsync();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/platform/events?onlyEligibleForMe=true");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task WebsiteEvents_ReturnsPublicCatalogWithoutLogin()
    {
        await using var factory = new EndUserEventApiFactory();
        var seed = await factory.SeedCatalogAsync();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/website/events?pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<EndUserEventCatalogPageDto>();
        Assert.NotNull(page);
        Assert.Contains(page.Items, item => item.Id == seed.EventId);
        Assert.Contains(page.Items, item => item.Title == "Isfahan poetry warmup");
    }

    [Fact]
    public async Task PlatformEvents_DoesNotImplicitlyFilterByProfileCity()
    {
        await using var factory = new EndUserEventApiFactory();
        var seed = await factory.SeedCatalogAsync();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            CreateToken(seed.UserId, "+989199000001", UserRole.EndUser));

        var response = await client.GetAsync("/api/v1/platform/events?pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<EndUserEventCatalogPageDto>();
        Assert.NotNull(page);
        Assert.Contains(page.Items, item => item.Id == seed.EventId);
        Assert.Contains(page.Items, item => item.Title == "Isfahan poetry warmup");
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

    private sealed record SeedResult(long UserId, long EventId);

    private sealed class EndUserEventApiFactory : WebApplicationFactory<Program>
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

        public async Task<SeedResult> SeedCatalogAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();

            var interest = new Interest("Board games", "Social");
            var tag = new Tag("بازی");
            db.Interests.Add(interest);
            db.Tags.Add(tag);
            await db.SaveChangesAsync();

            db.InterestTagMappings.Add(new InterestTagMapping(interest, tag, 80));

            var user = new User("+989199000001");
            user.CreateProfile(
                "End User",
                new DateOnly(1992, 1, 1),
                Gender.Male,
                new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
                new Height(178));
            user.Profile!.UpdateEducationLevel(EducationLevel.Graduated);
            user.Profile.AddInterest(interest);

            var planner = new User("+989199000002");
            planner.ChangeUserRole(UserRole.EventPlanner);
            var plannerProfile = new EventPlannerProfile(
                planner,
                "Randevoo Organizer",
                "https://example.com/organizer.jpg",
                "Organizer profile created for integration tests.");
            plannerProfile.UpdateMetrics(4.6m, 12, 4, 0, 3);

            var eventType = new EventType("Cafe Meetup");
            var datingEvent = new DatingEvent(
                planner,
                "Tehran social night",
                new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
                "Main test venue",
                DateTime.UtcNow.AddDays(3),
                DateTime.UtcNow.AddDays(3).AddHours(3),
                eventType,
                new AgeRange(25, 45),
                new AgeRange(25, 45),
                20,
                20,
                3,
                150m,
                120m,
                EventEducationLevelRestriction.BachelorOrHigher,
                null,
                "https://example.com/event.jpg",
                null,
                null,
                "<p>Integration test event description.</p>");
            datingEvent.SetLocationLookup(1, 1);
            datingEvent.ReplaceTags(new[] { tag });
            datingEvent.ApproveByAdmin();
            datingEvent.OpenForSell();

            var upcomingClosedEvent = new DatingEvent(
                planner,
                "Isfahan poetry warmup",
                new Location("Iran", "Isfahan", new Coordinates(32.6546m, 51.6680m)),
                "Poetry house | Main avenue 24",
                DateTime.UtcNow.AddDays(5),
                DateTime.UtcNow.AddDays(5).AddHours(2),
                eventType,
                new AgeRange(23, 44),
                new AgeRange(23, 42),
                14,
                16,
                3,
                110m,
                95m,
                EventEducationLevelRestriction.WithoutLimit,
                null,
                "https://example.com/event-closed.jpg",
                null,
                null,
                "<p>Closed-sale but still visible for public discovery.</p>");
            upcomingClosedEvent.SetLocationLookup(1, 4);
            upcomingClosedEvent.ReplaceTags(new[] { tag });
            upcomingClosedEvent.ApproveByAdmin();

            db.Users.AddRange(user, planner);
            db.EventPlannerProfiles.Add(plannerProfile);
            db.EventTypes.Add(eventType);
            db.DatingEvents.AddRange(datingEvent, upcomingClosedEvent);
            await db.SaveChangesAsync();

            return new SeedResult(user.Id, datingEvent.Id);
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
