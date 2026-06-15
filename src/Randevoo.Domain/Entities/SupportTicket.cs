using Randevoo.Domain.Common;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class SupportTicket : BaseEntity, IAggregateRoot
{
    private readonly List<SupportTicketMessage> _messages = new();
    private readonly List<SupportTicketHistoryEntry> _history = new();

    public string Title { get; private set; } = null!;
    public long TicketTypeId { get; private set; }
    public SupportTicketCategoryLookup TicketType { get; private set; } = null!;
    public long TicketStatusId { get; private set; }
    public SupportTicketStatusLookup TicketStatus { get; private set; } = null!;
    public long TicketRecipientTypeId { get; private set; }
    public SupportTicketRecipientTypeLookup TicketRecipientType { get; private set; } = null!;
    public SupportTicketCategory Category { get; private set; }
    public SupportTicketStatus Status { get; private set; }
    public long SubmitterUserId { get; private set; }
    public User SubmitterUser { get; private set; } = null!;
    public UserRole SubmitterRole { get; private set; }
    public long? AssignedSupportUserId { get; private set; }
    public User? AssignedSupportUser { get; private set; }
    public long? DatingEventId { get; private set; }
    public DatingEvent? DatingEvent { get; private set; }
    public long? RecipientPlannerUserId { get; private set; }
    public User? RecipientPlannerUser { get; private set; }
    public DateTime? ClosedAtUtc { get; private set; }
    public IReadOnlyCollection<SupportTicketMessage> Messages => _messages.AsReadOnly();
    public IReadOnlyCollection<SupportTicketHistoryEntry> History => _history.AsReadOnly();

    private SupportTicket() { }

    public SupportTicket(User submitter, string title, SupportTicketCategory category, SupportTicketMessage firstMessage, User? assignedSupportUser)
        : this(
            submitter,
            title,
            SupportTicketLookupIds.FromCategory(category),
            SupportTicketLookupIds.RecipientPlatformSupport,
            firstMessage,
            assignedSupportUser,
            null,
            null)
    {
    }

    public SupportTicket(
        User submitter,
        string title,
        long ticketTypeId,
        long ticketRecipientTypeId,
        SupportTicketMessage firstMessage,
        User? assignedSupportUser,
        DatingEvent? datingEvent,
        User? recipientPlannerUser)
    {
        if (submitter.Role is not (UserRole.EndUser or UserRole.EventPlanner))
            throw new BusinessRuleViolationException("Invalid ticket submitter", "Only users and planners can create support tickets");

        SubmitterUser = submitter;
        SubmitterUserId = submitter.Id;
        SubmitterRole = submitter.Role;
        Title = NormalizeTitle(title);
        TicketTypeId = ValidateTicketTypeId(ticketTypeId);
        Category = SupportTicketLookupIds.ToCategory(TicketTypeId);
        Status = SupportTicketStatus.Open;
        TicketStatusId = SupportTicketLookupIds.StatusOpen;
        TicketRecipientTypeId = ValidateRecipientTypeId(ticketRecipientTypeId);
        ApplyRecipient(assignedSupportUser, datingEvent, recipientPlannerUser);
        _messages.Add(firstMessage);
        _history.Add(new SupportTicketHistoryEntry(submitter, "TicketCreated", null, Status, null, AssignedSupportUserId));
        if (assignedSupportUser is not null)
            _history.Add(new SupportTicketHistoryEntry(submitter, "TicketAssigned", null, null, null, assignedSupportUser.Id, "Round-robin assignment"));
        if (RecipientPlannerUserId is not null)
            _history.Add(new SupportTicketHistoryEntry(submitter, "TicketSentToPlanner", null, null, null, RecipientPlannerUserId, "Organizer recipient"));
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
        ChangeStatus(actor, SupportTicketLookupIds.FromStatus(status), note);
    }

    public void ChangeStatus(User actor, long ticketStatusId, string? note = null)
    {
        EnsureCanManageStatus(actor);
        var oldStatus = Status;
        TicketStatusId = ValidateStatusId(ticketStatusId);
        var status = SupportTicketLookupIds.ToStatus(TicketStatusId);
        Status = status;
        ClosedAtUtc = status == SupportTicketStatus.Closed ? DateTime.UtcNow : null;
        _history.Add(new SupportTicketHistoryEntry(actor, status == SupportTicketStatus.Closed ? "TicketClosed" : status == SupportTicketStatus.Reopened ? "TicketReopened" : "StatusChanged", oldStatus, status, note: note));
        MarkAsUpdated();
    }

    public void Reassign(User actor, User? assignee, string? note = null)
    {
        EnsureAdmin(actor);
        if (TicketRecipientTypeId != SupportTicketLookupIds.RecipientPlatformSupport)
            throw new BusinessRuleViolationException("Invalid reassignment", "Only platform support tickets can be reassigned");
        if (assignee is not null && (!assignee.IsActive || assignee.Role != UserRole.PlatformSupportTeam))
            throw new BusinessRuleViolationException("Invalid assignee", "Ticket can only be assigned to an active support team user");

        var oldAssignee = AssignedSupportUserId;
        AssignedSupportUser = assignee;
        AssignedSupportUserId = assignee?.Id;
        _history.Add(new SupportTicketHistoryEntry(actor, "TicketReassigned", oldAssigneeUserId: oldAssignee, newAssigneeUserId: AssignedSupportUserId, note: note));
        MarkAsUpdated();
    }

    public bool CanBeViewedBy(User user) =>
        user.Role == UserRole.Admin
        || user.Id == SubmitterUserId
        || (TicketRecipientTypeId == SupportTicketLookupIds.RecipientPlatformSupport && user.Role == UserRole.PlatformSupportTeam)
        || (TicketRecipientTypeId == SupportTicketLookupIds.RecipientEventPlanner && user.Role == UserRole.EventPlanner && user.Id == RecipientPlannerUserId);

    private void EnsureCanReply(User user)
    {
        if (!CanBeViewedBy(user))
            throw new BusinessRuleViolationException("Access denied", "You cannot reply to this ticket");
    }

    private void EnsureCanManageStatus(User user)
    {
        if (user.Role == UserRole.Admin)
            return;
        if (TicketRecipientTypeId == SupportTicketLookupIds.RecipientPlatformSupport && user.Role == UserRole.PlatformSupportTeam)
            return;
        if (TicketRecipientTypeId == SupportTicketLookupIds.RecipientEventPlanner && user.Role == UserRole.EventPlanner && user.Id == RecipientPlannerUserId)
            return;

        throw new BusinessRuleViolationException("Access denied", "You cannot manage this ticket status");
    }

    private static void EnsureAdmin(User user)
    {
        if (user.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only admin users can reassign tickets");
    }

    private void ApplyRecipient(User? assignedSupportUser, DatingEvent? datingEvent, User? recipientPlannerUser)
    {
        if (TicketRecipientTypeId == SupportTicketLookupIds.RecipientPlatformSupport)
        {
            if (assignedSupportUser is not null && (!assignedSupportUser.IsActive || assignedSupportUser.Role != UserRole.PlatformSupportTeam))
                throw new BusinessRuleViolationException("Invalid assignee", "Ticket can only be assigned to an active support team user");

            AssignedSupportUser = assignedSupportUser;
            AssignedSupportUserId = assignedSupportUser?.Id;
            DatingEvent = datingEvent;
            DatingEventId = datingEvent?.Id;
            RecipientPlannerUser = null;
            RecipientPlannerUserId = null;
            return;
        }

        if (TicketRecipientTypeId == SupportTicketLookupIds.RecipientEventPlanner)
        {
            if (datingEvent is null)
                throw new BusinessRuleViolationException("Event required", "Organizer tickets must be linked to an event");

            var resolvedPlanner = recipientPlannerUser ?? datingEvent.EventPlannerUser;
            if (resolvedPlanner is null || resolvedPlanner.Role != UserRole.EventPlanner)
                throw new BusinessRuleViolationException("Invalid planner", "Organizer ticket recipient is invalid");

            DatingEvent = datingEvent;
            DatingEventId = datingEvent.Id;
            RecipientPlannerUser = resolvedPlanner;
            RecipientPlannerUserId = resolvedPlanner.Id;
            AssignedSupportUser = null;
            AssignedSupportUserId = null;
            return;
        }

        throw new BusinessRuleViolationException("Invalid recipient", "Ticket recipient is invalid");
    }

    private static string NormalizeTitle(string title)
    {
        var normalized = title?.Trim() ?? string.Empty;
        if (normalized.Length < 3 || normalized.Length > 180)
            throw new BusinessRuleViolationException("Invalid ticket title", "Title must be between 3 and 180 characters");

        return normalized;
    }

    private static long ValidateTicketTypeId(long ticketTypeId)
    {
        if (ticketTypeId < SupportTicketLookupIds.TypeFinancialProblem || ticketTypeId > SupportTicketLookupIds.TypePrePurchaseQuestion)
            throw new BusinessRuleViolationException("Invalid ticket type", "Ticket type is invalid");

        return ticketTypeId;
    }

    private static long ValidateStatusId(long ticketStatusId)
    {
        if (ticketStatusId < SupportTicketLookupIds.StatusOpen || ticketStatusId > SupportTicketLookupIds.StatusReopened)
            throw new BusinessRuleViolationException("Invalid ticket status", "Ticket status is invalid");

        return ticketStatusId;
    }

    private static long ValidateRecipientTypeId(long ticketRecipientTypeId)
    {
        if (ticketRecipientTypeId is not (SupportTicketLookupIds.RecipientPlatformSupport or SupportTicketLookupIds.RecipientEventPlanner))
            throw new BusinessRuleViolationException("Invalid ticket recipient", "Ticket recipient is invalid");

        return ticketRecipientTypeId;
    }
}
