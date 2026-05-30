using MediatR;
using Randevoo.Application.Features.EventChats.Common;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventChats.Commands.SendEventChatMessage;

public class SendEventChatMessageHandler : IRequestHandler<SendEventChatMessageCommand, EventConversationDto>
{
    private readonly IEventConversationRepository _conversations;
    private readonly IEventTicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;

    public SendEventChatMessageHandler(IEventConversationRepository conversations, IEventTicketRepository tickets, IUnitOfWork unitOfWork)
    {
        _conversations = conversations;
        _tickets = tickets;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventConversationDto> Handle(SendEventChatMessageCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversations.GetByIdWithDetailsAsync(request.ConversationId, cancellationToken)
            ?? throw new NotFoundException("EventConversation", request.ConversationId);

        var ticket = await _tickets.GetByEventAndUserAsync(conversation.DatingEventId, request.SenderUserId, cancellationToken)
            ?? throw new BusinessRuleViolationException("Ticket required", "Sender must have a ticket for the event");
        if (!ticket.IsValidForEventAccess)
            throw new BusinessRuleViolationException("Ticket is not valid", "Refunded or removed tickets cannot use event chat");

        conversation.SendMessage(request.SenderUserId, request.Body);
        await _conversations.UpdateAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return EventConversationDto.FromEntity(conversation);
    }
}
