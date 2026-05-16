// Domain/Entities/UserProfile.cs
using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Domain.Entities;

public class UserProfile : BaseEntity , IAggregateRoot
{
    public long UserId { get; private set; }
    public User User { get; private set; }


    public string DisplayName { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public Height Height { get; private set; }
    public EducationLevel EducationLevel { get; private set; }
    public bool Smoking { get; private set; }
    //public IReadOnlyList<Interest> Interests => _interests.AsReadOnly();
    //private readonly List<Interest> _interests;

    private readonly List<Interest> _interests = new();
    public IReadOnlyList<Interest> Interests => _interests.AsReadOnly();
    internal ICollection<Interest> InterestsCollection => _interests;

    public Location Location { get; private set; }

    public int Age => CalculateAge(DateOfBirth);



    // EF Core constructor
    private UserProfile() : base()
    {
        _interests = new List<Interest>();
    }

    // Public constructor
    public UserProfile(
        User user,
        string displayName,
        DateOnly dateOfBirth,
        Gender gender,
        Location location,
        Height? height = null) : base()
    {
        _interests = new List<Interest>();

        DisplayName = GuardAgainst.String.InvalidLength(displayName, nameof(displayName), 2, 50);
        Gender = GuardAgainst.Number.AgainstInvalidEnum<Gender>((int)gender, nameof(gender));
        DateOfBirth = GuardAgainst.Date.AgeRequirement(dateOfBirth, 18, nameof(dateOfBirth));
        Location = GuardAgainst.Object.Null(location, nameof(location));
        Height = height ?? new Height(170);

        EducationLevel = EducationLevel.NotSpecified;
        Smoking = false;

        // Add domain event for creation
        AddDomainEvent(new EntityCreatedEvent<UserProfile>(this));
    }

    // Behavior methods with domain events
    public void UpdateDisplayName(string newName)
    {
        var oldName = DisplayName;

        DisplayName = GuardAgainst.String.InvalidLength(newName, nameof(newName), 2, 50);
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(DisplayName), oldName, newName));
    }

    public void UpdateLocation(Location newLocation)
    {
        var oldLocation = Location;

        Location = GuardAgainst.Object.Null(newLocation, nameof(newLocation));
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(Location), oldLocation, newLocation));
    }

    public void UpdateHeight(Height newHeight)
    {
        var oldHeight = Height;

        Height = GuardAgainst.Object.Null(newHeight, nameof(newHeight));
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(Height), oldHeight, newHeight));
    }

    public void UpdateEducationLevel(EducationLevel level)
    {
        var oldLevel = EducationLevel;

        EducationLevel = GuardAgainst.Number.AgainstInvalidEnum<EducationLevel>((int)level, nameof(level));
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(EducationLevel), oldLevel, level));
    }

    public void UpdateGender(Gender gender)
    {
        var oldGender = Gender;

        Gender = GuardAgainst.Number.AgainstInvalidEnum<Gender>((int)gender, nameof(gender));
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(Gender), oldGender, gender));
    }

    public void SetSmoking(bool smokes)
    {
        var oldValue = Smoking;

        Smoking = smokes;
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(Smoking), oldValue, smokes));
    }

    public void AddInterest(Interest interest)
    {
        GuardAgainst.Object.Null(interest, nameof(interest));

        if (_interests.Count >= 10)
            throw new BusinessRuleViolationException(
                "Maximum interests exceeded",
                "User cannot have more than 10 interests");

        if (_interests.Any(i => i == interest))
            throw new BusinessRuleViolationException(
                "Duplicate interest",
                $"Interest '{interest.Name}' already added");

        _interests.Add(interest);
        interest.IncrementUsage(); // Track popularity

        UpdateTimestamp();

        AddDomainEvent(new InterestAddedEvent(this, interest));
    }

    public void RemoveInterest(Interest interest)
    {
        GuardAgainst.Object.Null(interest, nameof(interest));

        if (!_interests.Contains(interest))
            throw new BusinessRuleViolationException(" not found",
                "Interest not found"
                );

        _interests.Remove(interest);
        interest.DecrementUsage();
        UpdateTimestamp();

        AddDomainEvent(new InterestRemovedEvent(this, interest));
    }

    // Override soft delete to add specific event
    public override void SoftDelete()
    {
        base.SoftDelete();
        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(IsDeleted), false, true));
    }

    private static int CalculateAge(DateOnly birthDate)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - birthDate.Year;
        if (birthDate > today.AddYears(-age)) age--;
        return age;
    }
}



 
