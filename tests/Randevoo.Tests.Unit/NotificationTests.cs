using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Xunit;

namespace Randevoo.Tests.Unit;

public class NotificationTests
{
    [Fact]
    public void Pending_NotificationRecipient_Cannot_Be_Marked_As_Read()
    {
        var sender = new User("+989120000001");
        var recipient = new User("+989120000002");
        var notification = new Notification(
            sender,
            NotificationType.PlannerToParticipant,
            "Event update",
            "This message must be approved before delivery.",
            requiresApproval: true);

        notification.AddRecipient(recipient, NotificationDeliveryChannel.InApp);

        var exception = Assert.Throws<BusinessRuleViolationException>(() => notification.Recipients[0].MarkRead());
        Assert.Equal(NotificationRecipientStatus.Pending, notification.Recipients[0].Status);
        Assert.Contains("not readable", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Approved_NotificationRecipient_Can_Be_Marked_As_Read()
    {
        var sender = new User("+989120000003");
        var admin = new User("+989120000004");
        admin.ChangeUserRole(UserRole.Admin);
        var recipient = new User("+989120000005");
        var notification = new Notification(
            sender,
            NotificationType.PlannerToParticipant,
            "Event update",
            "Approved messages can be read after delivery.",
            requiresApproval: true);

        notification.AddRecipient(recipient, NotificationDeliveryChannel.InApp);
        notification.Approve(admin);

        notification.Recipients[0].MarkRead();

        Assert.Equal(NotificationRecipientStatus.Read, notification.Recipients[0].Status);
        Assert.NotNull(notification.Recipients[0].ReadAtUtc);
    }
}
