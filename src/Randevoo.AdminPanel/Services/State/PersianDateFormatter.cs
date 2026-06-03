using System.Globalization;

namespace Randevoo.AdminPanel.Services.State;

public static class PersianDateFormatter
{
    public static string Format(DateTimeOffset? utcDateTime, bool useShamsi)
    {
        if (utcDateTime is null)
        {
            return "-";
        }

        var local = utcDateTime.Value.ToLocalTime();
        if (!useShamsi)
        {
            return local.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        }

        var calendar = new PersianCalendar();
        var year = calendar.GetYear(local.DateTime);
        var month = calendar.GetMonth(local.DateTime);
        var day = calendar.GetDayOfMonth(local.DateTime);
        return $"{year:0000}/{month:00}/{day:00} {local:HH:mm}";
    }

    public static DateTimeOffset Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return DateTimeOffset.UtcNow;
        }

        if (DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out var parsed))
        {
            return parsed.ToUniversalTime();
        }

        var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
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
}

