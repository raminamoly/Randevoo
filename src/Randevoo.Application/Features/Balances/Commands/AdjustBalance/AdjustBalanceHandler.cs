using MediatR;
using Randevoo.Application.Features.Balances.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Balances.Commands.AdjustBalance;

public class AdjustBalanceHandler : IRequestHandler<AdjustBalanceCommand, BalanceDto>
{
    private readonly IUserRepository _users;
    private readonly IBalanceAccountRepository _balances;
    private readonly IUnitOfWork _unitOfWork;

    public AdjustBalanceHandler(IUserRepository users, IBalanceAccountRepository balances, IUnitOfWork unitOfWork)
    {
        _users = users;
        _balances = balances;
        _unitOfWork = unitOfWork;
    }

    public async Task<BalanceDto> Handle(AdjustBalanceCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);
        var account = await _balances.GetByUserIdAsync(request.UserId, cancellationToken);
        var isNewAccount = account is null;
        account ??= new BalanceAccount(user);

        if (isNewAccount)
            await _balances.AddAsync(account, cancellationToken);

        if (request.Amount >= 0)
            account.Credit(request.Amount, BalanceTransactionType.AdminAdjustment, request.Description);
        else
            account.Debit(Math.Abs(request.Amount), BalanceTransactionType.AdminAdjustment, request.Description);

        if (!isNewAccount)
            await _balances.UpdateAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return BalanceDto.FromEntity(account);
    }
}
