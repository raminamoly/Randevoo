
using MediatR;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Application.Features.DatingProfile.Commands.CreateDatingProfile;

public class CreateDatingProfileHandler : IRequestHandler<CreateDatingProfileCommand, long>
{
    private readonly IUserRepository _userRepo;
    private readonly IUserProfileRepository _profileRepo;
    private readonly IInterestRepository _interestRepo;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDatingProfileHandler(
        IUserRepository userRepo,
        IUserProfileRepository profileRepo,
        IInterestRepository interestRepo,
        IUnitOfWork unitOfWork)
    {
        _userRepo = userRepo;
        _profileRepo = profileRepo;
        _interestRepo = interestRepo;
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

        var location = new Location(request.Country, request.City, new Coordinates(request.Latitude, request.Longitude), request.Region);
        var height = request.HeightCm.HasValue ? new Height(request.HeightCm.Value) : null;

        user.CreateProfile(request.DisplayName, request.DateOfBirth, request.Gender, location, height);

        var profile = user.Profile;
        profile!.UpdateEducationLevel(request.EducationLevel);
        profile.SetSmoking(request.Smoking);
        if (request.ZodiacSignId is not null)
            profile.UpdateZodiacSign(request.ZodiacSignId);
        profile.ReplaceImages(NormalizeProfileImageUrls(request.PhotoUrls), request.PrimaryImageUrl);

        var interests = await ResolveInterestsAsync(request.InterestNames, cancellationToken);
        profile.ReplaceInterests(interests);

        await _profileRepo.AddAsync(profile!, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return profile!.Id;
    }

    private async Task<IReadOnlyList<Interest>> ResolveInterestsAsync(
        IReadOnlyList<string>? interestNames,
        CancellationToken cancellationToken)
    {
        var names = NormalizeInterestNames(interestNames);
        if (names.Count == 0)
            return [];

        var existing = await _interestRepo.GetByNamesAsync(names, cancellationToken);
        var missing = names
            .Where(name => existing.All(interest => !interest.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            .Select(name => new Interest(name))
            .ToList();

        if (missing.Count > 0)
            await _interestRepo.AddRangeAsync(missing, cancellationToken);

        return existing.Concat(missing).ToList();
    }

    private static IReadOnlyList<string> NormalizeInterestNames(IReadOnlyList<string>? names) =>
        (names ?? [])
        .Where(name => !string.IsNullOrWhiteSpace(name))
        .Select(name => name.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Take(4)
        .ToList();

    private static IReadOnlyList<string> NormalizeProfileImageUrls(IReadOnlyList<string>? imageUrls)
    {
        var urls = (imageUrls ?? [])
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => url.Trim().Replace('\\', '/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (urls.Count > 3)
            throw new BusinessRuleViolationException("Maximum profile images exceeded", "User profile cannot have more than 3 images");

        if (urls.Any(url => !url.StartsWith("/uploads/profiles/", StringComparison.OrdinalIgnoreCase)))
            throw new BusinessRuleViolationException("Invalid profile image", "Profile images must be uploaded before they can be saved");

        return urls;
    }
}
