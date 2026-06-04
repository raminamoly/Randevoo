using MediatR;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.ApproveEventParticipantSmsRequest;

public class ApproveEventParticipantSmsRequestHandler : IRequestHandler<ApproveEventParticipantSmsRequestCommand, int>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IEventParticipantSmsRequestRepository _requests;
    private readonly ISmsQueueRepository _smsQueue;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveEventParticipantSmsRequestHandler(
        IUserRepository users,
        IDatingEventRepository events,
        IEventParticipantSmsRequestRepository requests,
        ISmsQueueRepository smsQueue,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _events = events;
        _requests = requests;
        _smsQueue = smsQueue;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(ApproveEventParticipantSmsRequestCommand request, CancellationToken cancellationToken)
    {
        var admin = await _users.GetByIdAsync(request.AdminUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.AdminUserId);
        if (admin.Role != UserRole.Admin)
        {
            throw new BusinessRuleViolationException(
                "Access denied",
                "Only admins can approve participant SMS requests");
        }

        var smsRequest = await _requests.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new NotFoundException("EventParticipantSmsRequest", request.RequestId);
        var datingEvent = await _events.GetByIdWithTicketsAsync(smsRequest.DatingEventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", smsRequest.DatingEventId);

        var queuedItems = new List<SmsQueueItem>();
        foreach (var ticket in datingEvent.Tickets.Where(ticket => !ticket.IsRefunded))
        {
            var participant = await _users.GetByIdAsync(ticket.UserId, cancellationToken);
            if (participant is null)
            {
                continue;
            }

            queuedItems.Add(new SmsQueueItem(participant, datingEvent, request.ApprovedMessage, request.PlannedSendAtUtc, smsRequest.Id));
        }

        smsRequest.Approve(admin.Id, queuedItems.Count, request.ApprovedMessage, request.PlannedSendAtUtc, request.Note);
        await _smsQueue.AddRangeAsync(queuedItems, cancellationToken);
        await _requests.UpdateAsync(smsRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return queuedItems.Count;
    }
}
