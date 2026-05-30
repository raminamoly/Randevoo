using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Entities;

public class BalanceTransaction : BaseEntity
{
    public long BalanceAccountId { get; private set; }
    public BalanceAccount BalanceAccount { get; private set; } = null!;
    public long UserId { get; private set; }
    public decimal Amount { get; private set; }
    public BalanceTransactionType Type { get; private set; }
    public string Description { get; private set; } = null!;
    public long? DatingEventId { get; private set; }
    public string? ReferenceType { get; private set; }
    public long? ReferenceId { get; private set; }
    public long? CreatedByUserId { get; private set; }

    private BalanceTransaction() { }

    internal BalanceTransaction(BalanceAccount account, decimal amount, BalanceTransactionType type, string description, long? datingEventId)
    {
        BalanceAccount = account;
        UserId = account.UserId;
        Amount = amount;
        Type = type;
        Description = GuardAgainst.String.InvalidLength(description, nameof(description), 2, 300);
        DatingEventId = datingEventId;
    }

    public void SetReference(string? referenceType, long? referenceId, long? createdByUserId)
    {
        ReferenceType = string.IsNullOrWhiteSpace(referenceType) ? null : GuardAgainst.String.MaxLength(referenceType, nameof(referenceType), 100);
        ReferenceId = referenceId;
        CreatedByUserId = createdByUserId;
    }
}
