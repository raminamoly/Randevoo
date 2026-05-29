using MediatR;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Auth.Commands.ConfirmEmail;

public class ConfirmEmailHandler : IRequestHandler<ConfirmEmailCommand>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICodeHasher _codeHasher;

    public ConfirmEmailHandler(IUserRepository users, IUnitOfWork unitOfWork, ICodeHasher codeHasher)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _codeHasher = codeHasher;
    }

    public async Task Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        user.ConfirmEmail(_codeHasher.Hash(request.Token), DateTime.UtcNow);
        await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
