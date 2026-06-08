using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class BalanceTransactionTypeLookup : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = null!;
    public string DisplayNameFa { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }

    private BalanceTransactionTypeLookup() { }
}
