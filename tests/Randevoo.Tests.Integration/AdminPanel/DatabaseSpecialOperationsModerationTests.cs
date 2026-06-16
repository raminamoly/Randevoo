extern alias AdminPanel;

using Microsoft.EntityFrameworkCore;
using AdminPanel::Randevoo.AdminPanel.Models.Auth;
using AdminPanel::Randevoo.AdminPanel.Models.SpecialOperations;
using AdminPanel::Randevoo.AdminPanel.Services.ApiClients;
using AdminPanel::Randevoo.AdminPanel.Services.Permissions;
using Randevoo.Application.Interfaces.Currencies;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;
using Xunit;

namespace Randevoo.Tests.Integration.AdminPanel;

public class DatabaseSpecialOperationsModerationTests
{
    [Fact]
    public async Task ReportedUsers_AreSortedByOpenReportCount()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var admin = await AdminPanelTestData.CreateUserAsync(db, "+989123300001", UserRole.Admin, "Admin");
        var reportedMany = await AdminPanelTestData.CreateUserAsync(db, "+989123300002", UserRole.EndUser, "Reported Many");
        var reportedOne = await AdminPanelTestData.CreateUserAsync(db, "+989123300003", UserRole.EndUser, "Reported One");
        var reporter1 = await AdminPanelTestData.CreateUserAsync(db, "+989123300004", UserRole.EndUser, "Reporter One");
        var reporter2 = await AdminPanelTestData.CreateUserAsync(db, "+989123300005", UserRole.EndUser, "Reporter Two");

        db.ModerationReports.Add(new ModerationReport(reporter1, reportedOne, ModerationReportReason.Spam, "A single pending sample report."));
        db.ModerationReports.Add(new ModerationReport(reporter1, reportedMany, ModerationReportReason.Harassment, "First pending sample report."));
        db.ModerationReports.Add(new ModerationReport(reporter2, reportedMany, ModerationReportReason.UnsafeBehavior, "Second pending sample report."));
        await db.SaveChangesAsync();

        var service = CreateService(db);

        var result = await service.ListReportedUsersAsync(
            AdminPanelTestData.AsMockUser(admin, AdminRole.Admin),
            new UserReportListFilter());

        Assert.Equal(reportedMany.Id, result.Items.First().UserId);
        Assert.Equal(2, result.Items.First().OpenReports);
        Assert.Equal(reportedOne.Id, result.Items.Skip(1).First().UserId);
    }

    [Fact]
    public async Task RestrictTicketPurchase_CreatesRestrictionNotificationAndOperationLog()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var admin = await AdminPanelTestData.CreateUserAsync(db, "+989123300011", UserRole.Admin, "Admin");
        var reported = await AdminPanelTestData.CreateUserAsync(db, "+989123300012", UserRole.EndUser, "Reported");
        var reporter = await AdminPanelTestData.CreateUserAsync(db, "+989123300013", UserRole.EndUser, "Reporter");
        db.ModerationReports.Add(new ModerationReport(reporter, reported, ModerationReportReason.Harassment, "A pending sample report for restriction."));
        await db.SaveChangesAsync();

        var service = CreateService(db);

        var result = await service.ExecuteRestrictTicketPurchaseAsync(
            AdminPanelTestData.AsMockUser(admin, AdminRole.Admin),
            new RestrictTicketPurchaseInput
            {
                UserId = reported.Id,
                Reason = "Repeated reports require temporary ticket purchase restriction.",
                IdempotencyKey = Guid.NewGuid().ToString("N")
            });

        Assert.False(result.AlreadyApplied);
        Assert.True(await db.UserRestrictions.AnyAsync(item =>
            item.UserId == reported.Id
            && item.RestrictionType == UserRestrictionType.TicketPurchase
            && item.IsActive));
        Assert.True(await db.NotificationRecipients.AnyAsync(item => item.RecipientUserId == reported.Id));
        Assert.True(await db.SpecialOperationLogs.AnyAsync(item =>
            item.TargetUserId == reported.Id
            && item.OperationType == "UserTicketPurchaseRestricted"
            && item.Status == "Succeeded"));
    }

    [Fact]
    public async Task DeactivateReportedUser_DeactivatesUserCreatesNotificationAndOperationLog()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var admin = await AdminPanelTestData.CreateUserAsync(db, "+989123300021", UserRole.Admin, "Admin");
        var reported = await AdminPanelTestData.CreateUserAsync(db, "+989123300022", UserRole.EndUser, "Reported");
        var reporter = await AdminPanelTestData.CreateUserAsync(db, "+989123300023", UserRole.EndUser, "Reporter");
        db.ModerationReports.Add(new ModerationReport(reporter, reported, ModerationReportReason.UnsafeBehavior, "A pending sample report for deactivation."));
        await db.SaveChangesAsync();

        var service = CreateService(db);

        var result = await service.DeactivateReportedUserAsync(
            AdminPanelTestData.AsMockUser(admin, AdminRole.Admin),
            new DeactivateReportedUserInput
            {
                UserId = reported.Id,
                Reason = "Multiple moderation reports require account deactivation.",
                NotificationMessage = "Your account has been disabled by support after moderation review.",
                IdempotencyKey = Guid.NewGuid().ToString("N")
            });

        Assert.False(result.AlreadyApplied);
        Assert.False((await db.Users.AsNoTracking().FirstAsync(item => item.Id == reported.Id)).IsActive);
        Assert.True(await db.NotificationRecipients.AnyAsync(item => item.RecipientUserId == reported.Id));
        Assert.True(await db.SpecialOperationLogs.AnyAsync(item =>
            item.TargetUserId == reported.Id
            && item.OperationType == "ReportedUserDeactivated"
            && item.Status == "Succeeded"));
    }

    private static DatabaseSpecialOperationsApiClient CreateService(RandevooDbContext db)
        => new(db, new AllowAllPermissions(), new FixedExchangeRateProvider());

    private sealed class AllowAllPermissions : IOperationPermissionService
    {
        public Task<IReadOnlySet<string>> GetAllowedActionsAsync(MockUser user, string entity, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlySet<string>>(new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "view",
                "viewAuditLog",
                "userReportsView",
                "userReportsReview",
                "userReportsRestrictTicketPurchase",
                "userReportsRemoveRestriction",
                "userReportsSendWarning",
                "userReportsSendNotification",
                "userReportsDeactivateUser"
            });

        public Task<bool> IsAllowedAsync(MockUser user, string entity, string action, CancellationToken cancellationToken = default)
            => Task.FromResult(true);
    }

    private sealed class FixedExchangeRateProvider : ICurrencyExchangeRateProvider
    {
        public Task<CurrencyExchangeRateSnapshot> GetActiveRateToIrrAsync(string currencyCode, DateTime atUtc, CancellationToken cancellationToken = default)
            => Task.FromResult(new CurrencyExchangeRateSnapshot(1, currencyCode, "IRR", 1m, atUtc));
    }
}
