using MediatR;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Privacy.Commands.DeleteMyAccount;

public class DeleteMyAccountHandler : IRequestHandler<DeleteMyAccountCommand>
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMyAccountHandler(IUserRepository users, IRefreshTokenRepository refreshTokens, IUnitOfWork unitOfWork)
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        user.AnonymizeForPrivacyDeletion();
        foreach (var token in await _refreshTokens.ListByUserIdAsync(user.Id, cancellationToken))
            token.Revoke(DateTime.UtcNow);

        await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
