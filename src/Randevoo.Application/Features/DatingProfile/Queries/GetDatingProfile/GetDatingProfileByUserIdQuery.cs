using MediatR;
using Randevoo.Application.Features.DatingProfile.Common;

namespace Randevoo.Application.Features.DatingProfile.Queries.GetDatingProfile;

public record GetDatingProfileByUserIdQuery(long UserId) : IRequest<DatingProfileDto>;
