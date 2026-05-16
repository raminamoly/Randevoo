// Domain/Events/UserProfileEvents.cs
using Randevoo.Domain.Common;
using Randevoo.Domain.Common.Events;
using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Events;


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

 