using MediatR;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.SetDatingEventSaleStatus;

public class SetDatingEventSaleStatusHandler : IRequestHandler<SetDatingEventSaleStatusCommand>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;

    public SetDatingEventSaleStatusHandler(IUserRepository users, IDatingEventRepository events, IUnitOfWork unitOfWork)
    {
        _users = users;
        _events = events;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetDatingEventSaleStatusCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can change event sale status");

        if (request.Open) datingEvent.OpenForSell(); else datingEvent.CloseForSell();
        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
