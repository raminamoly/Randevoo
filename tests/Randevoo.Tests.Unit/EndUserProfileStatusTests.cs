using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Xunit;

namespace Randevoo.Tests.Unit;

public class EndUserProfileStatusTests
{
    [Fact]
    public void NewProfile_StartsIncomplete_WhenEducationIsNotSpecified()
    {
        var profile = CreateProfile();

        Assert.Equal(UserProfileStatus.Incomplete, profile.ProfileStatus);
    }

    [Fact]
    public void UpdateEducationLevel_MarksProfileReadyToBuy()
    {
        var profile = CreateProfile();

        profile.UpdateEducationLevel(EducationLevel.Graduated);

        Assert.Equal(UserProfileStatus.ReadyToBuy, profile.ProfileStatus);
    }

    [Fact]
    public void AddInterest_MarksReadyProfileComplete()
    {
        var profile = CreateProfile();
        profile.UpdateEducationLevel(EducationLevel.Graduated);

        profile.AddInterest(new Interest("Music"));

        Assert.Equal(UserProfileStatus.Complete, profile.ProfileStatus);
    }

    private static UserProfile CreateProfile()
    {
        var user = new User("+989121111111");
        return new UserProfile(
            user,
            "End User",
            DateOnly.FromDateTime(DateTime.UtcNow).AddYears(-30),
            Gender.Female,
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            new Height(165));
    }
}
