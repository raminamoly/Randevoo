using MediatR;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.DatingProfile.Commands.CreateDatingProfile;

public record CreateDatingProfileCommand(
    long UserId,
    string DisplayName,
    DateOnly DateOfBirth,
    Gender Gender,
    string Country,
    string City,
    decimal Latitude,
    decimal Longitude,
    int? HeightCm = null,
    EducationLevel EducationLevel = EducationLevel.NotSpecified,
    bool Smoking = false,
    string? Region = null,
    IReadOnlyList<string>? InterestNames = null,
    long? ZodiacSignId = null,
    IReadOnlyList<string>? PhotoUrls = null,
    string? PrimaryImageUrl = null
) : IRequest<long>;
