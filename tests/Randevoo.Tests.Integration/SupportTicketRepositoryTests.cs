using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;
using Randevoo.Infrastructure.Repositories;
using Xunit;

namespace Randevoo.Tests.Integration;

public class SupportTicketRepositoryTests
{
    [Fact]
    public async Task GetNextRoundRobinAssigneeAsync_AssignsActiveSupportUsersInSequence()
    {
        var options = new DbContextOptionsBuilder<RandevooDbContext>()
            .UseInMemoryDatabase($"support-round-robin-{Guid.NewGuid():N}")
            .Options;

        await using var db = new RandevooDbContext(options);
        var first = new User("+989130000001");
        first.ChangeUserRole(UserRole.PlatformSupportTeam);
        var second = new User("+989130000002");
        second.ChangeUserRole(UserRole.PlatformSupportTeam);
        var inactive = new User("+989130000003");
        inactive.ChangeUserRole(UserRole.PlatformSupportTeam);
        inactive.Deactivate();

        db.Users.AddRange(first, second, inactive);
        await db.SaveChangesAsync();

        var repository = new SupportTicketRepository(db);

        var assignedFirst = await repository.GetNextRoundRobinAssigneeAsync();
        await db.SaveChangesAsync();
        var assignedSecond = await repository.GetNextRoundRobinAssigneeAsync();
        await db.SaveChangesAsync();
        var assignedThird = await repository.GetNextRoundRobinAssigneeAsync();

        Assert.Equal(first.Id, assignedFirst?.Id);
        Assert.Equal(second.Id, assignedSecond?.Id);
        Assert.Equal(first.Id, assignedThird?.Id);
    }
}
