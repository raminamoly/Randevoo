using System.ComponentModel.DataAnnotations;
using Randevoo.Domain.Constants;

namespace Randevoo.AdminPanel.Models.Support;

public sealed class SupportTicketCreateInput
{
    [Required]
    [StringLength(180, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Range(1, long.MaxValue)]
    public long TicketTypeId { get; set; } = SupportTicketLookupIds.TypeGeneralQuestion;

    [Range(1, long.MaxValue)]
    public long TicketRecipientTypeId { get; set; } = SupportTicketLookupIds.RecipientPlatformSupport;

    public long? EventId { get; set; }

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
    [Range(1, long.MaxValue)]
    public long TicketStatusId { get; set; } = SupportTicketLookupIds.StatusInProgress;
    public string? Note { get; set; }
}

public sealed class SupportTicketReassignInput
{
    public long TicketId { get; set; }
    public long? AssigneeUserId { get; set; }
    public string? Note { get; set; }
}

public sealed record SupportTicketLookupOption(long Id, string TitleFa);

public sealed record SupportTicketEventOption(long Id, string Title, string PlannerDisplayName);
