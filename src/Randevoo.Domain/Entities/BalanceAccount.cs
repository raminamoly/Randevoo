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
    public string ReportingCurrencyCode { get; private set; } = "IRR";
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
        Credit(amount, type, description, datingEventId, null, null, null);
    }

    public void Credit(decimal amount, BalanceTransactionType type, string description, long? datingEventId, string? referenceType, long? referenceId, long? createdByUserId)
    {
        Credit(amount, type, description, datingEventId, referenceType, referenceId, createdByUserId, "IRR", amount, 1m, DateTime.UtcNow);
    }

    public void Credit(
        decimal amount,
        BalanceTransactionType type,
        string description,
        long? datingEventId,
        string? referenceType,
        long? referenceId,
        long? createdByUserId,
        string currencyCode,
        decimal reportingAmountIrr,
        decimal exchangeRateToIrr,
        DateTime exchangeRateCapturedAtUtc,
        long? exchangeRateId = null,
        TicketOrder? ticketOrder = null)
    {
        ValidateAmount(amount);
        ValidateAmount(reportingAmountIrr);
        Balance += reportingAmountIrr;
        AddTransaction(amount, type, description, datingEventId, referenceType, referenceId, createdByUserId, currencyCode, reportingAmountIrr, exchangeRateToIrr, exchangeRateCapturedAtUtc, exchangeRateId, ticketOrder);
        UpdateTimestamp();
    }

    public void Debit(decimal amount, BalanceTransactionType type, string description, long? datingEventId = null)
    {
        Debit(amount, type, description, datingEventId, null, null, null);
    }

    public void Debit(decimal amount, BalanceTransactionType type, string description, long? datingEventId, string? referenceType, long? referenceId, long? createdByUserId)
    {
        Debit(amount, type, description, datingEventId, referenceType, referenceId, createdByUserId, "IRR", amount, 1m, DateTime.UtcNow);
    }

    public void Debit(
        decimal amount,
        BalanceTransactionType type,
        string description,
        long? datingEventId,
        string? referenceType,
        long? referenceId,
        long? createdByUserId,
        string currencyCode,
        decimal reportingAmountIrr,
        decimal exchangeRateToIrr,
        DateTime exchangeRateCapturedAtUtc,
        long? exchangeRateId = null,
        TicketOrder? ticketOrder = null)
    {
        ValidateAmount(amount);
        ValidateAmount(reportingAmountIrr);
        if (Balance < reportingAmountIrr)
            throw new BusinessRuleViolationException("Insufficient balance", "User balance is lower than the requested amount");

        Balance -= reportingAmountIrr;
        AddTransaction(-amount, type, description, datingEventId, referenceType, referenceId, createdByUserId, currencyCode, -reportingAmountIrr, exchangeRateToIrr, exchangeRateCapturedAtUtc, exchangeRateId, ticketOrder);
        UpdateTimestamp();
    }

    public void DebitAllowNegative(decimal amount, BalanceTransactionType type, string description, long? datingEventId, string? referenceType, long? referenceId, long? createdByUserId)
    {
        DebitAllowNegative(amount, type, description, datingEventId, referenceType, referenceId, createdByUserId, "IRR", amount, 1m, DateTime.UtcNow);
    }

    public void DebitAllowNegative(
        decimal amount,
        BalanceTransactionType type,
        string description,
        long? datingEventId,
        string? referenceType,
        long? referenceId,
        long? createdByUserId,
        string currencyCode,
        decimal reportingAmountIrr,
        decimal exchangeRateToIrr,
        DateTime exchangeRateCapturedAtUtc,
        long? exchangeRateId = null,
        TicketOrder? ticketOrder = null)
    {
        ValidateAmount(amount);
        ValidateAmount(reportingAmountIrr);
        Balance -= reportingAmountIrr;
        AddTransaction(-amount, type, description, datingEventId, referenceType, referenceId, createdByUserId, currencyCode, -reportingAmountIrr, exchangeRateToIrr, exchangeRateCapturedAtUtc, exchangeRateId, ticketOrder);
        UpdateTimestamp();
    }

    private void AddTransaction(
        decimal amount,
        BalanceTransactionType type,
        string description,
        long? datingEventId,
        string? referenceType = null,
        long? referenceId = null,
        long? createdByUserId = null,
        string currencyCode = "IRR",
        decimal? reportingAmountIrr = null,
        decimal exchangeRateToIrr = 1m,
        DateTime? exchangeRateCapturedAtUtc = null,
        long? exchangeRateId = null,
        TicketOrder? ticketOrder = null)
    {
        var transaction = new BalanceTransaction(
            this,
            amount,
            type,
            description,
            datingEventId,
            currencyCode,
            reportingAmountIrr ?? amount,
            exchangeRateToIrr,
            exchangeRateCapturedAtUtc ?? DateTime.UtcNow,
            exchangeRateId);
        transaction.SetReference(referenceType, referenceId, createdByUserId);
        transaction.SetTicketOrder(ticketOrder);
        _transactions.Add(transaction);
    }

    private static void ValidateAmount(decimal amount)
    {
        if (amount <= 0)
            throw new BusinessRuleViolationException("Invalid amount", "Amount must be greater than zero");
    }
}
