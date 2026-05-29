using MediatR;
using Randevoo.Application.Features.Auth.Common;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Auth.Commands.VerifyMobileLoginCode;

public class VerifyMobileLoginCodeHandler : IRequestHandler<VerifyMobileLoginCodeCommand, AuthResult>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeHasher _codeHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public VerifyMobileLoginCodeHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICodeHasher codeHasher,
        IJwtTokenService jwtTokenService)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _codeHasher = codeHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResult> Handle(VerifyMobileLoginCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByMobileNumberAsync(request.MobileNumber.Trim(), cancellationToken)
            ?? throw new NotFoundException("User", request.MobileNumber);

        user.CompleteMobileLogin(_codeHasher.Hash(request.Code), DateTime.UtcNow);
        await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResult(user.Id, user.MobileNumber, _jwtTokenService.CreateToken(user));
    }
}
