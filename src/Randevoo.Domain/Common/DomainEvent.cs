
// Domain/Common/DomainEvent.cs (Base implementation)
namespace Randevoo.Domain.Common;

public abstract class DomainEvent : IDomainEvent
{
    public DateTime OccurredOn { get; protected set; }
    public Guid EventId { get; protected set; }

    protected DomainEvent()
    {
        EventId = Guid.NewGuid(); 

        OccurredOn = DateTime.UtcNow;
    }
}
