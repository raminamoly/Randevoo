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
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;
using Xunit;

namespace Randevoo.Tests.Integration.EndUsers;

public sealed class EndUserTicketCheckoutEndpointsTests
{
    [Fact]
    public async Task PlatformGateway_CreatesPaidOrderAndValidTicket()
    {
        await using var factory = new CheckoutApiFactory();
        var seed = await factory.SeedAsync(EventPaymentCollectionMethod.PlatformGateway);
        var client = factory.CreateAuthorizedClient(seed.BuyerUserId, seed.BuyerMobile);

        var response = await client.PostAsJsonAsync($"/api/v1/platform/events/{seed.EventId}/tickets", new { });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
        Assert.NotNull(result);
        Assert.Equal((int)TicketOrderPaymentStatus.Paid, result.PaymentStatus);
        Assert.NotEqual(0, result.TicketId);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
        Assert.True(await db.EventTickets.AnyAsync(ticket => ticket.Id == result.TicketId && ticket.UserId == seed.BuyerUserId));
        Assert.True(await db.OnlinePayments.AnyAsync(payment => payment.TicketOrderId == result.OrderId && payment.Status == OnlinePaymentStatus.Succeeded));
    }

    [Theory]
    [InlineData(EventPaymentCollectionMethod.PlatformManualTransfer)]
    [InlineData(EventPaymentCollectionMethod.OrganizerManualTransfer)]
    public async Task ManualTransfer_CreatesPendingOrderAndReceiptWithoutTicket(EventPaymentCollectionMethod method)
    {
        await using var factory = new CheckoutApiFactory();
        var seed = await factory.SeedAsync(method);
        var client = factory.CreateAuthorizedClient(seed.BuyerUserId, seed.BuyerMobile);

        var response = await client.PostAsJsonAsync($"/api/v1/platform/events/{seed.EventId}/tickets", new
        {
            ManualReceiptFilePath = "/uploads/manual-receipts/test.jpg",
            ManualReceiptTrackingNumber = "TRK-100"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
        Assert.NotNull(result);
        Assert.Equal((int)TicketOrderPaymentStatus.Pending, result.PaymentStatus);
        Assert.Equal(0, result.TicketId);
        Assert.NotNull(result.ManualPaymentReceiptId);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
        Assert.False(await db.EventTickets.AnyAsync(ticket => ticket.DatingEventId == seed.EventId && ticket.UserId == seed.BuyerUserId));
        Assert.True(await db.ManualPaymentReceipts.AnyAsync(receipt => receipt.Id == result.ManualPaymentReceiptId && receipt.TicketOrderId == result.OrderId));
    }

    [Fact]
    public async Task DiscountPreview_AppliesValidGenderScopedDiscountAndRejectsInvalidGender()
    {
        await using var factory = new CheckoutApiFactory();
        var seed = await factory.SeedAsync(EventPaymentCollectionMethod.PlatformGateway);
        var client = factory.CreateAuthorizedClient(seed.BuyerUserId, seed.BuyerMobile);

        var valid = await client.PostAsJsonAsync($"/api/v1/platform/events/{seed.EventId}/checkout/preview", new { DiscountCode = "MEN15" });
        var invalid = await client.PostAsJsonAsync($"/api/v1/platform/events/{seed.EventId}/checkout/preview", new { DiscountCode = "WOMEN15" });

        Assert.Equal(HttpStatusCode.OK, valid.StatusCode);
        var preview = await valid.Content.ReadFromJsonAsync<CheckoutPreview>();
        Assert.NotNull(preview);
        Assert.Equal(15m, preview.DiscountAmount);
        Assert.Equal(85m, preview.NetAmount);
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    [Fact]
    public async Task Capacity_PreventsSecondTicketForFullGenderCapacity()
    {
        await using var factory = new CheckoutApiFactory();
        var seed = await factory.SeedAsync(EventPaymentCollectionMethod.PlatformGateway, maleCapacity: 1);
        var client = factory.CreateAuthorizedClient(seed.BuyerUserId, seed.BuyerMobile);
        var otherClient = factory.CreateAuthorizedClient(seed.OtherMaleUserId, seed.OtherMaleMobile);

        var first = await client.PostAsJsonAsync($"/api/v1/platform/events/{seed.EventId}/tickets", new { });
        var second = await otherClient.PostAsJsonAsync($"/api/v1/platform/events/{seed.EventId}/tickets", new { });

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task BuyerCanPurchaseForDifferentExistingParticipant()
    {
        await using var factory = new CheckoutApiFactory();
        var seed = await factory.SeedAsync(EventPaymentCollectionMethod.PlatformGateway);
        var client = factory.CreateAuthorizedClient(seed.BuyerUserId, seed.BuyerMobile);

        var response = await client.PostAsJsonAsync($"/api/v1/platform/events/{seed.EventId}/tickets", new
        {
            ParticipantMobileNumber = seed.FemaleParticipantMobile,
            DiscountCode = "WOMEN15"
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CheckoutResult>();
        Assert.NotNull(result);
        Assert.Equal(seed.FemaleParticipantUserId, result.ParticipantUserId);

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();
        var order = await db.TicketOrders.SingleAsync(order => order.Id == result.OrderId);
        var ticket = await db.EventTickets.SingleAsync(ticket => ticket.Id == result.TicketId);
        Assert.Equal(seed.BuyerUserId, order.BuyerUserId);
        Assert.Equal(seed.FemaleParticipantUserId, ticket.UserId);
        Assert.Equal(18m, order.DiscountAmount);
    }

    private sealed record CheckoutResult(
        long OrderId,
        long TicketId,
        IReadOnlyList<long> TicketIds,
        int PaymentCollectionMethod,
        int PaymentStatus,
        int OrderStatus,
        long? ManualPaymentReceiptId,
        long? OnlinePaymentId,
        long? ParticipantUserId,
        decimal GrossAmount,
        decimal DiscountAmount,
        decimal NetAmount,
        string CurrencyCode);

    private sealed record CheckoutPreview(decimal GrossAmount, decimal DiscountAmount, decimal NetAmount);

    private sealed record SeedResult(
        long BuyerUserId,
        string BuyerMobile,
        long OtherMaleUserId,
        string OtherMaleMobile,
        long FemaleParticipantUserId,
        string FemaleParticipantMobile,
        long EventId);

    private sealed class CheckoutApiFactory : WebApplicationFactory<Program>
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

        public HttpClient CreateAuthorizedClient(long userId, string mobileNumber)
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                CreateToken(userId, mobileNumber, UserRole.EndUser));
            return client;
        }

        public async Task<SeedResult> SeedAsync(EventPaymentCollectionMethod paymentMethod, int maleCapacity = 10)
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();

            db.CurrencyExchangeRates.Add(new CurrencyExchangeRate("IRR", "IRR", 1m, DateTime.UtcNow.AddDays(-1), "Test"));

            var buyer = CreateUser("+989199100001", "Buyer", Gender.Male);
            var otherMale = CreateUser("+989199100002", "Other Male", Gender.Male);
            var female = CreateUser("+989199100003", "Female Participant", Gender.Female);
            var planner = new User("+989199100004");
            planner.ChangeUserRole(UserRole.EventPlanner);
            var plannerProfile = new EventPlannerProfile(planner, "Checkout Organizer", null, "Organizer profile created for checkout tests.");
            var eventType = new EventType($"Checkout Event Type {Guid.NewGuid():N}");
            var datingEvent = new DatingEvent(
                planner,
                $"Checkout event {Guid.NewGuid():N}",
                new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
                "Checkout venue",
                DateTime.UtcNow.AddDays(7),
                DateTime.UtcNow.AddDays(7).AddHours(2),
                eventType,
                new AgeRange(20, 50),
                new AgeRange(20, 50),
                maleCapacity,
                10,
                3,
                100m,
                120m,
                EventEducationLevelRestriction.WithoutLimit,
                null,
                "https://example.com/checkout-event.jpg",
                null,
                null,
                "<p>Checkout integration test event.</p>",
                10,
                "IRR",
                "IRR",
                paymentMethod,
                paymentMethod == EventPaymentCollectionMethod.OrganizerManualTransfer ? "Transfer to organizer test account." : null);
            datingEvent.AddDiscountCode("MEN15", EventDiscountGenderScope.Male, EventDiscountType.Percentage, 15, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30), 100, true);
            datingEvent.AddDiscountCode("WOMEN15", EventDiscountGenderScope.Female, EventDiscountType.Percentage, 15, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(30), 100, true);
            datingEvent.AddDiscountCode("EXPIRED10", EventDiscountGenderScope.All, EventDiscountType.Percentage, 10, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-1), 100, true);
            datingEvent.ApproveByAdmin();
            datingEvent.OpenForSell();

            db.Users.AddRange(buyer, otherMale, female, planner);
            db.EventPlannerProfiles.Add(plannerProfile);
            db.EventTypes.Add(eventType);
            db.DatingEvents.Add(datingEvent);
            await db.SaveChangesAsync();

            return new SeedResult(buyer.Id, buyer.MobileNumber, otherMale.Id, otherMale.MobileNumber, female.Id, female.MobileNumber, datingEvent.Id);
        }

        private static User CreateUser(string mobileNumber, string displayName, Gender gender)
        {
            var user = new User(mobileNumber);
            user.CreateProfile(
                displayName,
                new DateOnly(1992, 1, 1),
                gender,
                new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
                new Height(gender == Gender.Male ? 178 : 165));
            user.Profile!.UpdateEducationLevel(EducationLevel.Graduated);
            user.Profile.AddImage($"https://example.com/{Guid.NewGuid():N}.jpg", 1, true);
            return user;
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
