using MediatR;
using Randevoo.Application.Features.EventChats.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventChats.Commands.StartEventConversation;

public class StartEventConversationHandler : IRequestHandler<StartEventConversationCommand, EventLikeResultDto>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IEventTicketRepository _tickets;
    private readonly IEventConversationRepository _conversations;
    private readonly IEventLikeRepository _likes;
    private readonly IUnitOfWork _unitOfWork;

    public StartEventConversationHandler(
        IUserRepository users,
        IDatingEventRepository events,
        IEventTicketRepository tickets,
        IEventConversationRepository conversations,
        IEventLikeRepository likes,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _events = events;
        _tickets = tickets;
        _conversations = conversations;
        _likes = likes;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventLikeResultDto> Handle(StartEventConversationCommand request, CancellationToken cancellationToken)
    {
        var starter = await _users.GetByIdAsync(request.StarterUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.StarterUserId);
        var participant = await _users.GetByIdAsync(request.ParticipantUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ParticipantUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        var existing = await _conversations.GetBetweenParticipantsAsync(request.EventId, request.StarterUserId, request.ParticipantUserId, cancellationToken);
        if (existing is not null)
        {
            var existingLike = await _likes.GetDirectedAsync(request.EventId, request.StarterUserId, request.ParticipantUserId, cancellationToken);
            return new EventLikeResultDto(
                existingLike?.Id ?? 0,
                request.EventId,
                request.StarterUserId,
                request.ParticipantUserId,
                EventLikeStatus.Matched,
                EventConversationDto.FromEntity(existing));
        }

        if (datingEvent.DateTimeStart > DateTime.UtcNow)
            throw new BusinessRuleViolationException("Event has not started", "Chats can start after event start time");

        await EnsureValidTicketAsync(request.EventId, request.StarterUserId, cancellationToken);
        await EnsureValidTicketAsync(request.EventId, request.ParticipantUserId, cancellationToken);

        var connectionCount = await _conversations.CountActiveConnectionsForUserAsync(request.EventId, request.StarterUserId, cancellationToken);
        if (connectionCount >= datingEvent.NumberOfLikesAllowed)
            throw new BusinessRuleViolationException("Like limit reached", "User reached the event like limit");

        var directLike = await _likes.GetDirectedAsync(request.EventId, request.StarterUserId, request.ParticipantUserId, cancellationToken);
        if (directLike is not null)
            return EventLikeResultDto.FromEntity(directLike);

        var reverseLike = await _likes.GetReverseAsync(request.EventId, request.StarterUserId, request.ParticipantUserId, cancellationToken);
        if (reverseLike is not null)
        {
            if (reverseLike.Status == EventLikeStatus.Rejected)
                throw new BusinessRuleViolationException("Like rejected", "The other user rejected this event like");

            reverseLike.MarkMatched();
            await _likes.UpdateAsync(reverseLike, cancellationToken);

            var matchedLike = new EventLike(datingEvent, starter, participant);
            matchedLike.MarkMatched();
            await _likes.AddAsync(matchedLike, cancellationToken);

            var matchedConversation = new EventConversation(datingEvent, starter, participant);
            await _conversations.AddAsync(matchedConversation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return EventLikeResultDto.FromEntity(matchedLike, matchedConversation);
        }

        var eventLike = new EventLike(datingEvent, starter, participant);
        await _likes.AddAsync(eventLike, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return EventLikeResultDto.FromEntity(eventLike);
    }

    private async Task<EventConversationDto> CreateConversationAsync(DatingEvent datingEvent, User starter, User participant, CancellationToken cancellationToken)
    {
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
