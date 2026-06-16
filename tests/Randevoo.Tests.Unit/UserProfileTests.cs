using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;
using Xunit;

namespace Randevoo.Tests.Unit;

public class UserProfileTests
{
    private static User CreateUser()
    {
        return new User("+989121234567");
    }

    private static Location CreateLocation() =>
        new("USA", "Seattle", new Coordinates(47.6062m, -122.3321m));

    private static Interest CreateInterest(string name = "Hiking") =>
        new(name);

    private static UserProfile CreateValidProfile(DateOnly? dob = null) =>
        new(
            user: CreateUser(),
            displayName: "Alice",
            dateOfBirth: dob ?? DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30),
            gender: Gender.Female,
            location: CreateLocation(),
            height: new Height(165));

    [Fact]
    public void Constructor_WithValidData_SetsDefaultsAndAddsCreatedEvent()
    {
        var profile = CreateValidProfile();

        Assert.Equal("Alice", profile.DisplayName);
        Assert.Equal(Gender.Female, profile.Gender);
        Assert.Equal(EducationLevel.NotSpecified, profile.EducationLevel);
        Assert.False(profile.Smoking);
        Assert.NotNull(profile.Location);
        Assert.True(profile.Age >= 29);
        Assert.Contains(profile.DomainEvents, e => e is EntityCreatedEvent<UserProfile>);
    }

    [Fact]
    public void UpdateDisplayName_ValidName_UpdatesAndAddsEvent()
    {
        var profile = CreateValidProfile();
        profile.ClearDomainEvents();

        profile.UpdateDisplayName("Bob");

        Assert.Equal("Bob", profile.DisplayName);
        Assert.Contains(profile.DomainEvents, e => e is EntityUpdatedEvent<UserProfile>);
    }

    [Fact]
    public void AddInterest_NewInterest_AddsAndIncrementsUsageAndAddsEvent()
    {
        var profile = CreateValidProfile();
        var interest = CreateInterest("Hiking");
        profile.ClearDomainEvents();

        profile.AddInterest(interest);

        Assert.Contains(interest, profile.Interests);
        Assert.Equal(1, interest.UsageCount);
        Assert.Contains(profile.DomainEvents, e => e is InterestAddedEvent);
    }

    [Fact]
    public void AddInterest_Duplicate_ThrowsBusinessRuleViolationException()
    {
        var profile = CreateValidProfile();
        var interest = CreateInterest("Cooking");

        profile.AddInterest(interest);

        Assert.Throws<BusinessRuleViolationException>(() => profile.AddInterest(interest));
    }

    [Fact]
    public void AddInterest_ExceedMax_ThrowsBusinessRuleViolationException()
    {
        var profile = CreateValidProfile();

        for (var i = 0; i < 4; i++)
        {
            profile.AddInterest(CreateInterest($"I{i}"));
        }

        Assert.Throws<BusinessRuleViolationException>(() => profile.AddInterest(CreateInterest("Overflow")));
    }

    [Fact]
    public void ReplaceImages_WithThreeImages_SelectsRequestedPrimary()
    {
        var profile = CreateValidProfile();

        profile.ReplaceImages(
            [
                "/uploads/profiles/one.webp",
                "/uploads/profiles/two.webp",
                "/uploads/profiles/three.webp"
            ],
            "/uploads/profiles/two.webp");

        Assert.Equal(3, profile.Images.Count);
        Assert.Equal("/uploads/profiles/two.webp", profile.Images.Single(image => image.IsPrimary).ImageUrl);
    }

    [Fact]
    public void ReplaceImages_WithMoreThanThreeImages_ThrowsBusinessRuleViolationException()
    {
        var profile = CreateValidProfile();

        Assert.Throws<BusinessRuleViolationException>(() => profile.ReplaceImages(
            [
                "/uploads/profiles/one.webp",
                "/uploads/profiles/two.webp",
                "/uploads/profiles/three.webp",
                "/uploads/profiles/four.webp"
            ]));
    }

    [Fact]
    public void RemoveInterest_NotFound_ThrowsBusinessRuleViolationException()
    {
        var profile = CreateValidProfile();
        var interest = CreateInterest("NonExisting");

        Assert.Throws<BusinessRuleViolationException>(() => profile.RemoveInterest(interest));
    }

    [Fact]
    public void RemoveInterest_Existing_RemovesAndDecrementsUsageAndAddsEvent()
    {
        var profile = CreateValidProfile();
        var interest = CreateInterest("Travel");
        profile.AddInterest(interest);
        profile.ClearDomainEvents();

        profile.RemoveInterest(interest);

        Assert.DoesNotContain(interest, profile.Interests);
        Assert.Equal(0, interest.UsageCount);
        Assert.Contains(profile.DomainEvents, e => e is InterestRemovedEvent);
    }

    [Fact]
    public void SoftDelete_SetsIsDeletedAndAddsUpdatedEvent()
    {
        var profile = CreateValidProfile();
        profile.ClearDomainEvents();

        profile.SoftDelete();

        Assert.True(profile.IsDeleted);
        Assert.Contains(profile.DomainEvents, e => e is EntityUpdatedEvent<UserProfile>);
    }

    [Fact]
    public void Age_Calculation_IsConsistent()
    {
        var birth = new DateOnly(2000, 1, 1);
        var profile = CreateValidProfile(birth);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var expectedAge = today.Year - birth.Year;
        if (birth > today.AddYears(-expectedAge))
            expectedAge--;

        Assert.Equal(expectedAge, profile.Age);
    }

    [Fact]
    public void Constructor_WithNullUser_ThrowsException()
    {
        Assert.Throws<BusinessRuleViolationException>(() =>
            new UserProfile(
                user: null!,
                displayName: "Alice",
                dateOfBirth: DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30),
                gender: Gender.Female,
                location: CreateLocation()));
    }
}
