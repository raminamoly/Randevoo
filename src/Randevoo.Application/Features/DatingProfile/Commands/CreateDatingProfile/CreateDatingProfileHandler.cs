
using MediatR;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Application.Features.DatingProfile.Commands.CreateDatingProfile;

public class CreateDatingProfileHandler : IRequestHandler<CreateDatingProfileCommand, long>
{
    private readonly IUserRepository _userRepo;
    private readonly IUserProfileRepository _profileRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDatingProfileHandler(
        IUserRepository userRepo,
        IUserProfileRepository profileRepo,
        IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _profileRepo = profileRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> Handle(CreateDatingProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        if (await _profileRepo.GetByUserIdAsync(request.UserId, cancellationToken) is not null)
            throw new BusinessRuleViolationException("Duplicate profile", $"User {request.UserId} already has a dating profile");

        if (await _profileRepo.ExistsByDisplayNameAsync(request.DisplayName, cancellationToken))
            throw new BusinessRuleViolationException("Duplicate display name", $"Display name '{request.DisplayName}' is already taken");

        var location = new Location(request.Country, request.City, new Coordinates(request.Latitude, request.Longitude));
        var height = request.HeightCm.HasValue ? new Height(request.HeightCm.Value) : null;

        // Use aggregate behavior to create profile
        user.CreateProfile(request.DisplayName, request.DateOfBirth, request.Gender, location, height);

        // Persist (profile created as part of aggregate)
        var profile = user.Profile;
        await _profileRepo.AddAsync(profile!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return profile!.Id;
    }
}
