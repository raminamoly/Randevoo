using MediatR;
using Randevoo.Application.Features.DatingProfile.Common;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingProfile.Queries.GetDatingProfile;

public class GetDatingProfileByIdHandler : IRequestHandler<GetDatingProfileByIdQuery, DatingProfileDto>
{
    private readonly IUserProfileRepository _profileRepo;

    public GetDatingProfileByIdHandler(IUserProfileRepository profileRepo)
    {
        _profileRepo = profileRepo;
    }

    public async Task<DatingProfileDto> Handle(GetDatingProfileByIdQuery request, CancellationToken cancellationToken)
    {
        var profile = await _profileRepo.GetByIdWithDetailsAsync(request.ProfileId, cancellationToken)
            ?? throw new NotFoundException("UserProfile", request.ProfileId);

        return DatingProfileDto.FromEntity(profile);
    }
}
