// Domain/Common/IDomainEvent.cs
namespace Randevoo.Domain.Common.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}
