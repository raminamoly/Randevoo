using MediatR;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Auth.Commands.RevokeRefreshToken;

public class RevokeRefreshTokenHandler : IRequestHandler<RevokeRefreshTokenCommand>
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeHasher _codeHasher;

    public RevokeRefreshTokenHandler(IRefreshTokenRepository refreshTokens, IUnitOfWork unitOfWork, ICodeHasher codeHasher)
    {
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
        _codeHasher = codeHasher;
    }

    public async Task Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _refreshTokens.GetByTokenHashAsync(_codeHasher.Hash(request.RefreshToken), cancellationToken)
            ?? throw new BusinessRuleViolationException("Invalid refresh token", "The refresh token is not valid");

        token.Revoke(DateTime.UtcNow);
        await _refreshTokens.UpdateAsync(token, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
