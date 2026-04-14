namespace ShopOn.Web.Infrastructure;

public class Utilities
{
    private static readonly TimeZoneInfo BusinessTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");

    public decimal GetPayableAmount(decimal orderTotal, decimal returnTotal, decimal discount)
    {
        return orderTotal - returnTotal - discount;
    }

    public decimal GetBalanceAmount(decimal payableAmount, decimal amountPaid)
    {
        return payableAmount - amountPaid;
    }

    public static (DateTime StartUtc, DateTime EndUtc, string StartDisplay, string EndDisplay) GetCurrentMonthRange()
    {
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BusinessTimeZone);
        var localStart = new DateTime(localNow.Year, localNow.Month, 1);
        var localEnd = localStart.AddMonths(1).AddDays(-1);

        return (ToUtcStartOfDay(localStart), ToUtcEndOfDay(localEnd), localStart.ToString("dd-MMM-yyyy"), localEnd.ToString("dd-MMM-yyyy"));
    }

    public static DateTime OpenEndedStartUtc => new(1800, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime GetDefaultBusinessEndUtc()
    {
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BusinessTimeZone);
        return ToUtcEndOfDay(localNow.Date);
    }

    public static DateTime ParseStartDateOrDefaultUtc(string dateValue)
    {
        return string.IsNullOrWhiteSpace(dateValue)
            ? OpenEndedStartUtc
            : ToUtcStartOfDay(DateTime.Parse(dateValue).Date);
    }

    public static DateTime ParseEndDateOrDefaultUtc(string dateValue)
    {
        return string.IsNullOrWhiteSpace(dateValue)
            ? GetDefaultBusinessEndUtc()
            : ToUtcEndOfDay(DateTime.Parse(dateValue).Date);
    }

    public static string FormatBusinessDate(DateTime utcDateTime)
    {
        var normalizedUtc = utcDateTime.Kind == DateTimeKind.Utc
            ? utcDateTime
            : DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

        return TimeZoneInfo.ConvertTimeFromUtc(normalizedUtc, BusinessTimeZone).ToString("dd-MMM-yyyy");
    }

    private static DateTime ToUtcStartOfDay(DateTime localDate)
    {
        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(localDate.Date, DateTimeKind.Unspecified), BusinessTimeZone);
    }

    private static DateTime ToUtcEndOfDay(DateTime localDate)
    {
        var nextDay = DateTime.SpecifyKind(localDate.Date.AddDays(1), DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(nextDay, BusinessTimeZone).AddTicks(-1);
    }
}
