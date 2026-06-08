using MediatR;
using Randevoo.Application.Features.EventPlannerProfiles.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventPlannerProfiles.Commands.UpsertEventPlannerProfile;

public class UpsertEventPlannerProfileHandler : IRequestHandler<UpsertEventPlannerProfileCommand, EventPlannerProfileDto>
{
    private readonly IUserRepository _users;
    private readonly IEventPlannerProfileRepository _profiles;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertEventPlannerProfileHandler(IUserRepository users, IEventPlannerProfileRepository profiles, IUnitOfWork unitOfWork)
    {
        _users = users;
        _profiles = profiles;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventPlannerProfileDto> Handle(UpsertEventPlannerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var profile = await _profiles.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile is null)
        {
            profile = new EventPlannerProfile(user, request.Title, request.PictureUrl, request.Resume, request.SettlementCurrencyCode);
            await _profiles.AddAsync(profile, cancellationToken);
        }
        else
        {
            profile.Update(request.Title, request.PictureUrl, request.Resume, request.SettlementCurrencyCode);
            await _profiles.UpdateAsync(profile, cancellationToken);
        }

        await _users.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return EventPlannerProfileDto.FromEntity(profile);
    }
}
