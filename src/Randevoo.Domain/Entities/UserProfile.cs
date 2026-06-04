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
    public User User { get; private set; } = null!;


    public string DisplayName { get; private set; } = null!;
    public Gender Gender { get; private set; }
    public long? GenderId { get; private set; }
    public GenderLookup? GenderLookup { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public int BirthMonth { get; private set; }
    public string ZodiacSign { get; private set; } = null!;
    public Height Height { get; private set; } = null!;
    public EducationLevel EducationLevel { get; private set; }
    public long? EducationLevelId { get; private set; }
    public EducationLevelLookup? EducationLevelLookup { get; private set; }
    public bool Smoking { get; private set; }
   

    private readonly List<Interest> _interests = new();
    private readonly List<UserProfileImage> _images = new();
    public IReadOnlyList<Interest> Interests => _interests.AsReadOnly();
    public IReadOnlyList<UserProfileImage> Images => _images.AsReadOnly();
    internal ICollection<Interest> InterestsCollection => _interests;

    public Location Location { get; private set; } = null!;
    public long? CountryId { get; private set; }
    public Country? Country { get; private set; }
    public long? CityId { get; private set; }
    public City? City { get; private set; }

    public int Age => CalculateAge(DateOfBirth);



    // EF Core constructor
    private UserProfile()  
    {
        _interests = new List<Interest>();
        _images = new List<UserProfileImage>();
    }

    // Public constructor
    public UserProfile(
        User user,
        string displayName,
        DateOnly dateOfBirth,
        Gender gender,
        Location location,
        Height? height = null)  
    {
        GuardAgainst.Object.Null(user, nameof(user));
        _interests = new List<Interest>();
        _images = new List<UserProfileImage>();

        User = user;
        DisplayName = GuardAgainst.String.InvalidLength(displayName, nameof(displayName), 2, 50);
        Gender = GuardAgainst.Number.AgainstInvalidEnum<Gender>((int)gender, nameof(gender));
        GenderId = MapGenderId(gender);
        DateOfBirth = GuardAgainst.Date.AgeRequirement(dateOfBirth, 18, nameof(dateOfBirth));
        BirthMonth = DateOfBirth.Month;
        ZodiacSign = MapZodiacSign(DateOfBirth.Month, DateOfBirth.Day);
        Location = GuardAgainst.Object.Null(location, nameof(location));
        (CountryId, CityId) = MapLocationIds(location.Country, location.City);
        Height = height ?? new Height(170);

        EducationLevel = EducationLevel.NotSpecified;
        EducationLevelId = MapEducationLevelId(EducationLevel);
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
        (CountryId, CityId) = MapLocationIds(newLocation.Country, newLocation.City);
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
        EducationLevelId = MapEducationLevelId(EducationLevel);
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(EducationLevel), oldLevel, level));
    }

    public void UpdateGender(Gender gender)
    {
        var oldGender = Gender;

        Gender = GuardAgainst.Number.AgainstInvalidEnum<Gender>((int)gender, nameof(gender));
        GenderId = MapGenderId(Gender);
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(Gender), oldGender, gender));
    }

    public void UpdateDateOfBirth(DateOnly dateOfBirth)
    {
        var oldDateOfBirth = DateOfBirth;

        DateOfBirth = GuardAgainst.Date.AgeRequirement(dateOfBirth, 18, nameof(dateOfBirth));
        BirthMonth = DateOfBirth.Month;
        ZodiacSign = MapZodiacSign(DateOfBirth.Month, DateOfBirth.Day);
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(DateOfBirth), oldDateOfBirth, dateOfBirth));
    }

    public void SetSmoking(bool smokes)
    {
        var oldValue = Smoking;

        Smoking = smokes;
        UpdateTimestamp();

        AddDomainEvent(new EntityUpdatedEvent<UserProfile>(this, nameof(Smoking), oldValue, smokes));
    }

    public void UpdateProfile(
        string displayName,
        Gender gender,
        Location location,
        Height height,
        EducationLevel educationLevel,
        bool smoking)
    {
        UpdateDisplayName(displayName);
        UpdateGender(gender);
        UpdateLocation(location);
        UpdateHeight(height);
        UpdateEducationLevel(educationLevel);
        SetSmoking(smoking);
    }

    public void UpdateLookupReferences(long? countryId, long? cityId, long? educationLevelId, long? genderId)
    {
        CountryId = countryId;
        CityId = cityId;
        EducationLevelId = educationLevelId ?? MapEducationLevelId(EducationLevel);
        GenderId = genderId ?? MapGenderId(Gender);
        UpdateTimestamp();
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

    public UserProfileImage AddImage(string imageUrl, int displayOrder, bool isPrimary = false)
    {
        if (_images.Count >= 3)
            throw new BusinessRuleViolationException(
                "Maximum profile images exceeded",
                "User profile cannot have more than 3 images");

        if (_images.Any(image => image.DisplayOrder == displayOrder))
            throw new BusinessRuleViolationException(
                "Duplicate profile image order",
                $"Profile image display order '{displayOrder}' is already used");

        if (_images.Any(image => string.Equals(image.ImageUrl, imageUrl, StringComparison.OrdinalIgnoreCase)))
            throw new BusinessRuleViolationException(
                "Duplicate profile image",
                "This profile image already exists");

        if (_images.Count == 0)
            isPrimary = true;

        if (isPrimary)
        {
            foreach (var image in _images)
            {
                image.Update(image.ImageUrl, image.DisplayOrder, false);
            }
        }

        var profileImage = new UserProfileImage(this, imageUrl, displayOrder, isPrimary);
        _images.Add(profileImage);
        UpdateTimestamp();
        return profileImage;
    }

    public void RemoveInterest(Interest interest)
    {
        GuardAgainst.Object.Null(interest, nameof(interest));

        if (!_interests.Contains(interest))
            throw new BusinessRuleViolationException(
                "Interest not found",
                $"Interest '{interest.Name}' does not exist in user's interests");

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

    private static long? MapGenderId(Gender gender) => gender switch
    {
        Gender.Unknown => 1,
        Gender.Male => 2,
        Gender.Female => 3,
        _ => null
    };

    private static long? MapEducationLevelId(EducationLevel educationLevel) => educationLevel switch
    {
        EducationLevel.NotSpecified => 1,
        EducationLevel.Diploma => 2,
        EducationLevel.Undergraduate => 3,
        EducationLevel.Graduated => 3,
        EducationLevel.Postgraduate => 4,
        EducationLevel.PhD => 5,
        EducationLevel.PostDoc => 5,
        _ => null
    };

    private static (long? CountryId, long? CityId) MapLocationIds(string countryName, string cityName)
    {
        var countryId = countryName switch
        {
            "ایران" or "Iran" => 1L,
            "امارات متحده عربی" or "UAE" or "United Arab Emirates" => 2L,
            "ترکیه" or "Turkey" => 3L,
            _ => (long?)null
        };

        var cityId = (countryId, cityName) switch
        {
            (1, "تهران" or "Tehran") => 1L,
            (1, "مشهد" or "Mashhad") => 2L,
            (1, "شیراز" or "Shiraz") => 3L,
            (1, "اصفهان" or "Isfahan") => 4L,
            (1, "تبریز" or "Tabriz") => 5L,
            (2, "دبی" or "Dubai") => 6L,
            (2, "ابوظبی" or "Abu Dhabi") => 7L,
            (3, "استانبول" or "Istanbul") => 8L,
            (3, "آنکارا" or "Ankara") => 9L,
            _ => (long?)null
        };

        return (countryId, cityId);
    }

    private static string MapZodiacSign(int month, int day) => (month, day) switch
    {
        (3, >= 21) or (4, <= 19) => "Aries",
        (4, >= 20) or (5, <= 20) => "Taurus",
        (5, >= 21) or (6, <= 20) => "Gemini",
        (6, >= 21) or (7, <= 22) => "Cancer",
        (7, >= 23) or (8, <= 22) => "Leo",
        (8, >= 23) or (9, <= 22) => "Virgo",
        (9, >= 23) or (10, <= 22) => "Libra",
        (10, >= 23) or (11, <= 21) => "Scorpio",
        (11, >= 22) or (12, <= 21) => "Sagittarius",
        (12, >= 22) or (1, <= 19) => "Capricorn",
        (1, >= 20) or (2, <= 18) => "Aquarius",
        _ => "Pisces"
    };
}



 
