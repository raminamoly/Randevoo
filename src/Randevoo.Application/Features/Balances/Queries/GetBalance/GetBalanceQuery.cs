using MediatR;
using Randevoo.Application.Features.Balances.Common;

namespace Randevoo.Application.Features.Balances.Queries.GetBalance;

public record GetBalanceQuery(long UserId) : IRequest<BalanceDto>;
