using MediatR;
using Randevoo.Application.Features.EventChats.Common;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventChats.Queries.ListMyEventConversations;

public class ListMyEventConversationsHandler : IRequestHandler<ListMyEventConversationsQuery, IReadOnlyList<EventConversationDto>>
{
    private readonly IEventConversationRepository _conversations;

    public ListMyEventConversationsHandler(IEventConversationRepository conversations)
    {
        _conversations = conversations;
    }

    public async Task<IReadOnlyList<EventConversationDto>> Handle(ListMyEventConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversations = await _conversations.ListForUserAsync(request.UserId, cancellationToken);
        return conversations.Select(EventConversationDto.FromEntity).ToList();
    }
}
