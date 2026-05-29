using MediatR;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Auth.Commands.RequestEmailConfirmation;

public class RequestEmailConfirmationHandler : IRequestHandler<RequestEmailConfirmationCommand>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeGenerator _codeGenerator;
    private readonly ICodeHasher _codeHasher;
    private readonly IEmailSender _emailSender;

    public RequestEmailConfirmationHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICodeGenerator codeGenerator,
        ICodeHasher codeHasher,
        IEmailSender emailSender)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _codeGenerator = codeGenerator;
        _codeHasher = codeHasher;
        _emailSender = emailSender;
    }

    public async Task Handle(RequestEmailConfirmationCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        if (await _users.ExistsByEmailAsync(normalizedEmail, cancellationToken))
            throw new BusinessRuleViolationException("Duplicate email", $"Email '{normalizedEmail}' is already in use");

        var token = _codeGenerator.GenerateToken();
        user.StartEmailConfirmation(normalizedEmail, _codeHasher.Hash(token), DateTime.UtcNow.AddHours(24));
        await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var confirmationLink = $"{request.ConfirmationBaseUrl.TrimEnd('/')}/api/auth/email/confirm?userId={user.Id}&token={Uri.EscapeDataString(token)}";
        await _emailSender.SendEmailConfirmationAsync(normalizedEmail, confirmationLink, cancellationToken);
    }
}
