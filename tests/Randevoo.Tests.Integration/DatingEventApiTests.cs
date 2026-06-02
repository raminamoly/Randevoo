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
using Randevoo.Application.Features.Auth.Common;
using Randevoo.Application.Features.Balances.Common;
using Randevoo.Application.Features.DatingEvents.Common;
using Randevoo.Application.Features.DatingProfile.Common;
using Randevoo.Application.Features.EventChats.Common;
using Randevoo.Application.Features.EventParticipants.Common;
using Randevoo.Application.Features.EventPlannerProfiles.Common;
using Randevoo.Application.Features.EventSurveys.Common;
using Randevoo.Application.Features.EventTypes.Common;
using Randevoo.Application.Features.Moderation.Common;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;
using Xunit;

namespace Randevoo.Tests.Integration;

public class DatingEventApiTests
{
    [Fact]
    public async Task EventPlannerCanCreateEvent_AndEndUserCanBuyTicket()
    {
        await using var factory = new RandevooEventApiFactory();
        await factory.SeedEventTypesAsync();
        var client = factory.CreateClient();

        var plannerAuth = await LoginAsync(client, "+989121111111");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var plannerProfileResponse = await client.PutAsJsonAsync("/api/event-planner-profile/me", new
        {
            Title = "Randevoo Nights",
            PictureUrl = "https://example.com/p.jpg",
            Resume = "Experienced event planner for social dating events."
        });
        Assert.Equal(HttpStatusCode.OK, plannerProfileResponse.StatusCode);

        plannerAuth = await LoginAsync(client, "+989121111111");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var createEventResponse = await client.PostAsJsonAsync("/api/dating-events", CreateEventBody());
        Assert.Equal(HttpStatusCode.Created, createEventResponse.StatusCode);
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();
        Assert.NotNull(createdEvent);

        var openResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/open", null);
        Assert.Equal(HttpStatusCode.NoContent, openResponse.StatusCode);

        var locationResponse = await client.PutAsJsonAsync($"/api/dating-events/{createdEvent.Id}/location", new
        {
            Country = "Iran",
            City = "Shiraz",
            Region = "Central",
            Latitude = 29.5918m,
            Longitude = 52.5837m,
            Address = "Updated event address"
        });
        Assert.Equal(HttpStatusCode.NoContent, locationResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(999, "+989120000000", UserRole.Admin));
        var commissionResponse = await client.PutAsJsonAsync($"/api/dating-events/{createdEvent.Id}/commission", new
        {
            CommissionPercent = 12.5m
        });
        Assert.Equal(HttpStatusCode.NoContent, commissionResponse.StatusCode);

        var openEvents = await client.GetFromJsonAsync<List<DatingEventDto>>("/api/dating-events/open");
        Assert.NotNull(openEvents);
        var updatedEvent = Assert.Single(openEvents);
        Assert.Equal("Shiraz", updatedEvent.City);
        Assert.Equal("Updated event address", updatedEvent.Address);
        Assert.Equal(12.5m, updatedEvent.EventPlannerCommissionPercent);

        var endUserAuth = await LoginAsync(client, "+989122222222");
        await factory.SeedAdminAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(999, "+989120000000", UserRole.Admin));
        var adjustResponse = await client.PostAsJsonAsync($"/api/balances/{endUserAuth.UserId}/adjust", new
        {
            Amount = 500m,
            Description = "Test top up"
        });
        Assert.Equal(HttpStatusCode.OK, adjustResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", endUserAuth.Token);
        var profileResponse = await client.PostAsJsonAsync("/api/dating-profiles", new
        {
            UserId = endUserAuth.UserId,
            DisplayName = "TicketBuyer",
            DateOfBirth = new DateOnly(1998, 1, 1),
            Gender = Gender.Male,
            Country = "Iran",
            City = "Tehran",
            Latitude = 35.6895m,
            Longitude = 51.3890m,
            HeightCm = 178
        });
        Assert.Equal(HttpStatusCode.Created, profileResponse.StatusCode);

        var buyResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/tickets", null);
        Assert.Equal(HttpStatusCode.Created, buyResponse.StatusCode);

        var buyerBalance = await client.GetFromJsonAsync<BalanceDto>("/api/balances/me");
        Assert.NotNull(buyerBalance);
        Assert.Equal(400m, buyerBalance.Balance);
    }

    [Fact]
    public async Task EndUserCannotCreateDatingEvent()
    {
        await using var factory = new RandevooEventApiFactory();
        await factory.SeedEventTypesAsync();
        var client = factory.CreateClient();
        var auth = await LoginAsync(client, "+989123333333");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);

        var response = await client.PostAsJsonAsync("/api/dating-events", CreateEventBody());

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task EventParticipantsCanUseArchiveProfilesChatAndSurvey_AndPlannerCanRemoveWithRefund()
    {
        await using var factory = new RandevooEventApiFactory();
        await factory.SeedEventTypesAsync();
        var client = factory.CreateClient();

        var plannerAuth = await LoginAsync(client, "+989125000000");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);
        var plannerProfileResponse = await client.PutAsJsonAsync("/api/event-planner-profile/me", new
        {
            Title = "After Event Planner",
            PictureUrl = "https://example.com/planner.jpg",
            Resume = "Planner for post event flows."
        });
        Assert.Equal(HttpStatusCode.OK, plannerProfileResponse.StatusCode);

        plannerAuth = await LoginAsync(client, "+989125000000");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);
        var createEventResponse = await client.PostAsJsonAsync("/api/dating-events", CreatePastEventBody());
        Assert.Equal(HttpStatusCode.Created, createEventResponse.StatusCode);
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();
        Assert.NotNull(createdEvent);

        var openResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/open", null);
        Assert.Equal(HttpStatusCode.NoContent, openResponse.StatusCode);

        var firstUser = await CreateFundedProfileAndTicketAsync(factory, client, "+989125000001", createdEvent.Id, "ParticipantOne", Gender.Male);
        var secondUser = await CreateFundedProfileAndTicketAsync(factory, client, "+989125000002", createdEvent.Id, "ParticipantTwo", Gender.Female);
        var thirdUser = await CreateFundedProfileAndTicketAsync(factory, client, "+989125000003", createdEvent.Id, "ParticipantThree", Gender.Male);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firstUser.Token);
        var visibleProfiles = await client.GetFromJsonAsync<List<DatingProfileDto>>($"/api/event-participants/events/{createdEvent.Id}/profiles");
        Assert.NotNull(visibleProfiles);
        Assert.Equal(2, visibleProfiles.Count);

        var archive = await client.GetFromJsonAsync<List<EventArchiveItemDto>>("/api/event-participants/me/archive");
        Assert.NotNull(archive);
        Assert.Contains(archive, item => item.EventId == createdEvent.Id);

        var startConversationResponse = await client.PostAsJsonAsync($"/api/event-chats/events/{createdEvent.Id}/conversations", new
        {
            ParticipantUserId = secondUser.UserId
        });
        Assert.Equal(HttpStatusCode.Created, startConversationResponse.StatusCode);
        var conversation = await startConversationResponse.Content.ReadFromJsonAsync<EventConversationDto>();
        Assert.NotNull(conversation);

        var messageResponse = await client.PostAsJsonAsync($"/api/event-chats/conversations/{conversation.Id}/messages", new
        {
            Body = "Nice to meet you at the event."
        });
        Assert.Equal(HttpStatusCode.OK, messageResponse.StatusCode);

        var reportResponse = await client.PostAsJsonAsync("/api/moderation-reports", new
        {
            ReportedUserId = secondUser.UserId,
            DatingEventId = createdEvent.Id,
            EventConversationId = conversation.Id,
            Reason = ModerationReportReason.Harassment,
            Description = "Participant sent uncomfortable messages."
        });
        Assert.Equal(HttpStatusCode.Created, reportResponse.StatusCode);
        var createdReport = await reportResponse.Content.ReadFromJsonAsync<ModerationReportDto>();
        Assert.NotNull(createdReport);
        Assert.Equal(ModerationReportStatus.Pending, createdReport.Status);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(999, "+989120000000", UserRole.Admin));
        var pendingReports = await client.GetFromJsonAsync<List<ModerationReportDto>>("/api/moderation-reports/admin?status=Pending");
        Assert.NotNull(pendingReports);
        Assert.Contains(pendingReports, report => report.Id == createdReport.Id);

        var reviewReportResponse = await client.PutAsJsonAsync($"/api/moderation-reports/{createdReport.Id}/review", new
        {
            Status = ModerationReportStatus.Reviewed,
            Note = "Reviewed in integration test."
        });
        Assert.Equal(HttpStatusCode.OK, reviewReportResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firstUser.Token);
        var blockResponse = await client.PostAsJsonAsync($"/api/event-chats/conversations/{conversation.Id}/blocks", new
        {
            BlockedUserId = secondUser.UserId
        });
        Assert.Equal(HttpStatusCode.NoContent, blockResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondUser.Token);
        var blockedMessageResponse = await client.PostAsJsonAsync($"/api/event-chats/conversations/{conversation.Id}/messages", new
        {
            Body = "Can you see this?"
        });
        Assert.Equal(HttpStatusCode.BadRequest, blockedMessageResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firstUser.Token);
        var overLimitConversationResponse = await client.PostAsJsonAsync($"/api/event-chats/events/{createdEvent.Id}/conversations", new
        {
            ParticipantUserId = thirdUser.UserId
        });
        Assert.Equal(HttpStatusCode.BadRequest, overLimitConversationResponse.StatusCode);

        var surveyResponse = await client.PostAsJsonAsync($"/api/event-surveys/events/{createdEvent.Id}/me", new
        {
            Ratings = new[]
            {
                new { Factor = SurveyFactor.OverallExperience, Score = 5 },
                new { Factor = SurveyFactor.EventOrganization, Score = 4 },
                new { Factor = SurveyFactor.VenueAndLocation, Score = 4 },
                new { Factor = SurveyFactor.ParticipantQuality, Score = 5 },
                new { Factor = SurveyFactor.SafetyAndComfort, Score = 5 }
            },
            Comment = "Great event."
        });
        Assert.Equal(HttpStatusCode.OK, surveyResponse.StatusCode);
        var survey = await surveyResponse.Content.ReadFromJsonAsync<EventSurveyDto>();
        Assert.NotNull(survey);
        Assert.Equal(5, survey.Ratings.Count);
        var plannerProfile = await factory.GetEventPlannerProfileAsync(plannerAuth.UserId);
        Assert.NotNull(plannerProfile);
        Assert.Equal(1, plannerProfile.TotalSurveyCount);
        Assert.Equal(4.6m, plannerProfile.AverageRating);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);
        var participants = await client.GetFromJsonAsync<List<EventParticipantDto>>($"/api/event-participants/events/{createdEvent.Id}/participants");
        Assert.NotNull(participants);
        Assert.Equal(3, participants.Count);
        Assert.Contains(participants, participant => participant.UserId == firstUser.UserId && participant.MobileNumber == "+989125000001");

        var removeResponse = await client.PostAsJsonAsync($"/api/event-participants/events/{createdEvent.Id}/participants/{thirdUser.UserId}/remove", new
        {
            Reason = "Emergency safety removal"
        });
        Assert.Equal(HttpStatusCode.NoContent, removeResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", thirdUser.Token);
        var removedProfileAccessResponse = await client.GetAsync($"/api/event-participants/events/{createdEvent.Id}/profiles");
        Assert.Equal(HttpStatusCode.BadRequest, removedProfileAccessResponse.StatusCode);

        var removedArchive = await client.GetFromJsonAsync<List<EventArchiveItemDto>>("/api/event-participants/me/archive");
        Assert.NotNull(removedArchive);
        var removedArchiveItem = Assert.Single(removedArchive, item => item.EventId == createdEvent.Id);
        Assert.True(removedArchiveItem.IsRemoved);
        Assert.True(removedArchiveItem.IsRefunded);

        var removedBalance = await client.GetFromJsonAsync<BalanceDto>("/api/balances/me");
        Assert.NotNull(removedBalance);
        Assert.Contains(removedBalance.Transactions, transaction => transaction.Type == BalanceTransactionType.EmergencyRemovalRefund);
    }

    [Fact]
    public async Task ActiveEventTypesAreSeededAndVisible()
    {
        await using var factory = new RandevooEventApiFactory();
        var client = factory.CreateClient();
        await factory.SeedEventTypesAsync();

        var eventTypes = await client.GetFromJsonAsync<List<EventTypeDto>>("/api/event-types");

        Assert.NotNull(eventTypes);
        Assert.Contains(eventTypes, eventType => eventType.Name == "Mafia");
        Assert.Contains(eventTypes, eventType => eventType.Name == "Speed Dating");
    }

    [Fact]
    public async Task AdminCanChangeUserRole()
    {
        await using var factory = new RandevooEventApiFactory();
        var client = factory.CreateClient();
        var auth = await LoginAsync(client, "+989124444444");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(999, "+989120000000", UserRole.Admin));
        var response = await client.PutAsJsonAsync($"/api/admin/users/{auth.UserId}/role", new
        {
            Role = UserRole.EventPlanner
        });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(UserRole.EventPlanner, await factory.GetUserRoleAsync(auth.UserId));
    }

    private static object CreateEventBody() => new
    {
        Title = "Mafia Night",
        Country = "Iran",
        City = "Tehran",
        Region = "District 1",
        Latitude = 35.6895m,
        Longitude = 51.3890m,
        Address = "Main social club address",
        DateTimeStart = DateTime.UtcNow.AddDays(7),
        DateTimeEnd = DateTime.UtcNow.AddDays(7).AddHours(3),
        EventTypeId = 1L,
        MaleMinAge = 18,
        MaleMaxAge = 45,
        FemaleMinAge = 18,
        FemaleMaxAge = 45,
        MaleCapacity = 10,
        FemaleCapacity = 10,
        NumberOfChatAllowed = 3,
        TicketPrice = 100m,
        EventImage1 = "https://example.com/1.jpg",
        EventImage2 = "https://example.com/2.jpg",
        EventImage3 = "https://example.com/3.jpg",
        EventDescriptionHtml = "<p>A friendly mafia game night.</p>",
        EventPlannerCommissionPercent = 10m
    };

    private static object CreatePastEventBody() => new
    {
        Title = "Completed Social Night",
        Country = "Iran",
        City = "Tehran",
        Region = "District 2",
        Latitude = 35.6895m,
        Longitude = 51.3890m,
        Address = "Past event address",
        DateTimeStart = DateTime.UtcNow.AddHours(-4),
        DateTimeEnd = DateTime.UtcNow.AddHours(-1),
        EventTypeId = 3L,
        MaleMinAge = 18,
        MaleMaxAge = 45,
        FemaleMinAge = 18,
        FemaleMaxAge = 45,
        MaleCapacity = 10,
        FemaleCapacity = 10,
        NumberOfChatAllowed = 1,
        TicketPrice = 100m,
        EventImage1 = "https://example.com/past1.jpg",
        EventImage2 = "https://example.com/past2.jpg",
        EventImage3 = "https://example.com/past3.jpg",
        EventDescriptionHtml = "<p>A completed social event.</p>",
        EventPlannerCommissionPercent = 10m
    };

    private static async Task<AuthResult> CreateFundedProfileAndTicketAsync(
        RandevooEventApiFactory factory,
        HttpClient client,
        string mobileNumber,
        long eventId,
        string displayName,
        Gender gender)
    {
        var auth = await LoginAsync(client, mobileNumber);
        await factory.SeedAdminAsync();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(999, "+989120000000", UserRole.Admin));
        var adjustResponse = await client.PostAsJsonAsync($"/api/balances/{auth.UserId}/adjust", new
        {
            Amount = 500m,
            Description = "Test top up"
        });
        Assert.Equal(HttpStatusCode.OK, adjustResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
        var profileResponse = await client.PostAsJsonAsync("/api/dating-profiles", new
        {
            UserId = auth.UserId,
            DisplayName = displayName,
            DateOfBirth = new DateOnly(1998, 1, 1),
            Gender = gender,
            Country = "Iran",
            City = "Tehran",
            Latitude = 35.6895m,
            Longitude = 51.3890m,
            HeightCm = 178
        });
        Assert.Equal(HttpStatusCode.Created, profileResponse.StatusCode);

        var buyResponse = await client.PostAsync($"/api/dating-events/{eventId}/tickets", null);
        Assert.Equal(HttpStatusCode.Created, buyResponse.StatusCode);
        return auth;
    }

    private static async Task<AuthResult> LoginAsync(HttpClient client, string mobileNumber)
    {
        await client.PostAsJsonAsync("/api/auth/mobile/request-code", new { MobileNumber = mobileNumber });
        var response = await client.PostAsJsonAsync("/api/auth/mobile/verify-code", new { MobileNumber = mobileNumber, Code = "123456" });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<AuthResult>())!;
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

    private sealed class RandevooEventApiFactory : WebApplicationFactory<Program>
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

        public async Task SeedAdminAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            if (await db.Users.AnyAsync(u => u.Id == 999))
                return;

            var admin = new User("+989120000000");
            admin.ChangeUserRole(UserRole.Admin);
            db.Users.Add(admin);
            await db.SaveChangesAsync();
        }

        public async Task<UserRole> GetUserRoleAsync(long userId)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            return await db.Users
                .Where(user => user.Id == userId)
                .Select(user => user.Role)
                .SingleAsync();
        }

        public async Task<EventPlannerProfileDto?> GetEventPlannerProfileAsync(long userId)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            var profile = await db.EventPlannerProfiles.SingleOrDefaultAsync(profile => profile.UserId == userId);
            return profile is null ? null : EventPlannerProfileDto.FromEntity(profile);
        }

        public async Task SeedEventTypesAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            if (await db.EventTypes.AnyAsync())
                return;

            db.EventTypes.AddRange(
                new EventType("Mafia"),
                new EventType("Board Game"),
                new EventType("Poem Reading"),
                new EventType("Cafe Meetup"),
                new EventType("Hiking"),
                new EventType("Speed Dating"),
                new EventType("Game Tournament"),
                new EventType("Workshop"),
                new EventType("Art Night"),
                new EventType("Music Night"));
            await db.SaveChangesAsync();
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
