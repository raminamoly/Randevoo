// Domain/Common/BaseEntity.cs
using System.Collections.Generic;
using Randevoo.Domain.Common.Events;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Common;

public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    public long Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }

    // Domain Events collection - READ ONLY from outside
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }

    // Domain Events methods
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    // Soft delete methods
    public virtual void SoftDelete()
    {
        if (IsDeleted) return;

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdateTimestamp();

        AddDomainEvent(new EntitySoftDeletedEvent(this));
    }

    public virtual void Restore()
    {
        if (!IsDeleted) return;

        IsDeleted = false;
        DeletedAt = null;
        UpdateTimestamp();

        AddDomainEvent(new EntityRestoredEvent(this));
    }

    // Timestamp update
    protected void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    // For marking as updated (used in behavior methods)
    protected void MarkAsUpdated()
    {
        UpdateTimestamp();
    }
}