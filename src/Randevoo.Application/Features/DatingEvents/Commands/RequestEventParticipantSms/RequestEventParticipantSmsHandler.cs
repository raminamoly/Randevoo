using MediatR;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.RequestEventParticipantSms;

public class RequestEventParticipantSmsHandler : IRequestHandler<RequestEventParticipantSmsCommand, long>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IEventParticipantSmsRequestRepository _requests;
    private readonly IUnitOfWork _unitOfWork;

    public RequestEventParticipantSmsHandler(
        IUserRepository users,
        IDatingEventRepository events,
        IEventParticipantSmsRequestRepository requests,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _events = events;
        _requests = requests;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> Handle(RequestEventParticipantSmsCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
        {
            throw new BusinessRuleViolationException(
                "Access denied",
                "Only the event owner can request participant SMS approval");
        }

        var smsRequest = new Domain.Entities.EventParticipantSmsRequest(actor, datingEvent, request.Message, request.PlannedSendAtUtc);
        await _requests.AddAsync(smsRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return smsRequest.Id;
    }
}
