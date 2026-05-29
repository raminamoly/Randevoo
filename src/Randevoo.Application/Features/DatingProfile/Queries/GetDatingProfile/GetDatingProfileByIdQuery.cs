using MediatR;
using Randevoo.Application.Features.DatingProfile.Common;

namespace Randevoo.Application.Features.DatingProfile.Queries.GetDatingProfile;

public record GetDatingProfileByIdQuery(long ProfileId) : IRequest<DatingProfileDto>;
