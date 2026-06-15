using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Common;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;
using Randevoo.Infrastructure.Repositories;
using Xunit;

namespace Randevoo.Tests.Integration;

public class SupportTicketRepositoryTests
{
    [Fact]
    public async Task GetNextRoundRobinAssigneeAsync_AssignsActiveSupportUsersInSequence()
    {
        var options = new DbContextOptionsBuilder<RandevooDbContext>()
            .UseInMemoryDatabase($"support-round-robin-{Guid.NewGuid():N}")
            .Options;

        await using var db = new RandevooDbContext(options);
        var first = new User("+989130000001");
        first.ChangeUserRole(UserRole.PlatformSupportTeam);
        var second = new User("+989130000002");
        second.ChangeUserRole(UserRole.PlatformSupportTeam);
        var inactive = new User("+989130000003");
        inactive.ChangeUserRole(UserRole.PlatformSupportTeam);
        inactive.Deactivate();

        db.Users.AddRange(first, second, inactive);
        await db.SaveChangesAsync();

        var repository = new SupportTicketRepository(db);

        var assignedFirst = await repository.GetNextRoundRobinAssigneeAsync();
        await db.SaveChangesAsync();
        var assignedSecond = await repository.GetNextRoundRobinAssigneeAsync();
        await db.SaveChangesAsync();
        var assignedThird = await repository.GetNextRoundRobinAssigneeAsync();

        Assert.Equal(first.Id, assignedFirst?.Id);
        Assert.Equal(second.Id, assignedSecond?.Id);
        Assert.Equal(first.Id, assignedThird?.Id);
    }

    [Fact]
    public async Task ListAsync_AppliesSupportAndPlannerRecipientScopes()
    {
        var options = new DbContextOptionsBuilder<RandevooDbContext>()
            .UseInMemoryDatabase($"support-recipient-scope-{Guid.NewGuid():N}")
            .Options;

        await using var db = new RandevooDbContext(options);
        SeedSupportTicketLookups(db);
        var submitter = new User("+989130000011");
        var support = new User("+989130000012");
        support.ChangeUserRole(UserRole.PlatformSupportTeam);
        var planner = new User("+989130000013");
        planner.ChangeUserRole(UserRole.EventPlanner);
        var otherPlanner = new User("+989130000014");
        otherPlanner.ChangeUserRole(UserRole.EventPlanner);

        db.Users.AddRange(submitter, support, planner, otherPlanner);
        await db.SaveChangesAsync();

        var eventType = new EventType("Social");
        var datingEvent = CreateEvent(planner, eventType);
        db.EventTypes.Add(eventType);
        db.DatingEvents.Add(datingEvent);
        await db.SaveChangesAsync();

        var platformTicket = new SupportTicket(
            submitter,
            "Platform issue",
            SupportTicketLookupIds.TypeFinancialProblem,
            SupportTicketLookupIds.RecipientPlatformSupport,
            new SupportTicketMessage(submitter, "Payment issue."),
            support,
            null,
            null);
        var organizerTicket = new SupportTicket(
            submitter,
            "Organizer question",
            SupportTicketLookupIds.TypePrePurchaseQuestion,
            SupportTicketLookupIds.RecipientEventPlanner,
            new SupportTicketMessage(submitter, "Can I arrive late?"),
            null,
            datingEvent,
            planner);
        db.SupportTickets.AddRange(platformTicket, organizerTicket);
        await db.SaveChangesAsync();

        var repository = new SupportTicketRepository(db);

        var supportTickets = await repository.ListAsync(support.Id, UserRole.PlatformSupportTeam);
        var plannerTickets = await repository.ListAsync(planner.Id, UserRole.EventPlanner);
        var otherPlannerTickets = await repository.ListAsync(otherPlanner.Id, UserRole.EventPlanner);

        Assert.Contains(supportTickets, ticket => ticket.Id == platformTicket.Id);
        Assert.DoesNotContain(supportTickets, ticket => ticket.Id == organizerTicket.Id);
        Assert.Contains(plannerTickets, ticket => ticket.Id == organizerTicket.Id);
        Assert.DoesNotContain(plannerTickets, ticket => ticket.Id == platformTicket.Id);
        Assert.Empty(otherPlannerTickets);
    }

    private static DatingEvent CreateEvent(User planner, EventType eventType)
    {
        return new DatingEvent(
            planner,
            "Planner event",
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            "Main venue",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(2),
            eventType,
            new AgeRange(18, 45),
            new AgeRange(18, 45),
            10,
            10,
            3,
            100m,
            100m,
            EventEducationLevelRestriction.WithoutLimit,
            null,
            null,
            null,
            null,
            "<p>Test event description.</p>");
    }

    private static void SeedSupportTicketLookups(RandevooDbContext db)
    {
        db.SupportTicketCategories.AddRange(
            CreateLookup<SupportTicketCategoryLookup>(SupportTicketLookupIds.TypeFinancialProblem, "financial-problem", "مشکل مالی", 1),
            CreateLookup<SupportTicketCategoryLookup>(SupportTicketLookupIds.TypePrePurchaseQuestion, "pre-purchase-question", "سوال پیش از خرید", 5));
        db.SupportTicketStatuses.Add(CreateLookup<SupportTicketStatusLookup>(SupportTicketLookupIds.StatusOpen, "open", "باز", 1));
        db.SupportTicketRecipientTypes.AddRange(
            CreateLookup<SupportTicketRecipientTypeLookup>(SupportTicketLookupIds.RecipientPlatformSupport, "platform-support", "پشتیبانی سایت", 1),
            CreateLookup<SupportTicketRecipientTypeLookup>(SupportTicketLookupIds.RecipientEventPlanner, "event-planner", "برگزارکننده رویداد", 2));
    }

    private static TLookup CreateLookup<TLookup>(long id, string name, string displayNameFa, int displayOrder)
        where TLookup : BaseEntity
    {
        var lookup = (TLookup)Activator.CreateInstance(typeof(TLookup), nonPublic: true)!;
        SetProperty(lookup, nameof(BaseEntity.Id), id);
        SetProperty(lookup, "Name", name);
        SetProperty(lookup, "DisplayNameFa", displayNameFa);
        SetProperty(lookup, "IsActive", true);
        SetProperty(lookup, "DisplayOrder", displayOrder);
        return lookup;
    }

    private static void SetProperty(object target, string propertyName, object value)
    {
        var property = target.GetType().GetProperty(propertyName)
            ?? typeof(BaseEntity).GetProperty(propertyName)
            ?? throw new InvalidOperationException($"Property {propertyName} was not found.");
        property.SetValue(target, value);
    }
}
