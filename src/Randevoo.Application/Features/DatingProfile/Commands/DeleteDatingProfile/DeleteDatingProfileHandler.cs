using MediatR;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingProfile.Commands.DeleteDatingProfile;

public class DeleteDatingProfileHandler : IRequestHandler<DeleteDatingProfileCommand>
{
    private readonly IUserProfileRepository _profileRepo;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDatingProfileHandler(IUserProfileRepository profileRepo, IUnitOfWork unitOfWork)
    {
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteDatingProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepo.GetByIdAsync(request.ProfileId, cancellationToken)
            ?? throw new NotFoundException("UserProfile", request.ProfileId);

        await _profileRepo.DeleteAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
