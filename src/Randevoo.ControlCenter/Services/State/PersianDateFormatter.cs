using System.Globalization;

namespace Randevoo.ControlCenter.Services.State;

public static class PersianDateFormatter
{
    private static readonly PersianCalendar Calendar = new();

    public static string Format(DateTimeOffset value, bool usePersian)
    {
        var local = value.ToLocalTime().DateTime;
        if (!usePersian)
        {
            return local.ToString("MMM d, yyyy h:mm tt", CultureInfo.InvariantCulture);
        }

        var formatted = string.Create(
            CultureInfo.InvariantCulture,
            $"{Calendar.GetYear(local)}/{Calendar.GetMonth(local):00}/{Calendar.GetDayOfMonth(local):00} {Calendar.GetHour(local):00}:{Calendar.GetMinute(local):00}");

        return ToPersianNumber(formatted);
    }

    public static string ToPersianNumber(int value) => ToPersianNumber(value.ToString(CultureInfo.InvariantCulture));

    public static string ToPersianNumber(string value)
    {
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
