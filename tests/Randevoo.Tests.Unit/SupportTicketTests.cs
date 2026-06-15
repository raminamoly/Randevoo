using FluentAssertions;
using Randevoo.Domain.Common;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;
using Xunit;

namespace Randevoo.Tests.Unit;

public class SupportTicketTests
{
    [Fact]
    public void Constructor_WithEndUserSubmitter_CreatesOpenTicketWithInitialMessageAndAssignmentHistory()
    {
        var submitter = new User("+989120000001");
        var support = new User("+989120000002");
        support.ChangeUserRole(UserRole.PlatformSupportTeam);
        var attachment = new SupportTicketAttachment("receipt.png", "image/png", 2048, "/uploads/support/receipt.png");
        var message = new SupportTicketMessage(submitter, "Payment failed after gateway callback.", new[] { attachment });

        var ticket = new SupportTicket(submitter, "Payment problem", SupportTicketCategory.FinancialProblem, message, support);

        ticket.Status.Should().Be(SupportTicketStatus.Open);
        ticket.SubmitterRole.Should().Be(UserRole.EndUser);
        ticket.AssignedSupportUser.Should().Be(support);
        ticket.Messages.Should().ContainSingle();
        ticket.Messages.Single().Attachments.Should().ContainSingle();
        ticket.History.Select(item => item.Action).Should().Contain(new[] { "TicketCreated", "TicketAssigned" });
    }

    [Fact]
    public void Constructor_WithLookupIds_StoresTypeStatusAndRecipientIds()
    {
        var submitter = CreateUser("+989120000021", UserRole.EndUser, 21);
        var support = CreateUser("+989120000022", UserRole.PlatformSupportTeam, 22);
        var message = new SupportTicketMessage(submitter, "Ticket problem body.");

        var ticket = new SupportTicket(
            submitter,
            "Ticket problem",
            SupportTicketLookupIds.TypeTicketProblem,
            SupportTicketLookupIds.RecipientPlatformSupport,
            message,
            support,
            null,
            null);

        ticket.TicketTypeId.Should().Be(SupportTicketLookupIds.TypeTicketProblem);
        ticket.TicketStatusId.Should().Be(SupportTicketLookupIds.StatusOpen);
        ticket.TicketRecipientTypeId.Should().Be(SupportTicketLookupIds.RecipientPlatformSupport);
        ticket.Category.Should().Be(SupportTicketCategory.FinancialProblem);
        ticket.Status.Should().Be(SupportTicketStatus.Open);
    }

    [Fact]
    public void Constructor_WithPlannerRecipient_RoutesTicketToEventPlanner()
    {
        var submitter = CreateUser("+989120000031", UserRole.EndUser, 31);
        var planner = CreateUser("+989120000032", UserRole.EventPlanner, 32);
        var otherPlanner = CreateUser("+989120000033", UserRole.EventPlanner, 33);
        var support = CreateUser("+989120000034", UserRole.PlatformSupportTeam, 34);
        var datingEvent = CreateEvent(planner);
        var message = new SupportTicketMessage(submitter, "Can I arrive late?");

        var ticket = new SupportTicket(
            submitter,
            "Question for organizer",
            SupportTicketLookupIds.TypePrePurchaseQuestion,
            SupportTicketLookupIds.RecipientEventPlanner,
            message,
            null,
            datingEvent,
            planner);

        ticket.AssignedSupportUserId.Should().BeNull();
        ticket.RecipientPlannerUserId.Should().Be(planner.Id);
        ticket.DatingEventId.Should().Be(datingEvent.Id);
        ticket.CanBeViewedBy(planner).Should().BeTrue();
        ticket.CanBeViewedBy(otherPlanner).Should().BeFalse();
        ticket.CanBeViewedBy(support).Should().BeFalse();
        ticket.History.Select(item => item.Action).Should().Contain("TicketSentToPlanner");
    }

    [Fact]
    public void Constructor_WithSupportSubmitter_ThrowsBusinessRuleViolationException()
    {
        var support = new User("+989120000003");
        support.ChangeUserRole(UserRole.PlatformSupportTeam);
        var message = new SupportTicketMessage(support, "I should not create a submitter ticket.");

        Action act = () => new SupportTicket(support, "Invalid", SupportTicketCategory.GeneralQuestion, message, null);

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void ChangeStatus_WithSupportUser_CanCloseAndReopenTicket()
    {
        var submitter = new User("+989120000004");
        var support = new User("+989120000005");
        support.ChangeUserRole(UserRole.PlatformSupportTeam);
        var ticket = new SupportTicket(submitter, "Event issue", SupportTicketCategory.EventProblem, new SupportTicketMessage(submitter, "Event location is wrong."), support);

        ticket.ChangeStatus(support, SupportTicketStatus.Closed, "Resolved");
        ticket.ChangeStatus(support, SupportTicketStatus.Reopened, "User replied again");

        ticket.Status.Should().Be(SupportTicketStatus.Reopened);
        ticket.ClosedAtUtc.Should().BeNull();
        ticket.History.Select(item => item.Action).Should().Contain(new[] { "TicketClosed", "TicketReopened" });
    }

    [Fact]
    public void ChangeStatus_WithLookupStatusId_UpdatesLegacyStatusAndStatusId()
    {
        var submitter = CreateUser("+989120000041", UserRole.EndUser, 41);
        var support = CreateUser("+989120000042", UserRole.PlatformSupportTeam, 42);
        var ticket = new SupportTicket(submitter, "Question", SupportTicketCategory.GeneralQuestion, new SupportTicketMessage(submitter, "How does this work?"), support);

        ticket.ChangeStatus(support, SupportTicketLookupIds.StatusWaitingForUser, "Need more info");

        ticket.TicketStatusId.Should().Be(SupportTicketLookupIds.StatusWaitingForUser);
        ticket.Status.Should().Be(SupportTicketStatus.WaitingForUser);
    }

    [Fact]
    public void ChangeStatus_WithSubmitter_ThrowsBusinessRuleViolationException()
    {
        var submitter = new User("+989120000006");
        var ticket = new SupportTicket(submitter, "Question", SupportTicketCategory.GeneralQuestion, new SupportTicketMessage(submitter, "How does this work?"), null);

        Action act = () => ticket.ChangeStatus(submitter, SupportTicketStatus.Closed);

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Attachment_WithNonImageContentType_ThrowsBusinessRuleViolationException()
    {
        Action act = () => new SupportTicketAttachment("notes.pdf", "application/pdf", 1024, "/uploads/support/notes.pdf");

        act.Should().Throw<BusinessRuleViolationException>();
    }

    private static User CreateUser(string mobile, UserRole role, long id)
    {
        var user = new User(mobile);
        if (role != UserRole.EndUser)
        {
            user.ChangeUserRole(role);
        }

        SetId(user, id);
        return user;
    }

    private static DatingEvent CreateEvent(User planner)
    {
        var eventType = new EventType("Social");
        SetId(eventType, 51);
        var datingEvent = new DatingEvent(
            planner,
            "Organizer event",
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
        SetId(datingEvent, 61);
        return datingEvent;
    }

    private static void SetId(BaseEntity entity, long id)
    {
        typeof(BaseEntity).GetProperty(nameof(BaseEntity.Id))!.SetValue(entity, id);
    }
}
