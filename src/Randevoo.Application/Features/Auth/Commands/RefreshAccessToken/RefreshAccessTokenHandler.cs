using MediatR;
using Randevoo.Application.Features.Auth.Common;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Auth.Commands.RefreshAccessToken;

public class RefreshAccessTokenHandler : IRequestHandler<RefreshAccessTokenCommand, AuthResult>
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeGenerator _codeGenerator;
    private readonly ICodeHasher _codeHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuthTokenPolicy _authTokenPolicy;

    public RefreshAccessTokenHandler(
        IRefreshTokenRepository refreshTokens,
        IUserRepository users,
        IUnitOfWork unitOfWork,
        ICodeGenerator codeGenerator,
        ICodeHasher codeHasher,
        IJwtTokenService jwtTokenService,
        IAuthTokenPolicy authTokenPolicy)
    {
        _refreshTokens = refreshTokens;
        _users = users;
        _unitOfWork = unitOfWork;
        _codeGenerator = codeGenerator;
        _codeHasher = codeHasher;
        _jwtTokenService = jwtTokenService;
        _authTokenPolicy = authTokenPolicy;
    }

    public async Task<AuthResult> Handle(RefreshAccessTokenCommand request, CancellationToken cancellationToken)
    {
        var nowUtc = DateTime.UtcNow;
        var currentHash = _codeHasher.Hash(request.RefreshToken);
        var current = await _refreshTokens.GetByTokenHashAsync(currentHash, cancellationToken)
            ?? throw new BusinessRuleViolationException("Invalid refresh token", "The refresh token is not valid");

        current.EnsureActive(nowUtc);
        var user = await _users.GetByIdAsync(current.UserId, cancellationToken)
            ?? throw new NotFoundException("User", current.UserId);

        if (!user.IsActive)
            throw new BusinessRuleViolationException("Inactive user", "The user account is not active");

        var refreshToken = _codeGenerator.GenerateToken();
        var refreshTokenExpiresAt = nowUtc.AddDays(_authTokenPolicy.RefreshTokenExpiresDays);
        var refreshTokenHash = _codeHasher.Hash(refreshToken);
        current.Rotate(refreshTokenHash, nowUtc);

        await _refreshTokens.UpdateAsync(current, cancellationToken);
        await _refreshTokens.AddAsync(new RefreshToken(user.Id, refreshTokenHash, refreshTokenExpiresAt), cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _jwtTokenService.CreateToken(user);
        return new AuthResult(user.Id, user.MobileNumber, accessToken.Token, accessToken.ExpiresAtUtc, refreshToken, refreshTokenExpiresAt);
    }
}
