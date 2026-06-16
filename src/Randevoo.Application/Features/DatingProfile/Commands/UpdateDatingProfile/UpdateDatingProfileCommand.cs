using MediatR;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.DatingProfile.Commands.UpdateDatingProfile;

public record UpdateDatingProfileCommand(
    long ProfileId,
    string DisplayName,
    DateOnly DateOfBirth,
    Gender Gender,
    string Country,
    string City,
    decimal Latitude,
    decimal Longitude,
    int HeightCm,
    EducationLevel EducationLevel,
    bool Smoking,
    string? Region = null,
    IReadOnlyList<string>? InterestNames = null,
    long? ZodiacSignId = null,
    IReadOnlyList<string>? PhotoUrls = null,
    string? PrimaryImageUrl = null) : IRequest;
