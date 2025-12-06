namespace Services.Notifications.Application.Helpers;
public static class EmailUtils
{

    public static string FormatMoney(decimal? moneyAmount)
    {
        var vietnamCulture = new System.Globalization.CultureInfo("vi-VN");
        if (!moneyAmount.HasValue)
        {
            return "0";
        }

        return moneyAmount.Value.ToString("N0", vietnamCulture);
    }
    public static DateTime FormatTime (DateTime time)
    {
        var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
        return TimeZoneInfo.ConvertTimeFromUtc(time, vietnamTimeZone);
    }
}
