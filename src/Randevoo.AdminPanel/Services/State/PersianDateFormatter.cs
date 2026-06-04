using System.Globalization;

namespace Randevoo.AdminPanel.Services.State;

public static class PersianDateFormatter
{
    public static bool TryParseDate(string? text, bool useShamsi, out DateTimeOffset date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        var normalizedText = NormalizeDigits(text).Trim();
        if (!useShamsi)
        {
            return DateTimeOffset.TryParse(
                normalizedText,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out date);
        }

        var dateBits = normalizedText.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (dateBits.Length != 3
            || !int.TryParse(dateBits[0], NumberStyles.None, CultureInfo.InvariantCulture, out var year)
            || !int.TryParse(dateBits[1], NumberStyles.None, CultureInfo.InvariantCulture, out var month)
            || !int.TryParse(dateBits[2], NumberStyles.None, CultureInfo.InvariantCulture, out var day))
        {
            return false;
        }

        try
        {
            var calendar = new PersianCalendar();
            var gregorian = calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            date = new DateTimeOffset(gregorian, TimeSpan.FromHours(3.5));
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string FormatDate(DateTimeOffset? utcDateTime, bool useShamsi)
    {
        if (utcDateTime is null)
        {
            return string.Empty;
        }

        var local = utcDateTime.Value.ToLocalTime();
        if (!useShamsi)
        {
            return DisplayFormatter.ToPersianDigits(local.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
        }

        var calendar = new PersianCalendar();
        var year = calendar.GetYear(local.DateTime);
        var month = calendar.GetMonth(local.DateTime);
        var day = calendar.GetDayOfMonth(local.DateTime);
        return DisplayFormatter.ToPersianDigits($"{year:0000}/{month:00}/{day:00}");
    }

    public static string FormatTime(DateTimeOffset? utcDateTime)
    {
        if (utcDateTime is null)
        {
            return string.Empty;
        }

        return DisplayFormatter.ToPersianDigits(utcDateTime.Value.ToLocalTime().ToString("HH:mm", CultureInfo.InvariantCulture));
    }

    public static string Format(DateTimeOffset? utcDateTime, bool useShamsi)
    {
        if (utcDateTime is null)
        {
            return "-";
        }

        var local = utcDateTime.Value.ToLocalTime();
        if (!useShamsi)
        {
            return DisplayFormatter.ToPersianDigits(local.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
        }

        var calendar = new PersianCalendar();
        var year = calendar.GetYear(local.DateTime);
        var month = calendar.GetMonth(local.DateTime);
        var day = calendar.GetDayOfMonth(local.DateTime);
        return DisplayFormatter.ToPersianDigits($"{year:0000}/{month:00}/{day:00} {local:HH:mm}");
    }

    public static string Format(DateTime? utcDateTime, bool useShamsi)
    {
        if (utcDateTime is null)
        {
            return "-";
        }

        return Format(new DateTimeOffset(DateTime.SpecifyKind(utcDateTime.Value, DateTimeKind.Utc)), useShamsi);
    }

    public static string FormatDateTime(DateTime utcDateTime, bool useShamsi)
        => Format(new DateTimeOffset(DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc)), useShamsi);

    public static DateTimeOffset Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return DateTimeOffset.UtcNow;
        }

        var normalizedText = NormalizeDigits(text);

        if (DateTimeOffset.TryParse(normalizedText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed))
        {
            return parsed.ToUniversalTime();
        }

        var parts = normalizedText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
        {
            return DateTimeOffset.UtcNow;
        }

        var dateBits = parts[0].Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (dateBits.Length != 3)
        {
            return DateTimeOffset.UtcNow;
        }

        var year = int.Parse(dateBits[0], CultureInfo.InvariantCulture);
        var month = int.Parse(dateBits[1], CultureInfo.InvariantCulture);
        var day = int.Parse(dateBits[2], CultureInfo.InvariantCulture);

        var time = parts.Length > 1 ? parts[1] : "00:00";
        var timeBits = time.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var hour = timeBits.Length > 0 ? int.Parse(timeBits[0], CultureInfo.InvariantCulture) : 0;
        var minute = timeBits.Length > 1 ? int.Parse(timeBits[1], CultureInfo.InvariantCulture) : 0;

        var calendar = new PersianCalendar();
        var localDateTime = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Unspecified);
        var gregorian = calendar.ToDateTime(localDateTime.Year, localDateTime.Month, localDateTime.Day, localDateTime.Hour, localDateTime.Minute, 0, 0);
        return new DateTimeOffset(gregorian, TimeSpan.FromHours(3.5));
    }

    private static string NormalizeDigits(string value) => value
        .Replace('۰', '0')
        .Replace('۱', '1')
        .Replace('۲', '2')
        .Replace('۳', '3')
        .Replace('۴', '4')
        .Replace('۵', '5')
        .Replace('۶', '6')
        .Replace('۷', '7')
        .Replace('۸', '8')
        .Replace('۹', '9');
}
