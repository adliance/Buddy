using System;
using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace Adliance.AspNetCore.Buddy.Extensions;

public static class CookieExtensions
{
    public static string? Get(this IRequestCookieCollection cookies, string key, string? defaultValue)
    {
        if (cookies.TryGetValue(key, out var cookieVal)) return cookieVal;
        return defaultValue;
    }

    public static int Get(this IRequestCookieCollection cookies, string key, int defaultValue)
    {
        var val = cookies.Get(key, defaultValue.ToString(CultureInfo.InvariantCulture));
        if (int.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var i)) return i;
        return defaultValue;
    }

    public static bool Get(this IRequestCookieCollection cookies, string key, bool defaultValue)
    {
        var cookieVal = Get(cookies, key, null);
        if (string.IsNullOrWhiteSpace(cookieVal)) return defaultValue;
        return cookieVal == "1";
    }

    public static IResponseCookies Set(this IResponseCookies cookies, string key, string value)
    {
        return Set(cookies, key, value, TimeSpan.FromDays(365));
    }

    public static IResponseCookies Set(this IResponseCookies cookies, string key, int value)
    {
        return Set(cookies, key, value, TimeSpan.FromDays(365));
    }

    public static IResponseCookies Set(this IResponseCookies cookies, string key, int value, TimeSpan expiration)
    {
        return Set(cookies, key, value.ToString(CultureInfo.InvariantCulture), expiration);
    }

    public static IResponseCookies Set(this IResponseCookies cookies, string key, string value, TimeSpan expiration)
    {
        cookies.Append(key, value, new CookieOptions
        {
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            Secure = true,
            HttpOnly = true,
            Expires = DateTime.UtcNow.Add(expiration)
        });
        return cookies;
    }

    public static IResponseCookies Set(this IResponseCookies cookies, string key, bool value)
    {
        return Set(cookies, key, value, TimeSpan.FromDays(365));
    }

    public static IResponseCookies Set(this IResponseCookies cookies, string key, bool value, TimeSpan expiration)
    {
        return Set(cookies, key, value ? "1" : "0", expiration);
    }

    public static IResponseCookies Delete(this IResponseCookies cookies, string key)
    {
        cookies.Delete(key);
        return cookies;
    }
}
