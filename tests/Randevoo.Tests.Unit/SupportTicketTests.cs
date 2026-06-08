using FluentAssertions;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
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
}
