namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventListFilter
{
    public string? Search { get; set; }
    public long? TagId { get; set; }
    public string? City { get; set; }
    public long? EventModeId { get; set; }
    public EventOperationalStatus? OperationalStatus { get; set; }
    public Randevoo.Domain.Enums.EventApprovalStatus? ApprovalStatus { get; set; }
    public DateTimeOffset? FromDateUtc { get; set; }
    public DateTimeOffset? ToDateUtc { get; set; }
    public string Sort { get; set; } = "updated-desc";
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 25;
    public EventListScope Scope { get; set; } = EventListScope.Active;
}
