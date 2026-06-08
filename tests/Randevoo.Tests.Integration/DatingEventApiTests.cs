using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
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
        var createEventBody = await createEventResponse.Content.ReadAsStringAsync();
        Assert.True(createEventResponse.StatusCode == HttpStatusCode.Created, $"Create event failed with {(int)createEventResponse.StatusCode}: {createEventBody}");
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();
        Assert.NotNull(createdEvent);
        Assert.Equal(EventEducationLevelRestriction.WithoutLimit, createdEvent.EducationLevelRestriction);
        Assert.Equal("IRR", createdEvent.MaleTicketCurrencyCode);
        Assert.Equal("IRR", createdEvent.FemaleTicketCurrencyCode);

        await factory.ApproveEventAsync(createdEvent.Id);
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
        var adminUserId = await factory.SeedAdminAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(adminUserId, "+989120000000", UserRole.Admin));
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
        var createdProfile = await profileResponse.Content.ReadFromJsonAsync<DatingProfileDto>();
        Assert.NotNull(createdProfile);

        var updateProfileResponse = await client.PutAsJsonAsync($"/api/dating-profiles/{createdProfile.Id}", new
        {
            DisplayName = createdProfile.DisplayName,
            Gender = createdProfile.Gender,
            Country = createdProfile.Country,
            City = createdProfile.City,
            Latitude = createdProfile.Latitude,
            Longitude = createdProfile.Longitude,
            HeightCm = createdProfile.HeightCm,
            EducationLevel = EducationLevel.Graduated,
            Smoking = false,
            Region = createdProfile.Region
        });
        Assert.Equal(HttpStatusCode.NoContent, updateProfileResponse.StatusCode);

        var buyResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/tickets", null);
        Assert.Equal(HttpStatusCode.Created, buyResponse.StatusCode);

        var buyerBalance = await client.GetFromJsonAsync<BalanceDto>("/api/balances/me");
        Assert.NotNull(buyerBalance);
        Assert.Equal(400m, buyerBalance.Balance);
    }

    [Fact]
    public async Task CreateEvent_UsesOneSharedTicketCurrency()
    {
        await using var factory = new RandevooEventApiFactory();
        await factory.SeedEventTypesAsync();
        var client = factory.CreateClient();

        var plannerAuth = await LoginAsync(client, "+989121111119");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var plannerProfileResponse = await client.PutAsJsonAsync("/api/event-planner-profile/me", new
        {
            Title = "Currency Planner",
            PictureUrl = "https://example.com/currency.jpg",
            Resume = "Planner profile for currency tests.",
            SettlementCurrencyCode = "USD"
        });
        Assert.Equal(HttpStatusCode.OK, plannerProfileResponse.StatusCode);

        plannerAuth = await LoginAsync(client, "+989121111119");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var createEventResponse = await client.PostAsJsonAsync(
            "/api/dating-events",
            CreateEventBody(maleTicketCurrencyCode: "USD", femaleTicketCurrencyCode: "CAD"));

        var createEventBody = await createEventResponse.Content.ReadAsStringAsync();
        Assert.True(createEventResponse.StatusCode == HttpStatusCode.Created, $"Create event failed with {(int)createEventResponse.StatusCode}: {createEventBody}");
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();

        Assert.NotNull(createdEvent);
        Assert.Equal("USD", createdEvent.MaleTicketCurrencyCode);
        Assert.Equal("USD", createdEvent.FemaleTicketCurrencyCode);
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
    public async Task TicketPurchaseFails_WhenBuyerEducationDoesNotMeetEventRestriction()
    {
        await using var factory = new RandevooEventApiFactory();
        await factory.SeedEventTypesAsync();
        var client = factory.CreateClient();

        var plannerAuth = await LoginAsync(client, "+989121111112");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var plannerProfileResponse = await client.PutAsJsonAsync("/api/event-planner-profile/me", new
        {
            Title = "Restricted Planner",
            PictureUrl = "https://example.com/p.jpg",
            Resume = "Planner for education restriction tests."
        });
        Assert.Equal(HttpStatusCode.OK, plannerProfileResponse.StatusCode);

        plannerAuth = await LoginAsync(client, "+989121111112");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var createEventResponse = await client.PostAsJsonAsync("/api/dating-events", CreateEventBody(EventEducationLevelRestriction.BachelorOrHigher));
        var createEventBody = await createEventResponse.Content.ReadAsStringAsync();
        Assert.True(createEventResponse.StatusCode == HttpStatusCode.Created, $"Create event failed with {(int)createEventResponse.StatusCode}: {createEventBody}");
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();
        Assert.NotNull(createdEvent);

        await factory.ApproveEventAsync(createdEvent.Id);
        var openResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/open", null);
        Assert.Equal(HttpStatusCode.NoContent, openResponse.StatusCode);

        var buyer = await CreateUserWithProfileAsync(factory, client, "+989126555555", "EducationBuyer", Gender.Male, EducationLevel.Diploma);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyer.Token);
        var buyResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/tickets", null);

        Assert.Equal(HttpStatusCode.BadRequest, buyResponse.StatusCode);
        var problem = await buyResponse.Content.ReadAsStringAsync();
        Assert.Contains("education", problem, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TicketPurchase_AppliesDiscountCode_AndChargesDiscountedAmount()
    {
        await using var factory = new RandevooEventApiFactory();
        await factory.SeedEventTypesAsync();
        var client = factory.CreateClient();

        var plannerAuth = await LoginAsync(client, "+989121111113");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var plannerProfileResponse = await client.PutAsJsonAsync("/api/event-planner-profile/me", new
        {
            Title = "Discount Planner",
            PictureUrl = "https://example.com/p.jpg",
            Resume = "Planner for discount code tests."
        });
        Assert.Equal(HttpStatusCode.OK, plannerProfileResponse.StatusCode);

        plannerAuth = await LoginAsync(client, "+989121111113");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var createEventResponse = await client.PostAsJsonAsync("/api/dating-events", CreateEventBody());
        var createEventBody = await createEventResponse.Content.ReadAsStringAsync();
        Assert.True(createEventResponse.StatusCode == HttpStatusCode.Created, $"Create event failed with {(int)createEventResponse.StatusCode}: {createEventBody}");
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();
        Assert.NotNull(createdEvent);

        await factory.ApproveEventAsync(createdEvent.Id);
        var openResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/open", null);
        Assert.Equal(HttpStatusCode.NoContent, openResponse.StatusCode);

        await factory.AddDiscountCodeAsync(createdEvent.Id, "SAVE25", EventDiscountGenderScope.Male, EventDiscountType.Percentage, 25m);

        var buyer = await CreateUserWithProfileAsync(factory, client, "+989126777777", "DiscountBuyer", Gender.Male, EducationLevel.Graduated);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyer.Token);

        var buyResponse = await client.PostAsJsonAsync($"/api/dating-events/{createdEvent.Id}/tickets", new
        {
            DiscountCode = "save25"
        });

        Assert.Equal(HttpStatusCode.Created, buyResponse.StatusCode);

        var balance = await client.GetFromJsonAsync<BalanceDto>("/api/balances/me");
        Assert.NotNull(balance);
        Assert.Equal(425m, balance.Balance);
    }

    [Fact]
    public async Task TicketPurchase_ForOrganizerManualTransfer_DebitsPlannerForPlatformCommission()
    {
        await using var factory = new RandevooEventApiFactory();
        await factory.SeedEventTypesAsync();
        var client = factory.CreateClient();

        var plannerAuth = await LoginAsync(client, "+989121111119");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var plannerProfileResponse = await client.PutAsJsonAsync("/api/event-planner-profile/me", new
        {
            Title = "Manual Transfer Planner",
            PictureUrl = "https://example.com/manual.jpg",
            Resume = "Planner for direct payment tests."
        });
        Assert.Equal(HttpStatusCode.OK, plannerProfileResponse.StatusCode);

        plannerAuth = await LoginAsync(client, "+989121111119");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var createEventResponse = await client.PostAsJsonAsync(
            "/api/dating-events",
            CreateEventBody(
                paymentCollectionMethod: EventPaymentCollectionMethod.OrganizerManualTransfer,
                organizerPaymentInstructions: "Card number 1234-5678-9012-3456"));
        var createEventBody = await createEventResponse.Content.ReadAsStringAsync();
        Assert.True(createEventResponse.StatusCode == HttpStatusCode.Created, $"Create event failed with {(int)createEventResponse.StatusCode}: {createEventBody}");
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();
        Assert.NotNull(createdEvent);
        Assert.Equal(EventPaymentCollectionMethod.OrganizerManualTransfer, createdEvent.PaymentCollectionMethod);

        await factory.ApproveEventAsync(createdEvent.Id);
        var openResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/open", null);
        Assert.Equal(HttpStatusCode.NoContent, openResponse.StatusCode);

        var buyer = await CreateUserWithProfileAsync(factory, client, "+989126999999", "DirectPaymentBuyer", Gender.Male, EducationLevel.Graduated);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", buyer.Token);

        var buyResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/tickets", null);
        Assert.Equal(HttpStatusCode.Created, buyResponse.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);
        var plannerBalance = await client.GetFromJsonAsync<BalanceDto>("/api/balances/me");

        Assert.NotNull(plannerBalance);
        Assert.Equal(-10m, plannerBalance.Balance);
    }

    [Fact]
    public async Task PlannerSmsRequestRequiresAdminApprovalBeforeQueueingMessages()
    {
        await using var factory = new RandevooEventApiFactory();
        await factory.SeedEventTypesAsync();
        var client = factory.CreateClient();

        var plannerAuth = await LoginAsync(client, "+989121111111");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var plannerProfileResponse = await client.PutAsJsonAsync("/api/event-planner-profile/me", new
        {
            Title = "SMS Approval Planner",
            PictureUrl = "https://example.com/p.jpg",
            Resume = "Planner used for SMS approval tests."
        });
        Assert.Equal(HttpStatusCode.OK, plannerProfileResponse.StatusCode);

        plannerAuth = await LoginAsync(client, "+989121111111");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);

        var createEventResponse = await client.PostAsJsonAsync("/api/dating-events", CreateEventBody());
        var createEventBody = await createEventResponse.Content.ReadAsStringAsync();
        Assert.True(createEventResponse.StatusCode == HttpStatusCode.Created, $"Create event failed with {(int)createEventResponse.StatusCode}: {createEventBody}");
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();
        Assert.NotNull(createdEvent);

        await factory.ApproveEventAsync(createdEvent.Id);
        var openResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/open", null);
        Assert.Equal(HttpStatusCode.NoContent, openResponse.StatusCode);

        await CreateFundedProfileAndTicketAsync(factory, client, "+989126000001", createdEvent.Id, "SmsParticipant", Gender.Male);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plannerAuth.Token);
        var smsRequestResponse = await client.PostAsJsonAsync($"/api/dating-events/{createdEvent.Id}/send-sms", new
        {
            Message = "Please arrive 20 minutes early for check-in."
        });
        Assert.Equal(HttpStatusCode.Accepted, smsRequestResponse.StatusCode);

        var responseJson = JsonDocument.Parse(await smsRequestResponse.Content.ReadAsStringAsync());
        var requestId = responseJson.RootElement.GetProperty("requestId").GetInt64();

        var pendingRequest = await factory.GetSmsRequestAsync(requestId);
        Assert.NotNull(pendingRequest);
        Assert.Equal(EventParticipantSmsRequestStatus.Pending, pendingRequest.Status);
        Assert.Equal(0, await factory.GetSmsQueueCountForRequestAsync(requestId));

        var adminUserId = await factory.SeedAdminAsync();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CreateToken(adminUserId, "+989120000000", UserRole.Admin));
        var approveResponse = await client.PostAsJsonAsync($"/api/dating-events/sms-requests/{requestId}/approve", new
        {
            ApprovedMessage = "Please arrive 20 minutes early for check-in.",
            Note = "Approved for delivery."
        });
        var approveBody = await approveResponse.Content.ReadAsStringAsync();
        Assert.True(approveResponse.StatusCode == HttpStatusCode.OK, $"Approve failed with {(int)approveResponse.StatusCode}: {approveBody}");

        var approvedRequest = await factory.GetSmsRequestAsync(requestId);
        Assert.NotNull(approvedRequest);
        Assert.Equal(EventParticipantSmsRequestStatus.Approved, approvedRequest.Status);
        Assert.Equal("Approved for delivery.", approvedRequest.ReviewNote);
        Assert.Equal(1, approvedRequest.QueuedRecipientsCount);

        var queueItems = await factory.GetSmsQueueItemsForRequestAsync(requestId);
        var queueItem = Assert.Single(queueItems);
        Assert.Equal(SmsQueueItemStatus.Pending, queueItem.Status);
        Assert.Equal("+989126000001", queueItem.MobileNumber);
        Assert.Equal("Please arrive 20 minutes early for check-in.", queueItem.Message);
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
        var createEventResponse = await client.PostAsJsonAsync("/api/dating-events", CreateCompletableEventBody());
        var createEventBody = await createEventResponse.Content.ReadAsStringAsync();
        Assert.True(createEventResponse.StatusCode == HttpStatusCode.Created, $"Create event failed with {(int)createEventResponse.StatusCode}: {createEventBody}");
        var createdEvent = await createEventResponse.Content.ReadFromJsonAsync<DatingEventDto>();
        Assert.NotNull(createdEvent);

        await factory.ApproveEventAsync(createdEvent.Id);
        var openResponse = await client.PostAsync($"/api/dating-events/{createdEvent.Id}/open", null);
        Assert.Equal(HttpStatusCode.NoContent, openResponse.StatusCode);

        var firstUser = await CreateFundedProfileAndTicketAsync(factory, client, "+989125000001", createdEvent.Id, "ParticipantOne", Gender.Male);
        var secondUser = await CreateFundedProfileAndTicketAsync(factory, client, "+989125000002", createdEvent.Id, "ParticipantTwo", Gender.Female);
        var thirdUser = await CreateFundedProfileAndTicketAsync(factory, client, "+989125000003", createdEvent.Id, "ParticipantThree", Gender.Male);
        await factory.MarkEventEndedAsync(createdEvent.Id);

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
        Assert.Equal(HttpStatusCode.Accepted, startConversationResponse.StatusCode);
        var pendingLike = await startConversationResponse.Content.ReadFromJsonAsync<EventLikeResultDto>();
        Assert.NotNull(pendingLike);
        Assert.Equal(EventLikeStatus.Pending, pendingLike.Status);
        Assert.Null(pendingLike.Conversation);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", secondUser.Token);
        var matchConversationResponse = await client.PostAsJsonAsync($"/api/event-chats/events/{createdEvent.Id}/conversations", new
        {
            ParticipantUserId = firstUser.UserId
        });
        Assert.Equal(HttpStatusCode.Created, matchConversationResponse.StatusCode);
        var matchedLike = await matchConversationResponse.Content.ReadFromJsonAsync<EventLikeResultDto>();
        Assert.NotNull(matchedLike);
        Assert.Equal(EventLikeStatus.Matched, matchedLike.Status);
        var conversation = matchedLike.Conversation;
        Assert.NotNull(conversation);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", firstUser.Token);
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

    private static object CreateEventBody(
        EventEducationLevelRestriction educationLevelRestriction = EventEducationLevelRestriction.WithoutLimit,
        string maleTicketCurrencyCode = "IRR",
        string femaleTicketCurrencyCode = "IRR",
        EventPaymentCollectionMethod paymentCollectionMethod = EventPaymentCollectionMethod.PlatformGateway,
        string? organizerPaymentInstructions = null) => new
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
        NumberOfLikesAllowed = 3,
        MaleTicketPrice = 100m,
        MaleTicketCurrencyCode = maleTicketCurrencyCode,
        FemaleTicketPrice = 100m,
        FemaleTicketCurrencyCode = femaleTicketCurrencyCode,
        EducationLevelRestriction = educationLevelRestriction,
        Tags = new[] { "Mafia", "Night", "Friendly" },
        EventImage1 = "https://example.com/1.jpg",
        EventImage2 = "https://example.com/2.jpg",
        EventImage3 = "https://example.com/3.jpg",
        EventDescriptionHtml = "<p>A friendly mafia game night.</p>",
        EventPlannerCommissionPercent = 10m,
        PaymentCollectionMethod = paymentCollectionMethod,
        OrganizerPaymentInstructions = organizerPaymentInstructions
    };

    private static object CreateCompletableEventBody() => new
    {
        Title = "Completed Social Night",
        Country = "Iran",
        City = "Tehran",
        Region = "District 2",
        Latitude = 35.6895m,
        Longitude = 51.3890m,
        Address = "Past event address",
        DateTimeStart = DateTime.UtcNow.AddDays(3),
        DateTimeEnd = DateTime.UtcNow.AddDays(3).AddHours(3),
        EventTypeId = 3L,
        MaleMinAge = 18,
        MaleMaxAge = 45,
        FemaleMinAge = 18,
        FemaleMaxAge = 45,
        MaleCapacity = 10,
        FemaleCapacity = 10,
        NumberOfLikesAllowed = 1,
        MaleTicketPrice = 100m,
        MaleTicketCurrencyCode = "IRR",
        FemaleTicketPrice = 100m,
        FemaleTicketCurrencyCode = "IRR",
        EducationLevelRestriction = EventEducationLevelRestriction.WithoutLimit,
        Tags = new[] { "Social", "Completed" },
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
        Gender gender,
        EducationLevel educationLevel = EducationLevel.NotSpecified)
    {
        var auth = await CreateUserWithProfileAsync(factory, client, mobileNumber, displayName, gender, educationLevel);

        var buyResponse = await client.PostAsync($"/api/dating-events/{eventId}/tickets", null);
        Assert.Equal(HttpStatusCode.Created, buyResponse.StatusCode);
        return auth;
    }

    private static async Task<AuthResult> CreateUserWithProfileAsync(
        RandevooEventApiFactory factory,
        HttpClient client,
        string mobileNumber,
        string displayName,
        Gender gender,
        EducationLevel educationLevel)
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
        var createdProfile = await profileResponse.Content.ReadFromJsonAsync<DatingProfileDto>();
        Assert.NotNull(createdProfile);

        if (educationLevel != EducationLevel.NotSpecified)
        {
            var updateProfileResponse = await client.PutAsJsonAsync($"/api/dating-profiles/{createdProfile.Id}", new
            {
                DisplayName = createdProfile.DisplayName,
                Gender = createdProfile.Gender,
                Country = createdProfile.Country,
                City = createdProfile.City,
                Latitude = createdProfile.Latitude,
                Longitude = createdProfile.Longitude,
                HeightCm = createdProfile.HeightCm,
                EducationLevel = educationLevel,
                Smoking = false,
                Region = createdProfile.Region
            });
            Assert.Equal(HttpStatusCode.NoContent, updateProfileResponse.StatusCode);
        }

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

        public async Task<long> SeedAdminAsync()
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            var existingAdmin = await db.Users.FirstOrDefaultAsync(u => u.MobileNumber == "+989120000000");
            if (existingAdmin is not null)
                return existingAdmin.Id;

            var admin = new User("+989120000000");
            admin.ChangeUserRole(UserRole.Admin);
            db.Users.Add(admin);
            await db.SaveChangesAsync();
            return admin.Id;
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
            await SeedCurrencyExchangeRatesAsync(db);

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

        private static async Task SeedCurrencyExchangeRatesAsync(RandevooDbContext db)
        {
            if (await db.CurrencyExchangeRates.AnyAsync())
                return;

            var effectiveFromUtc = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc);
            db.CurrencyExchangeRates.AddRange(
                new CurrencyExchangeRate("IRR", "IRR", 1m, effectiveFromUtc, "IntegrationTest"),
                new CurrencyExchangeRate("USD", "IRR", 1750000m, effectiveFromUtc, "IntegrationTest"),
                new CurrencyExchangeRate("EUR", "IRR", 2000000m, effectiveFromUtc, "IntegrationTest"),
                new CurrencyExchangeRate("CAD", "IRR", 1280000m, effectiveFromUtc, "IntegrationTest"),
                new CurrencyExchangeRate("GBP", "IRR", 2350000m, effectiveFromUtc, "IntegrationTest"),
                new CurrencyExchangeRate("AED", "IRR", 476500m, effectiveFromUtc, "IntegrationTest"),
                new CurrencyExchangeRate("TRY", "IRR", 54000m, effectiveFromUtc, "IntegrationTest"));
            await db.SaveChangesAsync();
        }

        public async Task AddDiscountCodeAsync(long eventId, string code, EventDiscountGenderScope genderScope, EventDiscountType discountType, decimal value)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            var datingEvent = await db.DatingEvents
                .Include(item => item.DiscountCodes)
                .SingleAsync(item => item.Id == eventId);

            if (datingEvent.DiscountCodes.Any(item => item.Code == code))
                return;

            datingEvent.AddDiscountCode(
                code,
                genderScope,
                discountType,
                value,
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(7),
                5,
                true,
                "Integration test discount",
                "Created from integration test.");

            await db.SaveChangesAsync();
        }

        public async Task ApproveEventAsync(long eventId)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            var datingEvent = await db.DatingEvents.SingleAsync(item => item.Id == eventId);

            datingEvent.ApproveByAdmin();
            await db.SaveChangesAsync();
        }

        public async Task MarkEventEndedAsync(long eventId)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            var datingEvent = await db.DatingEvents.SingleAsync(item => item.Id == eventId);

            datingEvent.CloseForSell();
            db.Entry(datingEvent).Property(item => item.DateTimeStart).CurrentValue = DateTime.UtcNow.AddHours(-4);
            db.Entry(datingEvent).Property(item => item.DateTimeEnd).CurrentValue = DateTime.UtcNow.AddHours(-1);
            await db.SaveChangesAsync();
        }

        public async Task<EventParticipantSmsRequest?> GetSmsRequestAsync(long requestId)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            return await db.EventParticipantSmsRequests.SingleOrDefaultAsync(request => request.Id == requestId);
        }

        public async Task<int> GetSmsQueueCountForRequestAsync(long requestId)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            return await db.SmsQueueItems.CountAsync(item => item.EventParticipantSmsRequestId == requestId);
        }

        public async Task<List<SmsQueueItem>> GetSmsQueueItemsForRequestAsync(long requestId)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
            return await db.SmsQueueItems
                .Where(item => item.EventParticipantSmsRequestId == requestId)
                .OrderBy(item => item.Id)
                .ToListAsync();
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
