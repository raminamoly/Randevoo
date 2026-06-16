using System.Globalization;

namespace Randevoo.Web.Services;

public static class RandevooDisplay
{
    private static readonly PersianCalendar PersianCalendar = new();
    private static readonly string[] PersianWeekDays =
    [
        "یکشنبه",
        "دوشنبه",
        "سه‌شنبه",
        "چهارشنبه",
        "پنجشنبه",
        "جمعه",
        "شنبه"
    ];

    private static readonly string[] PersianMonths =
    [
        "فروردین",
        "اردیبهشت",
        "خرداد",
        "تیر",
        "مرداد",
        "شهریور",
        "مهر",
        "آبان",
        "آذر",
        "دی",
        "بهمن",
        "اسفند"
    ];

    public static string PersianDateTime(DateTime utcDateTime)
    {
        var local = ToIranTime(utcDateTime);
        var year = PersianCalendar.GetYear(local);
        var month = PersianCalendar.GetMonth(local);
        var day = PersianCalendar.GetDayOfMonth(local);
        var hour = PersianCalendar.GetHour(local);
        var minute = PersianCalendar.GetMinute(local);
        var weekDay = PersianWeekDays[(int)local.DayOfWeek];

        return ToPersianDigits($"{weekDay} {day} {PersianMonths[month - 1]} {year}، {hour:00}:{minute:00}");
    }

    public static string PersianDate(DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        var year = PersianCalendar.GetYear(dateTime);
        var month = PersianCalendar.GetMonth(dateTime);
        var day = PersianCalendar.GetDayOfMonth(dateTime);
        return ToPersianDigits($"{year:0000}/{month:00}/{day:00}");
    }

    public static string Rial(decimal amount) => $"{ToPersianDigits(amount.ToString("N0", CultureInfo.InvariantCulture))} ریال";

    public static string Money(decimal amount, string currencyCode)
        => string.Equals(currencyCode, "IRR", StringComparison.OrdinalIgnoreCase)
            ? Rial(amount)
            : $"{ToPersianDigits(amount.ToString("N0", CultureInfo.InvariantCulture))} {currencyCode}";

    public static string EventStatusLabel(int status) => status switch
    {
        1 => "فروش باز",
        2 => "فروش بسته",
        3 => "تکمیل ظرفیت",
        4 => "لغو شده",
        6 => "نزدیک زمان برگزاری",
        7 => "در حال برگزاری",
        8 => "تعامل پس از رویداد",
        9 => "پایان یافته",
        10 => "برگزار شده",
        _ => "رویداد"
    };

    public static string EventStatusClass(int status) => status switch
    {
        1 => "is-open",
        2 => "is-closed",
        3 => "is-warning",
        4 => "is-danger",
        7 => "is-live",
        8 => "is-success",
        9 => "is-muted",
        10 => "is-muted",
        _ => "is-neutral"
    };

    public static string PaymentMethodLabel(int method) => method switch
    {
        0 => "پرداخت آنلاین در پلتفرم",
        1 => "واریز دستی به پلتفرم",
        2 => "واریز مستقیم به برگزارکننده",
        _ => "هماهنگی پرداخت"
    };

    public static string EducationRestrictionLabel(int restriction) => restriction switch
    {
        0 => "بدون محدودیت تحصیلی",
        1 => "دیپلم به بالا",
        2 => "کارشناسی به بالا",
        3 => "کارشناسی ارشد به بالا",
        4 => "دکتری حرفه‌ای یا PhD",
        _ => "طبق شرایط رویداد"
    };

    public static string Duration(DateTime startsAtUtc, DateTime endsAtUtc)
    {
        var duration = endsAtUtc - startsAtUtc;
        if (duration.TotalMinutes <= 0)
            return "زمان برگزاری نامشخص";

        if (duration.TotalHours < 1)
            return $"{ToPersianDigits(((int)duration.TotalMinutes).ToString(CultureInfo.InvariantCulture))} دقیقه";

        var hours = (int)duration.TotalHours;
        var minutes = duration.Minutes;
        return minutes == 0
            ? $"{ToPersianDigits(hours.ToString(CultureInfo.InvariantCulture))} ساعت"
            : $"{ToPersianDigits(hours.ToString(CultureInfo.InvariantCulture))} ساعت و {ToPersianDigits(minutes.ToString(CultureInfo.InvariantCulture))} دقیقه";
    }

    public static string ToPersianDigits(string value)
    {
        var result = value
            .Replace("0", "۰", StringComparison.Ordinal)
            .Replace("1", "۱", StringComparison.Ordinal)
            .Replace("2", "۲", StringComparison.Ordinal)
            .Replace("3", "۳", StringComparison.Ordinal)
            .Replace("4", "۴", StringComparison.Ordinal)
            .Replace("5", "۵", StringComparison.Ordinal)
            .Replace("6", "۶", StringComparison.Ordinal)
            .Replace("7", "۷", StringComparison.Ordinal)
            .Replace("8", "۸", StringComparison.Ordinal)
            .Replace("9", "۹", StringComparison.Ordinal);

        return result;
    }

    public static string? PublicImageUrl(string? imageUrl, IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return null;

        var trimmed = imageUrl.Trim().Replace('\\', '/');
        if (Uri.TryCreate(trimmed, UriKind.Absolute, out _))
            return trimmed;

        if (trimmed.StartsWith("~/", StringComparison.Ordinal))
            trimmed = trimmed[1..];

        var publicBase = configuration["Assets:PublicBaseUrl"]?.TrimEnd('/');
        if (!string.IsNullOrWhiteSpace(publicBase))
            return $"{publicBase}/{trimmed.TrimStart('/')}";

        var uploadsRequestPath = configuration["Assets:AdminUploadsRequestPath"] ?? "/admin-uploads";
        if (trimmed.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            return $"{uploadsRequestPath.TrimEnd('/')}{trimmed["/uploads".Length..]}";

        if (trimmed.StartsWith("uploads/", StringComparison.OrdinalIgnoreCase))
            return $"{uploadsRequestPath.TrimEnd('/')}/{trimmed["uploads/".Length..]}";

        return trimmed.StartsWith("/", StringComparison.Ordinal) ? trimmed : $"/{trimmed}";
    }

    private static DateTime ToIranTime(DateTime dateTime)
    {
        var utc = dateTime.Kind == DateTimeKind.Utc ? dateTime : DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Iran Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone);
        }
        catch (TimeZoneNotFoundException)
        {
            try
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tehran");
                return TimeZoneInfo.ConvertTimeFromUtc(utc, timeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                return utc.AddHours(3.5);
            }
        }
    }
}
