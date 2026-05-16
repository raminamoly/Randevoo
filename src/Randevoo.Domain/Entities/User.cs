using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{
    // User identity properties
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

  
    public UserProfile Profile { get; private set; }

    private User() { } // EF Core

    public User(string email, string passwordHash)
    {
        Email = GuardAgainst.String.InvalidLength(email, nameof(email), 5, 100);
        PasswordHash = GuardAgainst.String.NullOrWhiteSpace(passwordHash, nameof(passwordHash));
        Role = UserRole.Basic;
        IsActive = true;
         
        // Profile will be created later via CreateProfile()
    }

  
    public void CreateProfile(string displayName, DateOnly dateOfBirth,
                              Gender gender, Location location, Height? height = null)
    {

        GuardAgainst.Entity.AlreadyExists(
          Profile,                           // The entity to check
          nameof(UserProfile),               // Entity name
          $"UserId: {Id}"                    // Identifier
      );

        Profile = new UserProfile(this, displayName, dateOfBirth, gender, location, height);
        AddDomainEvent(new EntityCreatedEvent<UserProfile>(Profile));
    }

    public void UpdatePassword(string newHash)
    {
        PasswordHash = newHash;
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdateTimestamp();
    }

    public void PromoteToPremium()
    {
        Role = UserRole.Premium;
        UpdateTimestamp();
    }
}