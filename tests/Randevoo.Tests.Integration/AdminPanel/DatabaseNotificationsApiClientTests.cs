extern alias AdminPanel;

using Microsoft.EntityFrameworkCore;
using AdminPanel::Randevoo.AdminPanel.Models.Auth;
using AdminPanel::Randevoo.AdminPanel.Models.Notifications;
using AdminPanel::Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.Domain.Enums;
using Xunit;

namespace Randevoo.Tests.Integration.AdminPanel;

public class DatabaseNotificationsApiClientTests
{
    [Fact]
    public async Task AdminNotification_IsDeliveredImmediately_AndCanBeMarkedRead()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var admin = await AdminPanelTestData.CreateUserAsync(db, "+989122200001", UserRole.Admin, "Admin");
        var recipient = await AdminPanelTestData.CreateUserAsync(db, "+989122200002", UserRole.EndUser, "Recipient");
        var service = new DatabaseNotificationsApiClient(db);
        var recipientMock = AdminPanelTestData.AsMockUser(recipient, AdminRole.EventPlanner);

        await service.CreateNotificationAsync(
            AdminPanelTestData.AsMockUser(admin, AdminRole.Admin),
            new NotificationCreateInput
            {
                Target = "User",
                TargetUserId = recipient.Id,
                Type = NotificationType.AdminToPlanner,
                Priority = NotificationPriority.Normal,
                Title = "Direct message",
                Body = "This notification should be delivered immediately."
            });

        Assert.Equal(1, await service.GetUnreadCountAsync(recipientMock));

        var inbox = await service.GetMyNotificationsAsync(recipientMock);
        await service.MarkAsReadAsync(recipientMock, inbox.Single().Id);

        Assert.Equal(0, await service.GetUnreadCountAsync(recipientMock));
        Assert.NotNull((await service.GetMyNotificationsAsync(recipientMock)).Single().ReadAtUtc);
    }

    [Fact]
    public async Task PlannerParticipantSmsNotification_WaitsForApproval_ThenCreatesSmsQueue()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var admin = await AdminPanelTestData.CreateUserAsync(db, "+989122200011", UserRole.Admin, "Admin");
        var planner = await AdminPanelTestData.CreateUserAsync(db, "+989122200012", UserRole.EventPlanner, "Planner");
        var buyer = await AdminPanelTestData.CreateUserAsync(db, "+989122200013", UserRole.EndUser, "Buyer");
        var participant = await AdminPanelTestData.CreateUserAsync(db, "+989122200014", UserRole.EndUser, "Participant", Gender.Female);
        var datingEvent = await AdminPanelTestData.CreateApprovedOpenEventAsync(db, planner, "Notification event");
        await AdminPanelTestData.SellTicketAsync(db, datingEvent, buyer, participant, 100m);
        var service = new DatabaseNotificationsApiClient(db);
        var participantMock = AdminPanelTestData.AsMockUser(participant, AdminRole.EventPlanner);

        await service.CreateNotificationAsync(
            AdminPanelTestData.AsMockUser(planner, AdminRole.EventPlanner),
            new NotificationCreateInput
            {
                Target = "EventParticipants",
                EventId = datingEvent.Id,
                Type = NotificationType.PlannerToParticipant,
                Priority = NotificationPriority.Important,
                Title = "Venue changed",
                Body = "Venue details changed and require review.",
                SendSms = true
            });

        var notificationId = await db.Notifications.Select(item => item.Id).SingleAsync();

        Assert.Equal(0, await service.GetUnreadCountAsync(participantMock));
        Assert.Empty(await service.GetMyNotificationsAsync(participantMock));
        Assert.Empty(await db.SmsQueueItems.ToListAsync());
        Assert.Equal(NotificationApprovalStatus.Pending, await db.Notifications.Select(item => item.ApprovalStatus).SingleAsync());

        await service.ApproveNotificationAsync(AdminPanelTestData.AsMockUser(admin, AdminRole.Admin), notificationId, "Approved.");

        Assert.Equal(1, await service.GetUnreadCountAsync(participantMock));
        Assert.Single(await db.SmsQueueItems.ToListAsync());
        Assert.All(await db.NotificationRecipients.ToListAsync(), item => Assert.NotEqual(NotificationRecipientStatus.Pending, item.Status));
    }

    [Fact]
    public async Task Planner_CannotSendNotification_ForAnotherPlannersEvent()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var planner = await AdminPanelTestData.CreateUserAsync(db, "+989122200021", UserRole.EventPlanner, "Planner");
        var otherPlanner = await AdminPanelTestData.CreateUserAsync(db, "+989122200022", UserRole.EventPlanner, "Other planner");
        var datingEvent = await AdminPanelTestData.CreateApprovedOpenEventAsync(db, otherPlanner, "Other planner event");
        var service = new DatabaseNotificationsApiClient(db);

        var action = () => service.CreateNotificationAsync(
            AdminPanelTestData.AsMockUser(planner, AdminRole.EventPlanner),
            new NotificationCreateInput
            {
                Target = "EventParticipants",
                EventId = datingEvent.Id,
                Type = NotificationType.PlannerToParticipant,
                Priority = NotificationPriority.Normal,
                Title = "Unauthorized",
                Body = "This should not be allowed."
            });

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }

    [Fact]
    public async Task PlannerOptions_ExcludeAdminOnlyMessageTypesAndTargets()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var planner = await AdminPanelTestData.CreateUserAsync(db, "+989122200031", UserRole.EventPlanner, "Planner");
        var service = new DatabaseNotificationsApiClient(db);

        var messageTypes = await service.GetMessageTypeOptionsAsync(AdminPanelTestData.AsMockUser(planner, AdminRole.EventPlanner));
        var targets = await service.GetTargetOptionsAsync(AdminPanelTestData.AsMockUser(planner, AdminRole.EventPlanner));

        Assert.DoesNotContain(messageTypes, item => item.Type == NotificationType.AdminToUser || item.Type == NotificationType.AdminToPlanner);
        Assert.DoesNotContain(targets, item => item.Value == "Planners");
        Assert.Contains(messageTypes, item => item.Type == NotificationType.PlannerToParticipant);
    }

    [Fact]
    public async Task Planner_CannotSendNotification_ToAllPlanners_EvenWithTamperedPayload()
    {
        await using var db = AdminPanelTestData.CreateDbContext();
        var planner = await AdminPanelTestData.CreateUserAsync(db, "+989122200041", UserRole.EventPlanner, "Planner");
        var service = new DatabaseNotificationsApiClient(db);

        var action = () => service.CreateNotificationAsync(
            AdminPanelTestData.AsMockUser(planner, AdminRole.EventPlanner),
            new NotificationCreateInput
            {
                Target = "Planners",
                Type = NotificationType.PlannerToParticipant,
                Priority = NotificationPriority.Normal,
                Title = "Tampered payload",
                Body = "Planner must not send this."
            });

        await Assert.ThrowsAsync<InvalidOperationException>(action);
    }
}
