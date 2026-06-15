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
    public void SellTicket_AllowsBuyerToPurchaseForDifferentParticipant()
    {
        var datingEvent = CreateEvent(
            "Buyer participant split",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(3));
        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();

        var buyer = new User("+989122000010");
        var participant = new User("+989122000011");
        var participantProfile = new UserProfile(
            participant,
            "Actual participant",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-28),
            Gender.Male,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(180));

        var order = new TicketOrder(
            datingEvent,
            buyer,
            100m,
            0m,
            100m,
            10m,
            EventPaymentCollectionMethod.PlatformGateway,
            "IRR",
            1m,
            DateTime.UtcNow,
            null,
            null,
            TicketOrderPaymentStatus.Paid,
            TicketOrderStatus.Confirmed);

        var ticket = datingEvent.SellTicket(order, participant, participantProfile);

        Assert.Same(buyer, ticket.TicketOrder.BuyerUser);
        Assert.Same(participant, ticket.ParticipantUser);
        Assert.Contains(ticket, order.Tickets);
    }

    [Fact]
    public void NewEvent_DefaultsTicketCurrenciesToIrr()
    {
        var datingEvent = CreateEvent("Currency default", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));

        Assert.Equal("IRR", datingEvent.MaleTicketCurrencyCode);
        Assert.Equal("IRR", datingEvent.FemaleTicketCurrencyCode);
    }

    [Fact]
    public void SellTicket_StoresSharedEventCurrency()
    {
        var datingEvent = CreateEvent(
            "Currency event",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(3),
            maleTicketPrice: 20m,
            femaleTicketPrice: 25m,
            maleTicketCurrencyCode: "USD",
            femaleTicketCurrencyCode: "CAD");
        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();

        var maleBuyer = new User("+989122000003");
        var maleProfile = new UserProfile(
            maleBuyer,
            "Male currency buyer",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-28),
            Gender.Male,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(180));

        var femaleBuyer = new User("+989122000004");
        var femaleProfile = new UserProfile(
            femaleBuyer,
            "Female currency buyer",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-27),
            Gender.Female,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(168));

        var maleTicket = datingEvent.SellTicket(maleBuyer, maleProfile);
        var femaleTicket = datingEvent.SellTicket(femaleBuyer, femaleProfile);

        Assert.Equal("USD", maleTicket.CurrencyCode);
        Assert.Equal("USD", femaleTicket.CurrencyCode);
        Assert.Equal(datingEvent.MaleTicketCurrencyCode, datingEvent.FemaleTicketCurrencyCode);
    }

    [Fact]
    public void NewEvent_StoresOrganizerManualPaymentInstructions()
    {
        var datingEvent = CreateEvent(
            "Organizer payment event",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(3),
            paymentCollectionMethod: EventPaymentCollectionMethod.OrganizerManualTransfer,
            organizerPaymentInstructions: "Card number 1234-5678-9012-3456");

        Assert.Equal(EventPaymentCollectionMethod.OrganizerManualTransfer, datingEvent.PaymentCollectionMethod);
        Assert.Equal("Card number 1234-5678-9012-3456", datingEvent.OrganizerPaymentInstructions);
    }

    [Fact]
    public void NewEvent_RequiresInstructionsForOrganizerManualPayment()
    {
        Assert.Throws<BusinessRuleViolationException>(() => CreateEvent(
            "Missing payment instructions",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(3),
            paymentCollectionMethod: EventPaymentCollectionMethod.OrganizerManualTransfer,
            organizerPaymentInstructions: "   "));
    }

    [Fact]
    public void NewEvent_StartsAsNotSubmittedAndDraft()
    {
        var datingEvent = CreateEvent("New event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));

        Assert.Equal(EventReviewStatus.NotSubmitted, datingEvent.ReviewStatus);
        Assert.Equal(EventOperationalStatus.SaleClosed, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
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
        Assert.Equal(EventOperationalStatus.SaleClosed, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    [Fact]
    public void ApproveByAdmin_SetsReviewStatusToApprovedWithoutOpeningSale()
    {
        var datingEvent = CreateEvent("Approved event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));
        datingEvent.SubmitForReview();

        datingEvent.ApproveByAdmin();

        Assert.Equal(EventReviewStatus.Approved, datingEvent.ReviewStatus);
        Assert.False(datingEvent.IsOpenForSell);
        Assert.Equal(EventOperationalStatus.SaleClosed, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    [Fact]
    public void RejectByAdmin_ReturnsEventToDraftAndClosesSale()
    {
        var datingEvent = CreateEvent("Rejected event", DateTime.UtcNow.AddDays(2), DateTime.UtcNow.AddDays(2).AddHours(2));
        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();

        datingEvent.RejectByAdmin("نیاز به اصلاح زمان‌بندی دارد.");

        Assert.Equal(EventReviewStatus.NotSubmitted, datingEvent.ReviewStatus);
        Assert.Equal(EventApprovalStatus.Draft, datingEvent.ApprovalStatus);
        Assert.Equal(EventSaleStatus.Closed, datingEvent.SaleStatus);
        Assert.Equal("نیاز به اصلاح زمان‌بندی دارد.", datingEvent.AdminReviewNote);
        Assert.False(datingEvent.IsOpenForSell);
        Assert.Equal(EventOperationalStatus.SaleClosed, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
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

        Assert.Equal(EventOperationalStatus.Completed, datingEvent.ResolveOperationalStatus(DateTime.UtcNow));
    }

    private static DatingEvent CreateEvent(
        string title,
        DateTime startAtUtc,
        DateTime endAtUtc,
        EventEducationLevelRestriction educationRestriction = EventEducationLevelRestriction.WithoutLimit,
        decimal maleTicketPrice = 100m,
        decimal femaleTicketPrice = 100m,
        string maleTicketCurrencyCode = "IRR",
        string femaleTicketCurrencyCode = "IRR",
        EventPaymentCollectionMethod paymentCollectionMethod = EventPaymentCollectionMethod.PlatformGateway,
        string? organizerPaymentInstructions = null)
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
            "<p>Test event description.</p>",
            10m,
            maleTicketCurrencyCode,
            femaleTicketCurrencyCode,
            paymentCollectionMethod,
            organizerPaymentInstructions);
    }
}
