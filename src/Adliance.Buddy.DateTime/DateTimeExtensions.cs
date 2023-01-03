using System.Globalization;
using NodaTime;

namespace Adliance.Buddy.DateTime;

public static class DateTimeExtensions
{
    private const string DefaultCulture = "de-DE";
    private const string DefaultDateFormat = "dd. MMMM yyyy";
    private const string DefaultDateTimeFormat = "dd. MMMM yyyy, HH:mm:ss";

    public static System.DateTime? UtcToCet(this System.DateTime? utcDateTime)
    {
        if (utcDateTime.HasValue) return UtcToCet(utcDateTime.Value);
        return null;
    }

    public static System.DateTime UtcToCet(this System.DateTime utcDateTime)
    {
        var vienna = DateTimeZoneProviders.Tzdb["Europe/Vienna"];
        return LocalDateTime.FromDateTime(utcDateTime).InZoneStrictly(DateTimeZone.Utc).WithZone(vienna).ToDateTimeUnspecified();
    }

    public static System.DateTime? CetToUtc(this System.DateTime? cetDateTime)
    {
        if (cetDateTime.HasValue) return CetToUtc(cetDateTime.Value);
        return null;
    }

    public static System.DateTime CetToUtc(this System.DateTime cetDateTime)
    {
        var vienna = DateTimeZoneProviders.Tzdb["Europe/Vienna"];
        return LocalDateTime.FromDateTime(cetDateTime).InZoneStrictly(vienna).ToDateTimeUtc();
    }

    #region Format Date

    public static string FormatDate(this System.DateTime? dateTime, string format = DefaultDateFormat, string culture = DefaultCulture)
    {
        return dateTime.HasValue ? FormatDate(dateTime.Value, format, new CultureInfo(culture)) : "";
    }

    public static string FormatUtcAsCetDate(this System.DateTime? dateTime, string format = DefaultDateFormat, string culture = DefaultCulture)
    {
        return dateTime.HasValue ? FormatUtcAsCetDate(dateTime.Value, format, new CultureInfo(culture)) : "";
    }

    public static string FormatDate(this System.DateTime dateTime, string format = DefaultDateFormat, string culture = DefaultCulture)
    {
        return FormatDate(dateTime, format, new CultureInfo(culture));
    }

    public static string FormatUtcAsCetDate(this System.DateTime dateTime, string format = DefaultDateFormat, string culture = DefaultCulture)
    {
        return FormatUtcAsCetDate(dateTime, format, new CultureInfo(culture));
    }

    public static string FormatDate(this System.DateTime dateTime, string format, CultureInfo culture)
    {
        return dateTime == default ? "" : dateTime.ToString(format, culture);
    }

    public static string FormatUtcAsCetDate(this System.DateTime dateTime, string format, CultureInfo culture)
    {
        return FormatDate(dateTime.UtcToCet(), format, culture);
    }

    #endregion

    #region Format Date Time

    public static string FormatDateTime(this System.DateTime? dateTime, string format = DefaultDateTimeFormat, string culture = DefaultCulture)
    {
        return dateTime.HasValue ? FormatDateTime(dateTime.Value, format, new CultureInfo(culture)) : "";
    }

    public static string FormatUtcAsCetDateTime(this System.DateTime? dateTime, string format = DefaultDateTimeFormat, string culture = DefaultCulture)
    {
        return dateTime.HasValue ? FormatUtcAsCetDateTime(dateTime.Value, format, new CultureInfo(culture)) : "";
    }

    public static string FormatDateTime(this System.DateTime dateTime, string format = DefaultDateTimeFormat, string culture = DefaultCulture)
    {
        return FormatDateTime(dateTime, format, new CultureInfo(culture));
    }

    public static string FormatUtcAsCetDateTime(this System.DateTime dateTime, string format = DefaultDateTimeFormat, string culture = DefaultCulture)
    {
        return FormatUtcAsCetDateTime(dateTime, format, new CultureInfo(culture));
    }

    public static string FormatDateTime(this System.DateTime dateTime, string format, CultureInfo culture)
    {
        return dateTime == default ? "" : dateTime.ToString(format, culture);
    }

    public static string FormatUtcAsCetDateTime(this System.DateTime dateTime, string format, CultureInfo culture)
    {
        return FormatDateTime(dateTime.UtcToCet(), format, culture);
    }

    #endregion
}