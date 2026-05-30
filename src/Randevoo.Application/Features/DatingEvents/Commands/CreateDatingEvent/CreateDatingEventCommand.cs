using MediatR;
using Randevoo.Application.Features.DatingEvents.Common;

namespace Randevoo.Application.Features.DatingEvents.Commands.CreateDatingEvent;

public record CreateDatingEventCommand(long EventPlannerUserId, DatingEventInput Input) : IRequest<DatingEventDto>;
