using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Features.Balances.Common;
using Randevoo.Application.Interfaces.Auditing;
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
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<AdjustBalanceHandler> _logger;

    public AdjustBalanceHandler(IUserRepository users, IBalanceAccountRepository balances, IUnitOfWork unitOfWork, IAuditLogger auditLogger, ILogger<AdjustBalanceHandler> logger)
    {
        _users = users;
        _balances = balances;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _logger = logger;
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

        var beforeBalance = account.Balance;
        if (request.Amount >= 0)
            account.Credit(request.Amount, BalanceTransactionType.AdminAdjustment, request.Description);
        else
            account.Debit(Math.Abs(request.Amount), BalanceTransactionType.AdminAdjustment, request.Description);

        await _auditLogger.LogAsync(new AuditLogEntry(
            request.ActorUserId,
            "BalanceAdjusted",
            "User",
            user.Id.ToString(),
            $"{{\"balance\":{beforeBalance}}}",
            $"{{\"balance\":{account.Balance},\"amount\":{request.Amount}}}",
            request.Description), cancellationToken);

        if (!isNewAccount)
            await _balances.UpdateAsync(account, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Admin {ActorUserId} adjusted balance for user {UserId} by {Amount}", request.ActorUserId, request.UserId, request.Amount);
        return BalanceDto.FromEntity(account);
    }
}
