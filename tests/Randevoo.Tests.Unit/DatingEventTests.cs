using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;
using Xunit;

namespace Randevoo.Tests.Unit;

public class DatingEventTests
{
    [Fact]
    public void SellTicket_Throws_WhenBuyerEducationDoesNotMeetRestriction()
    {
        var planner = new User("+989121000000");
        planner.ChangeUserRole(UserRole.EventPlanner);
        var eventType = new EventType("Social");
        var datingEvent = new DatingEvent(
            planner,
            "Restricted event",
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            "Main venue",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(3),
            eventType,
            new AgeRange(18, 45),
            new AgeRange(18, 45),
            10,
            10,
            3,
            100m,
            EventEducationLevelRestriction.BachelorOrHigher,
            null,
            null,
            null,
            null,
            "<p>Test event description.</p>");
        datingEvent.OpenForSell();

        var buyer = new User("+989122000000");
        var profile = new UserProfile(
            buyer,
            "Buyer",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-28),
            Gender.Male,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(180));
        profile.UpdateEducationLevel(EducationLevel.Diploma);

        Assert.Throws<BusinessRuleViolationException>(() => datingEvent.SellTicket(buyer, profile));
    }
}
