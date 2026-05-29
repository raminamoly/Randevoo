using MediatR;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.DatingProfile.Commands.UpdateDatingProfile;

public record UpdateDatingProfileCommand(
    long ProfileId,
    string DisplayName,
    Gender Gender,
    string Country,
    string City,
    decimal Latitude,
    decimal Longitude,
    int HeightCm,
    EducationLevel EducationLevel,
    bool Smoking,
    string? Region = null) : IRequest;
