using Randevoo.Domain.Entities;

namespace Randevoo.Application.Features.EventTypes.Common;

public record EventTypeDto(long Id, string Name, string? Description, bool IsActive)
{
    public static EventTypeDto FromEntity(EventType eventType) =>
        new(eventType.Id, eventType.Name, eventType.Description, eventType.IsActive);
}
