extern alias AdminPanel;

using Microsoft.EntityFrameworkCore;
using AdminPanel::Randevoo.AdminPanel.Models.Auth;
using AdminPanel::Randevoo.AdminPanel.Models.Finance;
using AdminPanel::Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Xunit;

namespace Randevoo.Tests.Integration.AdminPanel;

public class DatabaseFinanceApiClientRefundTests
{
    [Fact]
    public async Task ApproveTicketRefund_CreditsBuyerWallet_RefundsTicket_AndCreatesNotification()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var admin = await AdminPanelTestData.CreateUserAsync(db, "+989121100001", UserRole.Admin, "Admin user");
        var planner = await AdminPanelTestData.CreateUserAsync(db, "+989121100002", UserRole.EventPlanner, "Planner user");
        var buyer = await AdminPanelTestData.CreateUserAsync(db, "+989121100003", UserRole.EndUser, "Buyer user");
        var participant = await AdminPanelTestData.CreateUserAsync(db, "+989121100004", UserRole.EndUser, "Participant user", Gender.Female);
        var datingEvent = await AdminPanelTestData.CreateApprovedOpenEventAsync(db, planner, "Refund integration event");
        var ticket = await AdminPanelTestData.SellTicketAsync(db, datingEvent, buyer, participant, 100m);
        var service = new DatabaseFinanceApiClient(db);

        await service.RequestTicketRefundAsync(AdminPanelTestData.AsMockUser(planner, AdminRole.EventPlanner), ticket.Id, "Participant cannot attend.");
        var requestId = await db.TicketRefundRequests.Select(item => item.Id).SingleAsync();

        await service.ApproveTicketRefundRequestAsync(
            AdminPanelTestData.AsMockUser(admin, AdminRole.Admin),
            requestId,
            new TicketRefundReviewInput { ApprovedAmount = 75m, ReviewNote = "Partial refund approved." });

        var request = await db.TicketRefundRequests.SingleAsync();
        var refreshedTicket = await db.EventTickets.SingleAsync();
        var refreshedOrder = await db.TicketOrders.SingleAsync();
        var buyerAccount = await db.BalanceAccounts.Include(item => item.Transactions).SingleAsync(item => item.UserId == buyer.Id);
        var notification = await db.Notifications.Include(item => item.Recipients).SingleAsync(item => item.ReferenceType == nameof(TicketRefundRequest));

        Assert.Equal(TicketRefundRequestStatus.Approved, request.Status);
        Assert.Equal(75m, request.ApprovedAmount);
        Assert.True(refreshedTicket.IsRefunded);
        Assert.Equal(TicketOrderPaymentStatus.Refunded, refreshedOrder.PaymentStatus);
        Assert.Equal(75m, buyerAccount.Balance);
        Assert.Contains(buyerAccount.Transactions, item => item.Type == BalanceTransactionType.TicketRefund && item.Amount == 75m);
        Assert.Equal(NotificationType.Refund, notification.Type);
        Assert.Contains(notification.Recipients, item => item.RecipientUserId == buyer.Id && item.Status == NotificationRecipientStatus.Delivered);
    }

    [Fact]
    public async Task EventPlanner_CanSeeOwnRefundRequests_ButCannotApproveThem()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var ownerPlanner = await AdminPanelTestData.CreateUserAsync(db, "+989121100011", UserRole.EventPlanner, "Owner planner");
        var otherPlanner = await AdminPanelTestData.CreateUserAsync(db, "+989121100012", UserRole.EventPlanner, "Other planner");
        var buyer = await AdminPanelTestData.CreateUserAsync(db, "+989121100013", UserRole.EndUser, "Buyer");
        var participant = await AdminPanelTestData.CreateUserAsync(db, "+989121100014", UserRole.EndUser, "Participant", Gender.Female);
        var ownedEvent = await AdminPanelTestData.CreateApprovedOpenEventAsync(db, ownerPlanner, "Owned refund event");
        var otherEvent = await AdminPanelTestData.CreateApprovedOpenEventAsync(db, otherPlanner, "Other refund event");
        var ownedTicket = await AdminPanelTestData.SellTicketAsync(db, ownedEvent, buyer, participant, 100m);
        var otherTicket = await AdminPanelTestData.SellTicketAsync(db, otherEvent, buyer, ownerPlanner, 100m);
        var service = new DatabaseFinanceApiClient(db);

        await service.RequestTicketRefundAsync(AdminPanelTestData.AsMockUser(ownerPlanner, AdminRole.EventPlanner), ownedTicket.Id, "Owner event refund.");
        await service.RequestTicketRefundAsync(AdminPanelTestData.AsMockUser(otherPlanner, AdminRole.EventPlanner), otherTicket.Id, "Other event refund.");

        var visibleToOwner = await service.GetTicketRefundRequestsAsync(AdminPanelTestData.AsMockUser(ownerPlanner, AdminRole.EventPlanner));
        var ownerRequestId = visibleToOwner.Single().Id;
        var approveAction = () => service.ApproveTicketRefundRequestAsync(
            AdminPanelTestData.AsMockUser(ownerPlanner, AdminRole.EventPlanner),
            ownerRequestId,
            new TicketRefundReviewInput { ApprovedAmount = 100m, ReviewNote = "Trying as planner." });

        Assert.Single(visibleToOwner);
        Assert.Equal(ownedEvent.Id, visibleToOwner[0].EventId);
        await Assert.ThrowsAsync<InvalidOperationException>(approveAction);
    }

    [Fact]
    public async Task RejectTicketRefund_DoesNotCreditWallet_AndNotifiesBuyer()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var support = await AdminPanelTestData.CreateUserAsync(db, "+989121100021", UserRole.PlatformSupportTeam, "Support user");
        var planner = await AdminPanelTestData.CreateUserAsync(db, "+989121100022", UserRole.EventPlanner, "Planner user");
        var buyer = await AdminPanelTestData.CreateUserAsync(db, "+989121100023", UserRole.EndUser, "Buyer user");
        var datingEvent = await AdminPanelTestData.CreateApprovedOpenEventAsync(db, planner, "Rejected refund event");
        var ticket = await AdminPanelTestData.SellTicketAsync(db, datingEvent, buyer, buyer, 100m);
        var service = new DatabaseFinanceApiClient(db);

        await service.RequestTicketRefundAsync(AdminPanelTestData.AsMockUser(planner, AdminRole.EventPlanner), ticket.Id, "Refund requested.");
        var requestId = await db.TicketRefundRequests.Select(item => item.Id).SingleAsync();

        await service.RejectTicketRefundRequestAsync(AdminPanelTestData.AsMockUser(support, AdminRole.SupportTeam), requestId, "Refund policy does not allow this.");

        var request = await db.TicketRefundRequests.SingleAsync();
        var refreshedTicket = await db.EventTickets.SingleAsync();
        var notification = await db.Notifications.SingleAsync(item => item.ReferenceType == nameof(TicketRefundRequest));

        Assert.Equal(TicketRefundRequestStatus.Rejected, request.Status);
        Assert.False(refreshedTicket.IsRefunded);
        Assert.False(await db.BalanceAccounts.AnyAsync(item => item.UserId == buyer.Id));
        Assert.Equal("درخواست بازگشت وجه رد شد", notification.Title);
    }
}
