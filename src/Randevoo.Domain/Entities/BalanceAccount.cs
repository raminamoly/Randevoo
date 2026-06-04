using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class BalanceAccount : BaseEntity, IAggregateRoot
{
    private readonly List<BalanceTransaction> _transactions = new();

    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public decimal Balance { get; private set; }
    public IReadOnlyList<BalanceTransaction> Transactions => _transactions.AsReadOnly();

    private BalanceAccount() { }

    public BalanceAccount(User user)
    {
        User = GuardAgainst.Object.Null(user, nameof(user));
        UserId = user.Id;
        Balance = 0;
        AddDomainEvent(new EntityCreatedEvent<BalanceAccount>(this));
    }

    public void Credit(decimal amount, BalanceTransactionType type, string description, long? datingEventId = null)
    {
        ValidateAmount(amount);
        Balance += amount;
        AddTransaction(amount, type, description, datingEventId);
        UpdateTimestamp();
    }

    public void Credit(decimal amount, BalanceTransactionType type, string description, long? datingEventId, string? referenceType, long? referenceId, long? createdByUserId)
    {
        ValidateAmount(amount);
        Balance += amount;
        AddTransaction(amount, type, description, datingEventId, referenceType, referenceId, createdByUserId);
        UpdateTimestamp();
    }

    public void Debit(decimal amount, BalanceTransactionType type, string description, long? datingEventId = null)
    {
        ValidateAmount(amount);
        if (Balance < amount)
            throw new BusinessRuleViolationException("Insufficient balance", "User balance is lower than the requested amount");

        Balance -= amount;
        AddTransaction(-amount, type, description, datingEventId);
        UpdateTimestamp();
    }

    public void Debit(decimal amount, BalanceTransactionType type, string description, long? datingEventId, string? referenceType, long? referenceId, long? createdByUserId)
    {
        ValidateAmount(amount);
        if (Balance < amount)
            throw new BusinessRuleViolationException("Insufficient balance", "User balance is lower than the requested amount");

        Balance -= amount;
        AddTransaction(-amount, type, description, datingEventId, referenceType, referenceId, createdByUserId);
        UpdateTimestamp();
    }

    public void DebitAllowNegative(decimal amount, BalanceTransactionType type, string description, long? datingEventId, string? referenceType, long? referenceId, long? createdByUserId)
    {
        ValidateAmount(amount);
        Balance -= amount;
        AddTransaction(-amount, type, description, datingEventId, referenceType, referenceId, createdByUserId);
        UpdateTimestamp();
    }

    private void AddTransaction(decimal amount, BalanceTransactionType type, string description, long? datingEventId, string? referenceType = null, long? referenceId = null, long? createdByUserId = null)
    {
        var transaction = new BalanceTransaction(this, amount, type, description, datingEventId);
        transaction.SetReference(referenceType, referenceId, createdByUserId);
        _transactions.Add(transaction);
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new BusinessRuleViolationException("Invalid amount", "Amount must be greater than zero");
    }
}
