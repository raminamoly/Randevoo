using MediatR;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.SendSmsToParticipants;

public class SendSmsToParticipantsHandler : IRequestHandler<SendSmsToParticipantsCommand>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly ISmsSender _smsSender;

    public SendSmsToParticipantsHandler(IUserRepository users, IDatingEventRepository events, ISmsSender smsSender)
    {
        _users = users;
        _events = events;
        _smsSender = smsSender;
    }

    public async Task Handle(SendSmsToParticipantsCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdWithTicketsAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can message participants");

        foreach (var ticket in datingEvent.Tickets.Where(t => !t.IsRefunded))
        {
            var participant = await _users.GetByIdAsync(ticket.UserId, cancellationToken);
            if (participant is not null)
                await _smsSender.SendMessageAsync(participant.MobileNumber, request.Message, cancellationToken);
        }
    }
}
