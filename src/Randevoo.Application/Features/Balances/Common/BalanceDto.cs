using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.Balances.Common;

public record BalanceTransactionDto(
    decimal Amount,
    BalanceTransactionType Type,
    string Description,
    long? DatingEventId,
    string? ReferenceType,
    long? ReferenceId,
    long? CreatedByUserId,
    DateTime CreatedAt);
public record BalanceDto(long UserId, decimal Balance, IReadOnlyList<BalanceTransactionDto> Transactions)
{
    public static BalanceDto FromEntity(BalanceAccount account) =>
        new(account.UserId, account.Balance, account.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new BalanceTransactionDto(t.Amount, t.Type, t.Description, t.DatingEventId, t.ReferenceType, t.ReferenceId, t.CreatedByUserId, t.CreatedAt))
            .ToList());
}
