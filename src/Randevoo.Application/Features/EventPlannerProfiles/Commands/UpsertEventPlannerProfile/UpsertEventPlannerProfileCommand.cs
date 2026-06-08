using MediatR;
using Randevoo.Application.Features.EventPlannerProfiles.Common;

namespace Randevoo.Application.Features.EventPlannerProfiles.Commands.UpsertEventPlannerProfile;

public record UpsertEventPlannerProfileCommand(long UserId, string Title, string? PictureUrl, string Resume, string SettlementCurrencyCode = "IRR") : IRequest<EventPlannerProfileDto>;
