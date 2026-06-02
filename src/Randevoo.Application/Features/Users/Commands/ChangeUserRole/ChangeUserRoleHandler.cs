using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Users.Commands.ChangeUserRole;

public class ChangeUserRoleHandler : IRequestHandler<ChangeUserRoleCommand>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<ChangeUserRoleHandler> _logger;

    public ChangeUserRoleHandler(IUserRepository users, IUnitOfWork unitOfWork, IAuditLogger auditLogger, ILogger<ChangeUserRoleHandler> logger)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var oldRole = user.Role;
        user.ChangeUserRole(request.Role);
        await _auditLogger.LogAsync(new AuditLogEntry(
            request.ActorUserId,
            "UserRoleChanged",
            "User",
            user.Id.ToString(),
            $"{{\"role\":\"{oldRole}\"}}",
            $"{{\"role\":\"{user.Role}\"}}",
            "Admin role change"), cancellationToken);

        await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Admin {ActorUserId} changed user {UserId} role from {OldRole} to {NewRole}", request.ActorUserId, user.Id, oldRole, user.Role);
    }
}
