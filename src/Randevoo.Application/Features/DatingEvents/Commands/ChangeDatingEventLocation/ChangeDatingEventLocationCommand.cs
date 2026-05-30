using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.ChangeDatingEventLocation;

public record ChangeDatingEventLocationCommand(
    long ActorUserId,
    long EventId,
    string Country,
    string City,
    string? Region,
    decimal Latitude,
    decimal Longitude,
    string Address) : IRequest;
