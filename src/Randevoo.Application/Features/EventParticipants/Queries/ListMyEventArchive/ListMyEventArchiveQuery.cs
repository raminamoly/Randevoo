using MediatR;
using Randevoo.Application.Features.EventParticipants.Common;

namespace Randevoo.Application.Features.EventParticipants.Queries.ListMyEventArchive;

public record ListMyEventArchiveQuery(long UserId) : IRequest<IReadOnlyList<EventArchiveItemDto>>;
