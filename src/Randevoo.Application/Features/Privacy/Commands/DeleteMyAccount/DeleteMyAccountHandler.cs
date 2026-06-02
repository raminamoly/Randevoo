using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Privacy.Commands.DeleteMyAccount;

public class DeleteMyAccountHandler : IRequestHandler<DeleteMyAccountCommand>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<DeleteMyAccountHandler> _logger;

    public DeleteMyAccountHandler(IUserRepository users, IRefreshTokenRepository refreshTokens, IUnitOfWork unitOfWork, IAuditLogger auditLogger, ILogger<DeleteMyAccountHandler> logger)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        await _auditLogger.LogAsync(new AuditLogEntry(
            request.UserId,
            "AccountDeleted",
            "User",
            user.Id.ToString(),
            null,
            "{\"status\":\"anonymized\"}",
            "User requested account deletion"), cancellationToken);

        user.AnonymizeForPrivacyDeletion();
        foreach (var token in await _refreshTokens.ListByUserIdAsync(user.Id, cancellationToken))
            token.Revoke(DateTime.UtcNow);

        await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogWarning("User {UserId} deleted and anonymized their account", request.UserId);
    }
}
