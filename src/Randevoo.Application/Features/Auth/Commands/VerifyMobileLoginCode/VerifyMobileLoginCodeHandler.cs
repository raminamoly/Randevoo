using MediatR;
using Randevoo.Application.Features.Auth.Common;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Auth.Commands.VerifyMobileLoginCode;

public class VerifyMobileLoginCodeHandler : IRequestHandler<VerifyMobileLoginCodeCommand, AuthResult>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeHasher _codeHasher;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IAuthTokenPolicy _authTokenPolicy;

    public VerifyMobileLoginCodeHandler(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICodeHasher codeHasher,
        ICodeGenerator codeGenerator,
        IJwtTokenService jwtTokenService,
        IRefreshTokenRepository refreshTokens,
        IAuthTokenPolicy authTokenPolicy)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _codeHasher = codeHasher;
        _codeGenerator = codeGenerator;
        _jwtTokenService = jwtTokenService;
        _refreshTokens = refreshTokens;
        _authTokenPolicy = authTokenPolicy;
    }

    public async Task<AuthResult> Handle(VerifyMobileLoginCodeCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByMobileNumberAsync(request.MobileNumber.Trim(), cancellationToken)
            ?? throw new NotFoundException("User", request.MobileNumber);

        var nowUtc = DateTime.UtcNow;
        try
        {
            user.CompleteMobileLogin(_codeHasher.Hash(request.Code), nowUtc);
        }
        catch (BusinessRuleViolationException)
        {
            await _users.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw;
        }

        await _users.UpdateAsync(user, cancellationToken);

        var refreshToken = _codeGenerator.GenerateToken();
        var refreshTokenExpiresAt = nowUtc.AddDays(_authTokenPolicy.RefreshTokenExpiresDays);
        await _refreshTokens.AddAsync(new RefreshToken(user.Id, _codeHasher.Hash(refreshToken), refreshTokenExpiresAt), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenService.CreateToken(user);
        return new AuthResult(user.Id, user.MobileNumber, accessToken.Token, accessToken.ExpiresAtUtc, refreshToken, refreshTokenExpiresAt);
    }
}
