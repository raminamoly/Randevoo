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
        var datingEvent = CreateEvent(
            "Restricted event",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(3),
            educationRestriction: EventEducationLevelRestriction.BachelorOrHigher);
        datingEvent.ApproveByAdmin();
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

    [Fact]
    public void SellTicket_UsesGenderSpecificPrice()
    {
        var datingEvent = CreateEvent(
            "Pricing event",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(3),
            maleTicketPrice: 200m,
            femaleTicketPrice: 120m);
        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();

        var maleBuyer = new User("+989122000001");
        var maleProfile = new UserProfile(
            maleBuyer,
            "Male buyer",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-28),
            Gender.Male,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(180));

        var femaleBuyer = new User("+989122000002");
        var femaleProfile = new UserProfile(
            femaleBuyer,
            "Female buyer",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-27),
            Gender.Female,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(168));

        var maleTicket = datingEvent.SellTicket(maleBuyer, maleProfile);
        var femaleTicket = datingEvent.SellTicket(femaleBuyer, femaleProfile);

        Assert.Equal(200m, maleTicket.Price);
        Assert.Equal(120m, femaleTicket.Price);
    }

    [Fact]
    public void NewEvent_StartsAsNotSubmittedAndDraft()
    {
        var datingEvent = CreateEvent("New event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));

        Assert.Equal(EventReviewStatus.NotSubmitted, datingEvent.ReviewStatus);
        Assert.Equal(EventOperationalStatus.Draft, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    [Fact]
    public void SubmitForReview_SetsReviewStatusToPendingReviewAndClosesSale()
    {
        var datingEvent = CreateEvent("Submitted event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));
        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();

        datingEvent.SubmitForReview();

        Assert.Equal(EventReviewStatus.PendingReview, datingEvent.ReviewStatus);
        Assert.False(datingEvent.IsOpenForSell);
        Assert.Equal(EventOperationalStatus.Draft, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    [Fact]
    public void ApproveByAdmin_SetsReviewStatusToApprovedWithoutOpeningSale()
    {
        var datingEvent = CreateEvent("Approved event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));
        datingEvent.SubmitForReview();

        datingEvent.ApproveByAdmin();

        Assert.Equal(EventReviewStatus.Approved, datingEvent.ReviewStatus);
        Assert.False(datingEvent.IsOpenForSell);
        Assert.Equal(EventOperationalStatus.Draft, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    [Fact]
    public void RejectByAdmin_SetsReviewStatusToRejectedAndClosesSale()
    {
        var datingEvent = CreateEvent("Rejected event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));
        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();

        datingEvent.RejectByAdmin();

        Assert.Equal(EventReviewStatus.Rejected, datingEvent.ReviewStatus);
        Assert.False(datingEvent.IsOpenForSell);
        Assert.Equal(EventOperationalStatus.Draft, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    [Fact]
    public void OpenForSell_RequiresApprovedReviewStatus()
    {
        var datingEvent = CreateEvent("Unapproved event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));
        datingEvent.SubmitForReview();

        Assert.Throws<BusinessRuleViolationException>(() => datingEvent.OpenForSell());
    }

    [Fact]
    public void Cancelled_HasOperationalPriority()
    {
        var datingEvent = CreateEvent("Cancelled event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));
        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();

        datingEvent.Cancel();

        Assert.Equal(EventOperationalStatus.Cancelled, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    [Fact]
    public void PastEvent_ResolvesToClosedWhenNotCancelled()
    {
        var datingEvent = CreateEvent("Past event", DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-2).AddHours(2));
        datingEvent.ApproveByAdmin();

        Assert.Equal(EventOperationalStatus.Closed, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    private static DatingEvent CreateEvent(
        string title,
        DateTime startAtUtc,
        DateTime endAtUtc,
        EventEducationLevelRestriction educationRestriction = EventEducationLevelRestriction.WithoutLimit,
        decimal maleTicketPrice = 100m,
        decimal femaleTicketPrice = 100m)
    {
        var planner = new User($"+989121{Math.Abs(title.GetHashCode()) % 1000000:000000}");
        planner.ChangeUserRole(UserRole.EventPlanner);
        var eventType = new EventType("Social");

        return new DatingEvent(
            planner,
            title,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            "Main venue",
            startAtUtc,
            endAtUtc,
            eventType,
            new AgeRange(18, 45),
            new AgeRange(18, 45),
            10,
            10,
            3,
            maleTicketPrice,
            femaleTicketPrice,
            educationRestriction,
            null,
            null,
            null,
            null,
            "<p>Test event description.</p>");
    }
}
