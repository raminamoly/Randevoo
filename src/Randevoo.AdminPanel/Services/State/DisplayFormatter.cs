using System.Globalization;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;

namespace Randevoo.AdminPanel.Services.State;

public static class DisplayFormatter
{
    public static string Money(decimal value, bool useRtl)
        => ToPersianDigits($"{value:N0} تومان");

    public static string Count(int value) => ToPersianDigits(value.ToString(CultureInfo.InvariantCulture));

    public static string Number(int value) => ToPersianDigits(value.ToString(CultureInfo.InvariantCulture));

    public static string Number(decimal value) => ToPersianDigits(value.ToString("N0", CultureInfo.InvariantCulture));

    public static string Percent(decimal value) => ToPersianDigits($"{value:N0}٪");

    public static string Role(AdminRole role) => role switch
    {
        AdminRole.Admin => "مدیر",
        AdminRole.EventPlanner => "برگزارکننده",
        AdminRole.SupportTeam => "تیم پشتیبانی",
        _ => role.ToString()
    };

    public static string Status(EventApprovalState state) => state switch
    {
        EventApprovalState.Draft => "پیش نویس",
        EventApprovalState.PendingAdminReview => "در انتظار تایید مدیر",
        EventApprovalState.Approved => "تایید شده",
        EventApprovalState.Rejected => "رد شده",
        EventApprovalState.Closed => "بسته",
        EventApprovalState.Cancelled => "لغو شده",
        _ => state.ToString()
    };

    public static string EventTypeLabel(EventType eventType) => eventType switch
    {
        EventType.SocialEvening => "گردهمایی اجتماعی",
        EventType.Dinner => "شام",
        EventType.Coffee => "کافه",
        EventType.Rooftop => "روف تاپ",
        EventType.Workshop => "کارگاه",
        EventType.Gallery => "گالری",
        _ => eventType.ToString()
    };

    public static string Bool(bool value) => value ? "بله" : "خیر";

    public static string ToPersianDigits(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value
            .Replace('0', '۰')
            .Replace('1', '۱')
            .Replace('2', '۲')
            .Replace('3', '۳')
            .Replace('4', '۴')
            .Replace('5', '۵')
            .Replace('6', '۶')
            .Replace('7', '۷')
            .Replace('8', '۸')
            .Replace('9', '۹');
    }
}
