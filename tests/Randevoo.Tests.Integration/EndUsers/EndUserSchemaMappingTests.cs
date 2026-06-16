using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;
using Xunit;

namespace Randevoo.Tests.Integration.EndUsers;

public class EndUserSchemaMappingTests
{
    [Fact]
    public async Task EndUserStageOneEntities_CanBePersisted()
    {
        var options = new DbContextOptionsBuilder<RandevooDbContext>()
            .UseInMemoryDatabase($"end-user-stage-one-{Guid.NewGuid():N}")
            .Options;

        await using var db = new RandevooDbContext(options);
        var interest = new Interest("Music");
        var tag = new Tag("Concert");
        var mapping = new InterestTagMapping(interest, tag, 80);
        var datingEvent = CreateEvent();
        var status = new UserFacingEventStatus(datingEvent, UserFacingEventStatusKind.SaleClosed, DateTime.UtcNow);

        db.AddRange(interest, tag, mapping, datingEvent, status);
        await db.SaveChangesAsync();

        Assert.Equal(1, await db.InterestTagMappings.CountAsync());
        Assert.Equal(1, await db.UserFacingEventStatuses.CountAsync());
    }

    private static DatingEvent CreateEvent()
    {
        var planner = new User("+989129990001");
        planner.ChangeUserRole(UserRole.EventPlanner);

        return new DatingEvent(
            planner,
            "Mapped event",
            new Location("Iran", "Tehran", new Coordinates(35.6895m, 51.3890m)),
            "Main venue",
            DateTime.UtcNow.AddDays(3),
            DateTime.UtcNow.AddDays(3).AddHours(3),
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
