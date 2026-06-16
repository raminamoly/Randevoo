using Randevoo.Application.EndUsers.Events;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Xunit;

namespace Randevoo.Tests.Unit;

public class UserFacingEventStatusResolverTests
{
    private readonly UserFacingEventStatusResolver _resolver = new();

    [Fact]
    public void Resolve_ReturnsSaleOpen_ForApprovedOpenFutureEvent()
    {
        var now = DateTime.UtcNow;
        var datingEvent = CreateEvent(now.AddDays(3), now.AddDays(3).AddHours(3));
        datingEvent.ApproveByAdmin();
        datingEvent.OpenForSell();

        var status = _resolver.Resolve(datingEvent, now);

        Assert.Equal(UserFacingEventStatusKind.SaleOpen, status);
    }

    [Fact]
    public void Resolve_ReturnsParticipantProfilesOpen_WithinConfiguredWindow()
    {
        var now = DateTime.UtcNow;
        var datingEvent = CreateEvent(now.AddHours(12), now.AddHours(16));

        var status = _resolver.Resolve(datingEvent, now);

        Assert.Equal(UserFacingEventStatusKind.ParticipantProfilesOpen, status);
    }

    [Fact]
    public void Resolve_ReturnsLikeWindowOpen_AfterEventEndsBeforeWindowCloses()
    {
        var now = DateTime.UtcNow;
        var datingEvent = CreateEvent(now.AddHours(-4), now.AddHours(-1));

        var status = _resolver.Resolve(datingEvent, now);

        Assert.Equal(UserFacingEventStatusKind.LikeWindowOpen, status);
    }

    private static DatingEvent CreateEvent(DateTime startAtUtc, DateTime endAtUtc)
    {
        var planner = new User($"+989120{Math.Abs(startAtUtc.GetHashCode()) % 1000000:000000}");
        planner.ChangeUserRole(UserRole.EventPlanner);

        return new DatingEvent(
            planner,
            "Resolver event",
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            "Main venue",
            startAtUtc,
            endAtUtc,
            new EventType("Social"),
            new AgeRange(18, 45),
            new AgeRange(18, 45),
            10,
            10,
            3,
            100m,
            100m,
            EventEducationLevelRestriction.WithoutLimit,
            null,
            null,
            null,
            null,
            "<p>Test event description.</p>");
    }
}
