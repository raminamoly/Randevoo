using MediatR;

namespace Randevoo.Application.Features.DatingEvents.Commands.SetDatingEventSaleStatus;

public record SetDatingEventSaleStatusCommand(long EventId, long ActorUserId, bool Open) : IRequest;
