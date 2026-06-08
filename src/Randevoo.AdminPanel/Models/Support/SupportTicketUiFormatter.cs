using System.Globalization;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Models.Support;

public static class SupportTicketUiFormatter
{
    public static string FormatStatus(SupportTicketStatus status) => status switch
    {
        SupportTicketStatus.Open => "باز",
        SupportTicketStatus.InProgress => "در حال رسیدگی",
        SupportTicketStatus.WaitingForUser => "منتظر ثبت‌کننده",
        SupportTicketStatus.Closed => "بسته",
        SupportTicketStatus.Reopened => "بازگشایی شده",
        _ => status.ToString()
    };

    public static string FormatCategory(SupportTicketCategory category) => category switch
    {
        SupportTicketCategory.FinancialProblem => "مشکل مالی",
        SupportTicketCategory.EventProblem => "مشکل رویداد",
        SupportTicketCategory.GeneralQuestion => "سوال عمومی",
        _ => category.ToString()
    };

    public static string FormatRole(UserRole role) => role switch
    {
        UserRole.EndUser => "شرکت‌کننده",
        UserRole.EventPlanner => "برگزارکننده",
        UserRole.PlatformSupportTeam => "کارشناس پشتیبانی",
        UserRole.Admin => "مدیر",
        _ => role.ToString()
    };

    public static string FormatTransactionType(BalanceTransactionType type) => type switch
    {
        BalanceTransactionType.TicketPurchase => "خرید بلیت",
        BalanceTransactionType.TicketRefund => "بازگشت وجه",
        BalanceTransactionType.EventPlannerIncome => "درآمد برگزارکننده",
        BalanceTransactionType.EventPlannerIncomeReversal => "برگشت درآمد برگزارکننده",
        BalanceTransactionType.PlannerWithdrawalPayout => "تسویه برگزارکننده",
        BalanceTransactionType.EmergencyRemovalRefund => "بازگشت اضطراری",
        BalanceTransactionType.AdminAdjustment => "اصلاح مدیر",
        _ => type.ToString()
    };

    public static string FormatPaymentStatus(OnlinePaymentStatus status) => status switch
    {
        OnlinePaymentStatus.Pending => "در انتظار",
        OnlinePaymentStatus.Succeeded => "موفق",
        OnlinePaymentStatus.Failed => "ناموفق",
        OnlinePaymentStatus.Refunded => "برگشت خورده",
        _ => status.ToString()
    };

    public static string ToJalaliDate(DateTime? dateTime)
    {
        if (dateTime is null)
            return string.Empty;

        var calendar = new PersianCalendar();
        var value = dateTime.Value;
        return FormattableString.Invariant($"{calendar.GetYear(value):0000}/{calendar.GetMonth(value):00}/{calendar.GetDayOfMonth(value):00}");
    }

    public static DateTime? ParseJalaliDate(string? value)
    {
        var normalized = NormalizeDigits(value).Replace("-", "/", StringComparison.Ordinal).Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            return null;

        var parts = normalized.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 3)
            return null;

        if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var year) ||
            !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var month) ||
            !int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var day))
            return null;

        var calendar = new PersianCalendar();
        return calendar.ToDateTime(year, month, day, 0, 0, 0, 0, PersianCalendar.PersianEra);
    }

    private static string NormalizeDigits(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        return value
            .Replace('۰', '0')
            .Replace('۱', '1')
            .Replace('۲', '2')
            .Replace('۳', '3')
            .Replace('۴', '4')
            .Replace('۵', '5')
            .Replace('۶', '6')
            .Replace('۷', '7')
            .Replace('۸', '8')
            .Replace('۹', '9')
            .Replace('٠', '0')
            .Replace('١', '1')
            .Replace('٢', '2')
            .Replace('٣', '3')
            .Replace('٤', '4')
            .Replace('٥', '5')
            .Replace('٦', '6')
            .Replace('٧', '7')
            .Replace('٨', '8')
            .Replace('٩', '9');
    }
}
