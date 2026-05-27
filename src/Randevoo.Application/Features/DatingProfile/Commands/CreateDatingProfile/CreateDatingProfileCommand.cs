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
    int? HeightCm = null
) : IRequest<long>;