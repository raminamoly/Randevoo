using MediatR;
using Randevoo.Application.Features.Balances.Common;

namespace Randevoo.Application.Features.Balances.Commands.AdjustBalance;

public record AdjustBalanceCommand(long ActorUserId, long UserId, decimal Amount, string Description) : IRequest<BalanceDto>;
