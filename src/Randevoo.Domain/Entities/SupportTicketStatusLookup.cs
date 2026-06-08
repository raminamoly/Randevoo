using Randevoo.Domain.Common;

namespace Randevoo.Domain.Entities;

public class SupportTicketStatusLookup : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string DisplayNameFa { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private SupportTicketStatusLookup() { }
}
