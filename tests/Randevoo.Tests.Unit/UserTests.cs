using FluentAssertions;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Events;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.ValueObjects;
using Randevoo.Tests.Unit.Builder;
using Xunit;

namespace Randevoo.Tests.Unit;

public class UserTests
{
    private readonly UserBuilder _userBuilder = new();

    private static Location CreateLocation() =>
        new("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m));

    [Fact]
    public void Constructor_WithValidMobileNumber_SetsPropertiesAndRaisesCreatedEvent()
    {
        var user = _userBuilder.Build();

        user.MobileNumber.Should().Be("+989121234567");
        user.Email.Should().BeNull();
        user.IsEmailConfirmed.Should().BeFalse();
        user.Role.Should().Be(UserRole.EndUser);
        user.IsActive.Should().BeTrue();
        user.DomainEvents.Should().Contain(e => e is EntityCreatedEvent<User>);
    }

    [Fact]
    public void Constructor_WithInvalidMobileNumber_ThrowsBusinessRuleViolationException()
    {
        Action act = () => _userBuilder.WithMobileNumber("pwd").Build();

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void MobileLogin_WithValidCode_CompletesAndClearsCode()
    {
        var user = _userBuilder.Build();

        user.StartMobileLogin("hash", DateTime.UtcNow.AddMinutes(5));
        user.CompleteMobileLogin("hash", DateTime.UtcNow);

        user.MobileLoginCodeHash.Should().BeNull();
        user.MobileLoginCodeExpiresAt.Should().BeNull();
    }

    [Fact]
    public void MobileLogin_WithWrongCode_ThrowsBusinessRuleViolationException()
    {
        var user = _userBuilder.Build();
        user.StartMobileLogin("hash", DateTime.UtcNow.AddMinutes(5));

        Action act = () => user.CompleteMobileLogin("wrong", DateTime.UtcNow);

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void ConfirmEmail_WithValidToken_SetsConfirmedEmail()
    {
        var user = _userBuilder.Build();

        user.StartEmailConfirmation("Ramin.Amoly@gmail.com", "token-hash", DateTime.UtcNow.AddHours(1));
        user.ConfirmEmail("token-hash", DateTime.UtcNow);

        user.Email.Should().Be("ramin.amoly@gmail.com");
        user.IsEmailConfirmed.Should().BeTrue();
        user.PendingEmail.Should().BeNull();
        user.EmailConfirmationTokenHash.Should().BeNull();
    }

    [Fact]
    public void CreateProfile_WithValidData_SetsProfileAndRaisesEvent()
    {
        var user = _userBuilder.Build();
        user.ClearDomainEvents();

        user.CreateProfile(
            "Ramin Amoly",
            new DateOnly(1990, 1, 1),
            Gender.Male,
            CreateLocation(),
            new Height(177));

        user.Profile.Should().NotBeNull();
        user.Profile!.DisplayName.Should().Be("Ramin Amoly");
        user.Profile.DateOfBirth.Should().Be(new DateOnly(1990, 1, 1));
        user.Profile.Gender.Should().Be(Gender.Male);
        user.Profile.DomainEvents.Should().Contain(e => e is EntityCreatedEvent<UserProfile>);
    }

    [Fact]
    public void CreateProfile_WhenAlreadyExists_ThrowsBusinessRuleViolationException()
    {
        var user = _userBuilder.Build();
        user.CreateProfile("First Profile", DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30), Gender.Male, CreateLocation());

        Action act = () => user.CreateProfile("Second Profile", DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-25), Gender.Female, CreateLocation());

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalseAndRaiseEvent()
    {
        var user = _userBuilder.Build();
        user.ClearDomainEvents();

        user.Deactivate();

        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
        user.DomainEvents.Should().Contain(e => e.GetType().Name.Contains("EntityUpdated"));
    }

    [Fact]
    public void ChangeUserRole_ShouldUpdateRoleAndRaiseEvent()
    {
        var user = _userBuilder.Build();
        user.ClearDomainEvents();

        user.ChangeUserRole(UserRole.Admin);

        user.Role.Should().Be(UserRole.Admin);
        user.DomainEvents.Should().Contain(e => e.GetType().Name.Contains("EntityUpdated"));
    }
}
