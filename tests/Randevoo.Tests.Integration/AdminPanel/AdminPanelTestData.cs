extern alias AdminPanel;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using AdminPanel::Randevoo.AdminPanel.Models.Auth;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Tests.Integration.AdminPanel;

internal static class AdminPanelTestData
{
    internal static RandevooDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<RandevooDbContext>()
            .UseInMemoryDatabase($"randevoo-admin-tests-{Guid.NewGuid():N}")
            .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var db = new RandevooDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    internal static MockUser AsMockUser(User user, AdminRole role) => new()
    {
        Id = user.Id,
        FullName = user.Profile?.DisplayName ?? user.MobileNumber,
        Mobile = user.MobileNumber,
        Role = role,
        IsActive = user.IsActive
    };

    internal static async Task<User> CreateUserAsync(RandevooDbContext db, string mobile, UserRole role, string displayName, Gender gender = Gender.Male)
    {
        var user = new User(mobile);
        user.ChangeUserRole(role);
        db.Users.Add(user);
        await db.SaveChangesAsync();

        user.CreateProfile(
            displayName,
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-28),
            gender,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(gender == Gender.Female ? 168 : 180));
        user.Profile!.UpdateEducationLevel(EducationLevel.Graduated);
        await db.SaveChangesAsync();

        return user;
    }

    internal static async Task<DatingEvent> CreateApprovedOpenEventAsync(RandevooDbContext db, User planner, string title)
    {
        var eventType = new EventType("Social dinner");
        db.EventTypes.Add(eventType);
        await db.SaveChangesAsync();

        var datingEvent = new DatingEvent(
            planner,
            title,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            "Tehran, Valiasr, sample venue",
            DateTime.UtcNow.AddDays(7),
            DateTime.UtcNow.AddDays(7).AddHours(3),
            eventType,
            new AgeRange(20, 45),
            new AgeRange(20, 45),
            20,
            20,
            5,
            100m,
            100m,
            EventEducationLevelRestriction.WithoutLimit,
            [],
            null,
            null,
            null,
            "<p>Integration test event description</p>",
            eventPlannerCommissionPercent: 10m);

        datingEvent.ApproveByAdmin(planner.Id, "Approved for integration test.");
        datingEvent.OpenForSell();
        db.DatingEvents.Add(datingEvent);
        await db.SaveChangesAsync();

        return datingEvent;
    }

    internal static async Task<EventTicket> SellTicketAsync(RandevooDbContext db, DatingEvent datingEvent, User buyer, User participant, decimal amount = 100m)
    {
        var order = new TicketOrder(
            datingEvent,
            buyer,
            amount,
            0m,
            amount,
            amount * datingEvent.EventPlannerCommissionPercent / 100m,
            EventPaymentCollectionMethod.PlatformGateway,
            "IRR",
            1m,
            DateTime.UtcNow,
            null,
            null,
            TicketOrderPaymentStatus.Paid,
            TicketOrderStatus.Confirmed);

        var ticket = datingEvent.SellTicket(order, participant, participant.Profile!, amount);
        db.TicketOrders.Add(order);
        await db.SaveChangesAsync();
        return ticket;
    }
}
