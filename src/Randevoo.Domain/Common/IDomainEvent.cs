// Domain/Common/IDomainEvent.cs
namespace Randevoo.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}
