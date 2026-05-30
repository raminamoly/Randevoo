using MediatR;
using Randevoo.Application.Features.EventChats.Common;

namespace Randevoo.Application.Features.EventChats.Queries.ListMyEventConversations;

public record ListMyEventConversationsQuery(long UserId) : IRequest<IReadOnlyList<EventConversationDto>>;
