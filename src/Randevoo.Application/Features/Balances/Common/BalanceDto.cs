using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.Balances.Common;

public record BalanceTransactionDto(
    decimal Amount,
    string CurrencyCode,
    decimal ReportingAmountIrr,
    decimal ExchangeRateToIrr,
    DateTime ExchangeRateCapturedAtUtc,
    BalanceTransactionType Type,
    string Description,
    long? DatingEventId,
    string? ReferenceType,
    long? ReferenceId,
    long? CreatedByUserId,
    DateTime CreatedAt);
public record BalanceDto(long UserId, decimal Balance, string ReportingCurrencyCode, IReadOnlyList<BalanceTransactionDto> Transactions)
{
    public static BalanceDto FromEntity(BalanceAccount account) =>
        new(account.UserId, account.Balance, account.ReportingCurrencyCode, account.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new BalanceTransactionDto(t.Amount, t.CurrencyCode, t.ReportingAmountIrr, t.ExchangeRateToIrr, t.ExchangeRateCapturedAtUtc, t.Type, t.Description, t.DatingEventId, t.ReferenceType, t.ReferenceId, t.CreatedByUserId, t.CreatedAt))
            .ToList());
}
