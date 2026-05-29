using MediatR;

namespace Randevoo.Application.Features.DatingProfile.Commands.DeleteDatingProfile;

public record DeleteDatingProfileCommand(long ProfileId) : IRequest;
