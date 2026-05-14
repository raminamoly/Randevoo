// Domain/Events/UserProfileEvents.cs
using Randevoo.Domain.Common;
using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Events;

// When user profile is created
public class UserProfileCreatedEvent : DomainEvent
{
    public UserProfile UserProfile { get; }

    public UserProfileCreatedEvent(UserProfile userProfile)
    {
        UserProfile = userProfile;
    }
}

// When user profile is updated
public class UserProfileUpdatedEvent : DomainEvent
{
    public UserProfile UserProfile { get; }
    public string UpdatedField { get; }
    public object OldValue { get; }
    public object NewValue { get; }

    public UserProfileUpdatedEvent(UserProfile userProfile, string updatedField, object oldValue, object newValue)
    {
        UserProfile = userProfile;
        UpdatedField = updatedField;
        OldValue = oldValue;
        NewValue = newValue;
    }
}

// When interest is added
public class InterestAddedEvent : DomainEvent
{
    public UserProfile UserProfile { get; }
    public Interest Interest { get; }

    public InterestAddedEvent(UserProfile userProfile, Interest interest)
    {
        UserProfile = userProfile;
        Interest = interest;
    }
}

// When interest is removed
public class InterestRemovedEvent : DomainEvent
{
    public UserProfile UserProfile { get; }
    public Interest Interest { get; }

    public InterestRemovedEvent(UserProfile userProfile, Interest interest)
    {
        UserProfile = userProfile;
        Interest = interest;
    }
}

// For soft delete events (already referenced in BaseEntity)
public class EntitySoftDeletedEvent : DomainEvent
{
    public BaseEntity Entity { get; }

    public EntitySoftDeletedEvent(BaseEntity entity)
    {
        Entity = entity;
    }
}

public class EntityRestoredEvent : DomainEvent
{
    public BaseEntity Entity { get; }

    public EntityRestoredEvent(BaseEntity entity)
    {
        Entity = entity;
    }
}