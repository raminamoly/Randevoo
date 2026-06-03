using System.Globalization;

namespace Randevoo.AdminPanel.Services.State;

public static class DisplayFormatter
{
    public static string Money(decimal value, bool useRtl)
        => useRtl ? $"{value:N0} تومان" : $"IRR {value:N0}";

    public static string Count(int value) => value.ToString(CultureInfo.InvariantCulture);
}

