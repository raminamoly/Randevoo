using MediatR;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventChats.Commands.BlockEventChatUser;

public class BlockEventChatUserHandler : IRequestHandler<BlockEventChatUserCommand>
{
    private readonly IEventConversationRepository _conversations;
    private readonly IUnitOfWork _unitOfWork;

    public BlockEventChatUserHandler(IEventConversationRepository conversations, IUnitOfWork unitOfWork)
    {
        _conversations = conversations;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(BlockEventChatUserCommand request, CancellationToken cancellationToken)
    {
        var conversation = await _conversations.GetByIdWithDetailsAsync(request.ConversationId, cancellationToken)
            ?? throw new NotFoundException("EventConversation", request.ConversationId);

        conversation.Block(request.BlockerUserId, request.BlockedUserId);
        await _conversations.UpdateAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
