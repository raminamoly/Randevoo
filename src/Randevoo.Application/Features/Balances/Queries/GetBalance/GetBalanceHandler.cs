using MediatR;
using Randevoo.Application.Features.Balances.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Balances.Queries.GetBalance;

public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, BalanceDto>
{
    private readonly IUserRepository _users;
    private readonly IBalanceAccountRepository _balances;
    private readonly IUnitOfWork _unitOfWork;

    public GetBalanceHandler(IUserRepository users, IBalanceAccountRepository balances, IUnitOfWork unitOfWork)
    {
        _users = users;
        _balances = balances;
        _unitOfWork = unitOfWork;
    }

    public async Task<BalanceDto> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var account = await _balances.GetByUserIdAsync(request.UserId, cancellationToken);
        if (account is not null)
            return BalanceDto.FromEntity(account);

        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new Randevoo.Domain.Exceptions.NotFoundException("User", request.UserId);

        account = new BalanceAccount(user);
        await _balances.AddAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return BalanceDto.FromEntity(account);
    }
}
