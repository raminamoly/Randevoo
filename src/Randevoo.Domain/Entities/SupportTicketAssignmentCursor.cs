using Randevoo.Domain.Common;

namespace Randevoo.Domain.Entities;

public class SupportTicketAssignmentCursor : BaseEntity
{
    public string QueueName { get; private set; } = null!;
    public long? LastAssignedUserId { get; private set; }

    private SupportTicketAssignmentCursor() { }

    public SupportTicketAssignmentCursor(string queueName)
    {
        QueueName = queueName.Trim();
    }

    public void MarkAssigned(long userId)
    {
        LastAssignedUserId = userId;
        MarkAsUpdated();
    }
}
