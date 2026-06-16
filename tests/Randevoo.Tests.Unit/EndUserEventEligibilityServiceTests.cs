using Randevoo.Application.EndUsers.Events;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Xunit;

namespace Randevoo.Tests.Unit;

public class EndUserEventEligibilityServiceTests
{
    private readonly EndUserEventEligibilityService _service = new();

    [Fact]
    public void Evaluate_AllowsReadyProfile_WhenAgeAndEducationMatch()
    {
        var now = DateTime.UtcNow;
        var profile = CreateProfile(EducationLevel.Graduated, Gender.Male, new DateOnly(1991, 1, 1));
        var datingEvent = CreateOpenEvent(now.AddDays(2), EventEducationLevelRestriction.BachelorOrHigher);

        var result = _service.Evaluate(profile, datingEvent, now);

        Assert.True(result.CanBuyTicket);
        Assert.Equal("eligible", result.ReasonCode);
    }

    [Fact]
    public void Evaluate_BlocksIncompleteProfile()
    {
        var now = DateTime.UtcNow;
        var profile = CreateProfile(EducationLevel.NotSpecified, Gender.Male, new DateOnly(1991, 1, 1));
        var datingEvent = CreateOpenEvent(now.AddDays(2), EventEducationLevelRestriction.WithoutLimit);

        var result = _service.Evaluate(profile, datingEvent, now);

        Assert.False(result.CanBuyTicket);
        Assert.Equal("profile_incomplete", result.ReasonCode);
    }

    [Fact]
    public void Evaluate_BlocksEducationMismatch()
    {
        var now = DateTime.UtcNow;
        var profile = CreateProfile(EducationLevel.Diploma, Gender.Female, new DateOnly(1993, 1, 1));
        var datingEvent = CreateOpenEvent(now.AddDays(2), EventEducationLevelRestriction.MasterOrHigher);

        var result = _service.Evaluate(profile, datingEvent, now);

        Assert.False(result.CanBuyTicket);
        Assert.Equal("education_not_allowed", result.ReasonCode);
    }

    private static UserProfile CreateProfile(EducationLevel educationLevel, Gender gender, DateOnly dateOfBirth)
    {
        var user = new User($"+98912{Math.Abs(Guid.NewGuid().GetHashCode()) % 10000000:0000000}");
        user.CreateProfile(
            "Test User",
            dateOfBirth,
            gender,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(175));
        user.Profile!.UpdateEducationLevel(educationLevel);
        return user.Profile;
    }

    private static DatingEvent CreateOpenEvent(DateTime startsAtUtc, EventEducationLevelRestriction educationRestriction)
    {
        var planner = new User($"+98913{Math.Abs(Guid.NewGuid().GetHashCode()) % 10000000:0000000}");
        planner.ChangeUserRole(UserRole.EventPlanner);

        var datingEvent = new DatingEvent(
            planner,
            "Eligibility event",
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            "Main venue",
            startsAtUtc,
            startsAtUtc.AddHours(3),
            new EventType("Social"),
            new AgeRange(25, 45),
            new AgeRange(25, 45),
            10,
            10,
            3,
            100m,
            100m,
            educationRestriction,
            null,
            null,
            null,
            null,
            "<p>Test event description.</p>");

        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();
        return datingEvent;
    }
}
