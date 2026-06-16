using FluentAssertions;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Enums;
using Xunit;

namespace Randevoo.Tests.Unit;

public class OperationPermissionCatalogTests
{
    [Fact]
    public void Catalog_Should_Not_Contain_Duplicate_Action_Keys()
    {
        var duplicates = OperationPermissionCatalog.All
            .GroupBy(item => $"{item.Entity}::{item.Action}", StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        duplicates.Should().BeEmpty();
    }

    [Fact]
    public void AdminPanelRoles_Should_Exclude_EndUsers()
    {
        OperationPermissionCatalog.AdminPanelRoles.Should().BeEquivalentTo(
        [
            UserRole.Admin,
            UserRole.EventPlanner,
            UserRole.PlatformSupportTeam
        ]);

        OperationPermissionCatalog.AdminPanelRoles.Should().NotContain(UserRole.EndUser);
    }

    [Fact]
    public void Catalog_Should_Define_Core_Operation_Permission_Page_Actions()
    {
        OperationPermissionCatalog.Find("operationPermissions", "view").Should().NotBeNull();
        OperationPermissionCatalog.Find("operationPermissions", "manage").Should().NotBeNull();
        OperationPermissionCatalog.Find("participants", "emergencyRefund").Should().NotBeNull();
        OperationPermissionCatalog.Find("withdrawals", "confirm").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "view").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "cancelTicketRefundToWallet").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "manualIssueTicketWithWalletDebit").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "manualWalletCredit").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "manualWalletDebit").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "userReportsView").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "userReportsReview").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "userReportsRestrictTicketPurchase").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "userReportsRemoveRestriction").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "userReportsSendWarning").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "userReportsSendNotification").Should().NotBeNull();
        OperationPermissionCatalog.Find("specialOperations", "userReportsDeactivateUser").Should().NotBeNull();
    }
}
