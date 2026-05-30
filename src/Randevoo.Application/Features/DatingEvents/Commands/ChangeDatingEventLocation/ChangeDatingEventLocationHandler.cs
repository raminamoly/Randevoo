using MediatR;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Application.Features.DatingEvents.Commands.ChangeDatingEventLocation;

public class ChangeDatingEventLocationHandler : IRequestHandler<ChangeDatingEventLocationCommand>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeDatingEventLocationHandler(IUserRepository users, IDatingEventRepository events, IUnitOfWork unitOfWork)
    {
        _users = users;
        _events = events;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangeDatingEventLocationCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can change event location");

        datingEvent.ChangeAddressLocation(
            new Location(request.Country, request.City, new Coordinates(request.Latitude, request.Longitude), request.Region),
            request.Address);
        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
