using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.Domain.Enums;
using AdminEventOperationalStatus = Randevoo.AdminPanel.Models.Events.EventOperationalStatus;

namespace Randevoo.AdminPanel.Services.State;

public static class EventStatusTransitionCatalog
{
    public static IReadOnlyList<EventStatusTransitionOption> GetOptions(DatingEvent datingEvent, AdminRole role)
    {
        if (role is not (AdminRole.Admin or AdminRole.EventPlanner))
            return Array.Empty<EventStatusTransitionOption>();

        if (datingEvent.ApprovalStatus != EventApprovalStatus.Approved)
            return Array.Empty<EventStatusTransitionOption>();

        if (datingEvent.OperationalStatus is AdminEventOperationalStatus.Cancelled or AdminEventOperationalStatus.Completed)
            return Array.Empty<EventStatusTransitionOption>();

        var options = new List<EventStatusTransitionOption>();

        if (datingEvent.OperationalStatus == AdminEventOperationalStatus.SaleClosed)
        {
            options.Add(new EventStatusTransitionOption
            {
                Action = EventStatusTransitionAction.OpenSale,
                Title = "باز کردن فروش",
                TargetLabel = "فروش باز",
                Description = "مطمئنی می‌خواهی فروش این رویداد را باز کنی؟ بعد از این کاربران می‌توانند بلیت بخرند.",
                IconCssClass = "bi-play-circle",
                ToneCssClass = "status-transition-success"
            });
        }

        if (datingEvent.OperationalStatus == AdminEventOperationalStatus.SaleOpen)
        {
            options.Add(new EventStatusTransitionOption
            {
                Action = EventStatusTransitionAction.CloseSale,
                Title = "بستن فروش",
                TargetLabel = "فروش بسته",
                Description = "مطمئنی می‌خواهی فروش این رویداد را ببندی؟ خرید جدید متوقف می‌شود اما بلیت‌های قبلی معتبر می‌مانند.",
                IconCssClass = "bi-pause-circle",
                ToneCssClass = "status-transition-neutral"
            });
        }

        options.Add(new EventStatusTransitionOption
        {
            Action = EventStatusTransitionAction.CancelEvent,
            Title = role == AdminRole.Admin ? "لغو رویداد" : "درخواست لغو رویداد",
            TargetLabel = role == AdminRole.Admin ? "لغو شده" : "درخواست لغو",
            Description = "مطمئنی می‌خواهی این رویداد را لغو کنی؟ این کار می‌تواند نیازمند برگشت وجه و اطلاع‌رسانی باشد.",
            RequiresNote = true,
            NoteLabel = "دلیل لغو",
            NotePlaceholder = "دلیل لغو رویداد را بنویسید.",
            IconCssClass = "bi-x-circle",
            ToneCssClass = "status-transition-danger"
        });

        return options;
    }

    public static string? GetEmptyMessage(DatingEvent datingEvent)
    {
        if (datingEvent.ApprovalStatus != EventApprovalStatus.Approved)
            return "پروفایل رویداد هنوز تایید نشده است. بعد از تایید پروفایل، وضعیت عملیاتی قابل تغییر خواهد بود.";

        if (datingEvent.OperationalStatus == AdminEventOperationalStatus.Cancelled)
            return "این رویداد لغو شده و تغییر وضعیت عملیاتی عادی ندارد.";

        if (datingEvent.OperationalStatus == AdminEventOperationalStatus.Completed)
            return "این رویداد تمام شده است. ادامه کار از مسیر تسویه، نظرسنجی و گزارش‌گیری انجام می‌شود.";

        return null;
    }
}
