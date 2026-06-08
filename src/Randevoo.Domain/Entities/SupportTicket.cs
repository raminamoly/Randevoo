using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class SupportTicket : BaseEntity, IAggregateRoot
{
    private readonly List<SupportTicketMessage> _messages = new();
    private readonly List<SupportTicketHistoryEntry> _history = new();

    public string Title { get; private set; } = null!;
    public SupportTicketCategory Category { get; private set; }
    public SupportTicketStatus Status { get; private set; }
    public long SubmitterUserId { get; private set; }
    public User SubmitterUser { get; private set; } = null!;
    public UserRole SubmitterRole { get; private set; }
    public long? AssignedSupportUserId { get; private set; }
    public User? AssignedSupportUser { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public IReadOnlyCollection<SupportTicketMessage> Messages => _messages.AsReadOnly();
    public IReadOnlyCollection<SupportTicketHistoryEntry> History => _history.AsReadOnly();

    private SupportTicket() { }

    public SupportTicket(User submitter, string title, SupportTicketCategory category, SupportTicketMessage firstMessage, User? assignedSupportUser)
    {
        if (submitter.Role is not (UserRole.EndUser or UserRole.EventPlanner))
            throw new BusinessRuleViolationException("Invalid ticket submitter", "Only users and planners can create support tickets");

        SubmitterUser = submitter;
        SubmitterUserId = submitter.Id;
        SubmitterRole = submitter.Role;
        Title = NormalizeTitle(title);
        Category = category;
        Status = SupportTicketStatus.Open;
        AssignedSupportUser = assignedSupportUser;
        AssignedSupportUserId = assignedSupportUser?.Id;
        _messages.Add(firstMessage);
        _history.Add(new SupportTicketHistoryEntry(submitter, "TicketCreated", null, Status, null, AssignedSupportUserId));
        if (assignedSupportUser is not null)
            _history.Add(new SupportTicketHistoryEntry(submitter, "TicketAssigned", null, null, null, assignedSupportUser.Id, "Round-robin assignment"));
    }

    public void AddReply(User actor, string body, IEnumerable<SupportTicketAttachment>? attachments = null, User? representedUser = null)
    {
        EnsureCanReply(actor);
        _messages.Add(new SupportTicketMessage(actor, body, attachments, representedUser));
        _history.Add(new SupportTicketHistoryEntry(actor, representedUser is null ? "ReplyAdded" : "AdminReplyOnBehalf", note: representedUser?.Id.ToString()));
        MarkAsUpdated();
    }

    public void ChangeStatus(User actor, SupportTicketStatus status, string? note = null)
    {
        EnsureSupportOrAdmin(actor);
        var oldStatus = Status;
        Status = status;
        ClosedAtUtc = status == SupportTicketStatus.Closed ? DateTime.UtcNow : null;
        _history.Add(new SupportTicketHistoryEntry(actor, status == SupportTicketStatus.Closed ? "TicketClosed" : status == SupportTicketStatus.Reopened ? "TicketReopened" : "StatusChanged", oldStatus, status, note: note));
        MarkAsUpdated();
    }

    public void Reassign(User actor, User? assignee, string? note = null)
    {
        EnsureSupportOrAdmin(actor);
        if (assignee is not null && (!assignee.IsActive || assignee.Role != UserRole.PlatformSupportTeam))
            throw new BusinessRuleViolationException("Invalid assignee", "Ticket can only be assigned to an active support team user");

        var oldAssignee = AssignedSupportUserId;
        AssignedSupportUser = assignee;
        AssignedSupportUserId = assignee?.Id;
        _history.Add(new SupportTicketHistoryEntry(actor, "TicketReassigned", oldAssigneeUserId: oldAssignee, newAssigneeUserId: AssignedSupportUserId, note: note));
        MarkAsUpdated();
    }

    public bool CanBeViewedBy(User user) =>
        user.Role is UserRole.Admin or UserRole.PlatformSupportTeam || user.Id == SubmitterUserId;

    private void EnsureCanReply(User user)
    {
        if (!CanBeViewedBy(user))
            throw new BusinessRuleViolationException("Access denied", "You cannot reply to this ticket");
    }

    private static void EnsureSupportOrAdmin(User user)
    {
        if (user.Role is not (UserRole.Admin or UserRole.PlatformSupportTeam))
            throw new BusinessRuleViolationException("Access denied", "Only support team users and admins can manage ticket status");
    }

    private static string NormalizeTitle(string title)
    {
        var normalized = title?.Trim() ?? string.Empty;
        if (normalized.Length < 3 || normalized.Length > 180)
            throw new BusinessRuleViolationException("Invalid ticket title", "Title must be between 3 and 180 characters");

        return normalized;
    }
}
