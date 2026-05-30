using MediatR;
using Randevoo.Application.Features.DatingProfile.Common;

namespace Randevoo.Application.Features.EventParticipants.Queries.ListVisibleParticipantProfiles;

public record ListVisibleParticipantProfilesQuery(long UserId, long EventId) : IRequest<IReadOnlyList<DatingProfileDto>>;
