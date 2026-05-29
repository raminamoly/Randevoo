using Randevoo.Domain.Common;
using Randevoo.Domain.Common.Events;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Domain.Entities;

public class User : BaseEntity, IAggregateRoot
{

    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; }

  
    public UserProfile? Profile { get; private set; }

    private User() { } // EF Core

    public User(string email, string passwordHash)
    {
        Email = GuardAgainst.String.InvalidEmail(email, nameof(email)).Trim().ToLowerInvariant();
        PasswordHash = GuardAgainst.String.NullOrWhiteSpace(passwordHash, nameof(passwordHash));
        Role = UserRole.Basic;
        IsActive = true;

        // Profile will be created later via CreateProfile()

        AddDomainEvent(new EntityCreatedEvent<User>(this));
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
    }

    public void UpdatePassword(string newHash)
    {
        var oldPasswordHash = PasswordHash;
        PasswordHash = GuardAgainst.String.NullOrWhiteSpace(newHash, nameof(newHash));
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<User>(this, nameof(PasswordHash), oldPasswordHash, newHash));


    }

    public void Deactivate()
    {
        var oldIsActive = IsActive;
        IsActive = false;
        UpdateTimestamp();
        AddDomainEvent(new EntityUpdatedEvent<User>(this, nameof(IsActive), oldIsActive, IsActive));
    }

    public void ChangeUserRole(UserRole userRole)
    {
        var oldUserRole = Role;
        Role = userRole;
        UpdateTimestamp();
        AddDomainEvent(new EntityUpdatedEvent<User>(this, nameof(Role), oldUserRole, userRole));
    }
}
