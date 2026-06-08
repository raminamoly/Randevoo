using System.ComponentModel.DataAnnotations;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Support;

public sealed class SupportTicketCreateInput
{
    [Required]
    [StringLength(180, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    public SupportTicketCategory Category { get; set; } = SupportTicketCategory.GeneralQuestion;

    [Required]
    [StringLength(4000, MinimumLength = 2)]
    public string Body { get; set; } = string.Empty;
}

public sealed class SupportTicketReplyInput
{
    public long TicketId { get; set; }

    [Required]
    [StringLength(4000, MinimumLength = 2)]
    public string Body { get; set; } = string.Empty;

    public long? RepresentedUserId { get; set; }
}

public sealed class SupportTicketStatusInput
{
    public long TicketId { get; set; }
    public SupportTicketStatus Status { get; set; }
    public string? Note { get; set; }
}

public sealed class SupportTicketReassignInput
{
    public long TicketId { get; set; }
    public long? AssigneeUserId { get; set; }
    public string? Note { get; set; }
}
