using MediatR;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Application.Features.DatingProfile.Commands.UpdateDatingProfile;

public class UpdateDatingProfileHandler : IRequestHandler<UpdateDatingProfileCommand>
{
    private readonly IUserProfileRepository _profileRepo;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDatingProfileHandler(IUserProfileRepository profileRepo, IUnitOfWork unitOfWork)
    {
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateDatingProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepo.GetByIdAsync(request.ProfileId, cancellationToken)
            ?? throw new NotFoundException("UserProfile", request.ProfileId);

        if (await _profileRepo.ExistsByDisplayNameAsync(request.DisplayName, request.ProfileId, cancellationToken))
            throw new BusinessRuleViolationException("Duplicate display name", $"Display name '{request.DisplayName}' is already taken");

        var location = new Location(
            request.Country,
            request.City,
            new Coordinates(request.Latitude, request.Longitude),
            request.Region);

        profile.UpdateProfile(
            request.DisplayName,
            request.Gender,
            location,
            new Height(request.HeightCm),
            request.EducationLevel,
            request.Smoking);

        await _profileRepo.UpdateAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
