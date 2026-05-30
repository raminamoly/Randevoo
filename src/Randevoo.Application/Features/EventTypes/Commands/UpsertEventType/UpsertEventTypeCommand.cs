using MediatR;
using Randevoo.Application.Features.EventTypes.Common;

namespace Randevoo.Application.Features.EventTypes.Commands.UpsertEventType;

public record UpsertEventTypeCommand(long? Id, string Name, string? Description, bool IsActive) : IRequest<EventTypeDto>;
