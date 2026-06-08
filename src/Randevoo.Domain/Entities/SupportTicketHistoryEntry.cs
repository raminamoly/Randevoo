using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Entities;

public class SupportTicketHistoryEntry : BaseEntity
{
    public long SupportTicketId { get; private set; }
    public SupportTicket SupportTicket { get; private set; } = null!;
    public long ActorUserId { get; private set; }
    public User ActorUser { get; private set; } = null!;
    public string Action { get; private set; } = null!;
    public SupportTicketStatus? OldStatus { get; private set; }
    public SupportTicketStatus? NewStatus { get; private set; }
    public long? OldAssigneeUserId { get; private set; }
    public long? NewAssigneeUserId { get; private set; }
    public string? Note { get; private set; }

    private SupportTicketHistoryEntry() { }

    public SupportTicketHistoryEntry(User actor, string action, SupportTicketStatus? oldStatus = null, SupportTicketStatus? newStatus = null, long? oldAssigneeUserId = null, long? newAssigneeUserId = null, string? note = null)
    {
        ActorUser = actor;
        ActorUserId = actor.Id;
        Action = action.Trim();
        OldStatus = oldStatus;
        NewStatus = newStatus;
        OldAssigneeUserId = oldAssigneeUserId;
        NewAssigneeUserId = newAssigneeUserId;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
