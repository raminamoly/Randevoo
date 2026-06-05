using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Auth.Commands.RequestMobileLoginCode;

public class RequestMobileLoginCodeHandler : IRequestHandler<RequestMobileLoginCodeCommand>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeGenerator _codeGenerator;
    private readonly ICodeHasher _codeHasher;
    private readonly ISmsSender _smsSender;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<RequestMobileLoginCodeHandler> _logger;

    public RequestMobileLoginCodeHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICodeGenerator codeGenerator,
        ICodeHasher codeHasher,
        ISmsSender smsSender,
        IAuditLogger auditLogger,
        ILogger<RequestMobileLoginCodeHandler> logger)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _codeGenerator = codeGenerator;
        _codeHasher = codeHasher;
        _smsSender = smsSender;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task Handle(RequestMobileLoginCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByMobileNumberAsync(request.MobileNumber.Trim(), cancellationToken);
        var isNewUser = user is null;
        if (user is null)
        {
            user = new User(request.MobileNumber);
        }

        var nowUtc = DateTime.UtcNow;
        var code = _codeGenerator.GenerateNumericCode(6);
        user.StartMobileLogin(_codeHasher.Hash(code), nowUtc, nowUtc.AddMinutes(5));
        if (isNewUser)
            await _users.AddAsync(user, cancellationToken);
        else
            await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogger.TryLogAsync(new AuditLogEntry(
            ActorUserId: user.Id,
            Action: isNewUser ? "MobileSignupAndLoginCodeRequested" : "MobileLoginCodeRequested",
            TargetType: "User",
            TargetId: user.Id.ToString(),
            LogType: isNewUser ? "signup" : "login_code_request",
            Module: "auth",
            Description: isNewUser ? "User created and requested a mobile login code." : "User requested a mobile login code.",
            Status: "success"), cancellationToken);

        await _smsSender.SendLoginCodeAsync(user.MobileNumber, code, cancellationToken);
        _logger.LogInformation("Mobile login code requested for user {UserId}; new user: {IsNewUser}", user.Id, isNewUser);
    }
}
