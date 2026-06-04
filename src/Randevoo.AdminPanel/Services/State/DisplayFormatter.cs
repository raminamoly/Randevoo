using System.Globalization;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Events;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Services.State;

public static class DisplayFormatter
{
    public static string Money(decimal value, bool useRtl)
        => ToPersianDigits($"{value:N0} ریال");

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

    public static string TransactionType(BalanceTransactionType type) => type switch
    {
        BalanceTransactionType.AdminAdjustment => "اصلاح مدیر",
        BalanceTransactionType.TicketPurchase => "خرید بلیت",
        BalanceTransactionType.TicketRefund => "بازگشت بلیت",
        BalanceTransactionType.EventPlannerIncome => "درآمد کمیسیون",
        BalanceTransactionType.PlatformCommission => "کمیسیون پلتفرم",
        BalanceTransactionType.EmergencyRemovalRefund => "بازگشت حذف اضطراری",
        BalanceTransactionType.PlannerWithdrawalPayout => "تسویه برگزارکننده",
        BalanceTransactionType.EventPlannerIncomeReversal => "برگشت سهم برگزارکننده",
        _ => type.ToString()
    };

    public static string Gender(Gender gender) => gender switch
    {
        Randevoo.Domain.Enums.Gender.Male => "آقا",
        Randevoo.Domain.Enums.Gender.Female => "خانم",
        _ => "ثبت نشده"
    };

    public static string WithdrawalStatus(PlannerWithdrawalRequestStatus status) => status switch
    {
        PlannerWithdrawalRequestStatus.Pending => "در انتظار تایید",
        PlannerWithdrawalRequestStatus.Confirmed => "تایید شده",
        PlannerWithdrawalRequestStatus.Rejected => "رد شده",
        _ => status.ToString()
    };

    public static string EventTypeLabel(string? eventTypeName)
        => string.IsNullOrWhiteSpace(eventTypeName) ? "ثبت نشده" : eventTypeName.Trim();

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

    public static string EducationRestriction(EventEducationLevelRestriction restriction) => restriction switch
    {
        EventEducationLevelRestriction.WithoutLimit => "بدون محدودیت",
        EventEducationLevelRestriction.DiplomaOrHigher => "دیپلم به بالا",
        EventEducationLevelRestriction.BachelorOrHigher => "لیسانس به بالا",
        EventEducationLevelRestriction.MasterOrHigher => "فوق لیسانس به بالا",
        EventEducationLevelRestriction.ProfessionalDoctorateOrPhD => "دکترای حرفه ای / PHD / پزشک / دندان پزشک",
        _ => restriction.ToString()
    };

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
