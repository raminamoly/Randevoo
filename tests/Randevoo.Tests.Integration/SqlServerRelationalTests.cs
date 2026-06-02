using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Infrastructure.Data;
using Testcontainers.MsSql;
using Xunit;

namespace Randevoo.Tests.Integration;

public class SqlServerRelationalTests
{
    [Fact]
    public async Task SqlServer_EnforcesUniqueMobileNumber()
    {
        if (!string.Equals(Environment.GetEnvironmentVariable("RUN_SQLSERVER_TESTCONTAINERS"), "true", StringComparison.OrdinalIgnoreCase))
            return;

        await using var container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();
        await container.StartAsync();

        var options = new DbContextOptionsBuilder<RandevooDbContext>()
            .UseSqlServer(container.GetConnectionString())
            .Options;

        await using var db = new RandevooDbContext(options);
        await db.Database.MigrateAsync();

        db.Users.Add(new User("+989121111111"));
        db.Users.Add(new User("+989121111111"));

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}
