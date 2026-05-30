using MediatR;
using Randevoo.Application.Features.EventChats.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventChats.Commands.StartEventConversation;

public class StartEventConversationHandler : IRequestHandler<StartEventConversationCommand, EventConversationDto>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IEventTicketRepository _tickets;
    private readonly IEventConversationRepository _conversations;
    private readonly IUnitOfWork _unitOfWork;

    public StartEventConversationHandler(
        IUserRepository users,
        IDatingEventRepository events,
        IEventTicketRepository tickets,
        IEventConversationRepository conversations,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _events = events;
        _tickets = tickets;
        _conversations = conversations;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventConversationDto> Handle(StartEventConversationCommand request, CancellationToken cancellationToken)
    {
        var existing = await _conversations.GetBetweenParticipantsAsync(request.EventId, request.StarterUserId, request.ParticipantUserId, cancellationToken);
        if (existing is not null)
            return EventConversationDto.FromEntity(existing);

        var starter = await _users.GetByIdAsync(request.StarterUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.StarterUserId);
        var participant = await _users.GetByIdAsync(request.ParticipantUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ParticipantUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.DateTimeStart > DateTime.UtcNow)
            throw new BusinessRuleViolationException("Event has not started", "Chats can start after event start time");

        await EnsureValidTicketAsync(request.EventId, request.StarterUserId, cancellationToken);
        await EnsureValidTicketAsync(request.EventId, request.ParticipantUserId, cancellationToken);

        var connectionCount = await _conversations.CountActiveConnectionsForUserAsync(request.EventId, request.StarterUserId, cancellationToken);
        if (connectionCount >= datingEvent.NumberOfChatAllowed)
            throw new BusinessRuleViolationException("Chat limit reached", "User reached the event chat connection limit");

        var conversation = new EventConversation(datingEvent, starter, participant);
        await _conversations.AddAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return EventConversationDto.FromEntity(conversation);
    }

    private async Task EnsureValidTicketAsync(long eventId, long userId, CancellationToken cancellationToken)
    {
        var ticket = await _tickets.GetByEventAndUserAsync(eventId, userId, cancellationToken)
            ?? throw new BusinessRuleViolationException("Ticket required", "Both users must have tickets for the event");

        if (!ticket.IsValidForEventAccess)
            throw new BusinessRuleViolationException("Ticket is not valid", "Refunded or removed tickets cannot use event chat");
    }
}
