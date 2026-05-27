
using MediatR;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Application.Features.DatingProfile.Commands.CreateDatingProfile;

public class CreateDatingProfileHandler : IRequestHandler<CreateDatingProfileCommand, long>
{
    private readonly IUserRepository _userRepo;
    private readonly IUserProfileRepository _profileRepo;

    public CreateDatingProfileHandler(IUserRepository userRepo, IUserProfileRepository profileRepo)
    {
        _userRepo = userRepo;
        _profileRepo = profileRepo;
    }

    public async Task<long> Handle(CreateDatingProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new InvalidOperationException($"User with id {request.UserId} not found.");

        var location = new Location(request.Country, request.City, new Coordinates(request.Latitude, request.Longitude));
        var height = request.HeightCm.HasValue ? new Height(request.HeightCm.Value) : null;

        // Use aggregate behavior to create profile
        user.CreateProfile(request.DisplayName, request.DateOfBirth, request.Gender, location, height);

        // Persist (profile created as part of aggregate)
        var profile = user.Profile;
        await _profileRepo.AddAsync(profile, cancellationToken);

        return profile.Id;
    }
}