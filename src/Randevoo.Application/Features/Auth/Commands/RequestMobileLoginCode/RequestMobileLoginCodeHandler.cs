using MediatR;
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

    public RequestMobileLoginCodeHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICodeGenerator codeGenerator,
        ICodeHasher codeHasher,
        ISmsSender smsSender)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _codeGenerator = codeGenerator;
        _codeHasher = codeHasher;
        _smsSender = smsSender;
    }

    public async Task Handle(RequestMobileLoginCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByMobileNumberAsync(request.MobileNumber.Trim(), cancellationToken);
        var isNewUser = user is null;
        if (user is null)
        {
            user = new User(request.MobileNumber);
        }

        var code = _codeGenerator.GenerateNumericCode(6);
        user.StartMobileLogin(_codeHasher.Hash(code), DateTime.UtcNow.AddMinutes(5));
        if (isNewUser)
            await _users.AddAsync(user, cancellationToken);
        else
            await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _smsSender.SendLoginCodeAsync(user.MobileNumber, code, cancellationToken);
    }
}
