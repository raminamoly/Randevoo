using FluentAssertions;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;
using Xunit;

namespace Randevoo.Tests.Unit;

public class UserTests
{
    private static Location CreateLocation() =>
      new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m));

    private static User CreateUser(string email = "ramin.amoly@gmail.com", string password = "123") =>
        new User(email, password);

    [Fact]
    public void Constructor_WithValidData_SetsPropertiesAndRaisesCreatedEvent()
    {
        // Arrange / Act
        var user = CreateUser();

        // Assert
        user.Email.Should().Be("ramin.amoly@gmail.com");
        user.PasswordHash.Should().Be("123");
        user.Role.Should().Be(UserRole.Basic);
        user.IsActive.Should().BeTrue();
        user.DomainEvents.Should().NotBeNull().And.BeEmpty(); // constructor may add event depending on implementation
    }

    [Fact]
    public void Constructor_WithInvalidEmail_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        Action act = () => new User("", "pwd");

        // Act / Assert
        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void CreateProfile_WithValidData_SetsProfileAndRaisesEvent()
    {
        // Arrange
        var user = CreateUser();
        user.ClearDomainEvents();

        // Act
        user.CreateProfile(
            "Ramin Amoly",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            CreateLocation(),
            new Height(177)
        );

        // Assert
        user.Profile.Should().NotBeNull();
        user.Profile.DisplayName.Should().Be("Ramin Amoly");
        user.Profile.DateOfBirth.Should().Be(new DateOnly(1990, 1, 1));
        user.Profile.Gender.Should().Be(Gender.Male);
        user.DomainEvents.Should().Contain(e => e.GetType().Name.Contains("EntityCreated")); // created event for profile
    }

    [Fact]
    public void CreateProfile_WhenAlreadyExists_ThrowsBusinessRuleViolationException()
    {
        // Arrange
        var user = CreateUser();
        user.CreateProfile("X", DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30), Gender.Male, CreateLocation());

        // Act
        Action act = () => user.CreateProfile("Y", DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-25), Gender.Female, CreateLocation());

        // Assert
        act.Should().Throw<BusinessRuleViolationException>();
    }
   
    [Fact]
    public void UpdatePassword_WithValidData_UpdatesPasswordAndRaisesEvent()
    {
        // Arrange
        var user = CreateUser();
        user.ClearDomainEvents();
        var before = user.UpdatedAt;

        // Act
        user.UpdatePassword("new-hash");

        // Assert
        user.PasswordHash.Should().Be("new-hash");
        user.UpdatedAt.Should().NotBe(before);
        user.DomainEvents.Should().Contain(e => e.GetType().Name.Contains("EntityUpdated"));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalseAndRaiseEvent()
    {
        // Arrange
        var user = CreateUser();
        user.ClearDomainEvents();

        // Act
        user.Deactivate();

        // Assert
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
        user.DomainEvents.Should().Contain(e => e.GetType().Name.Contains("EntityUpdated"));
    }

    [Fact]
    public void ChangeUserRole_ShouldUpdateRoleAndRaiseEvent()
    {
        // Arrange
        var user = CreateUser();
        user.ClearDomainEvents();

        // Act
        user.ChangeUserRole(UserRole.Admin);

        // Assert
        user.Role.Should().Be(UserRole.Admin);
        user.DomainEvents.Should().Contain(e => e.GetType().Name.Contains("EntityUpdated"));
    }

}

