using MediatR;
using Randevoo.Application.Features.DatingProfile.Common;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingProfile.Queries.GetDatingProfile;

public class GetDatingProfileByUserIdHandler : IRequestHandler<GetDatingProfileByUserIdQuery, DatingProfileDto>
{
    private readonly IUserProfileRepository _profileRepo;

    public GetDatingProfileByUserIdHandler(IUserProfileRepository profileRepo)
    {
        _profileRepo = profileRepo;
    }

    public async Task<DatingProfileDto> Handle(GetDatingProfileByUserIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepo.GetByUserIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("UserProfile", $"UserId {request.UserId}");

        return DatingProfileDto.FromEntity(profile);
    }
}
