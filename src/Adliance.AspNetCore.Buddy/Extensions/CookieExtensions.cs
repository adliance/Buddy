using System;
using Microsoft.AspNetCore.Http;

namespace Adliance.AspNetCore.Buddy.Extensions
{
    public static class CookieExtensions
    {
        public static string? Get(IRequestCookieCollection cookies, string key, string? defaultValue)
        {
            if (cookies.TryGetValue(key, out var cookieVal))
            {
                return cookieVal;
            }
            return defaultValue;
        }

        public static bool Get(this IRequestCookieCollection cookies, string key, bool defaultValue)
        {
            var cookieVal = Get(cookies, key, null);
            if (string.IsNullOrWhiteSpace(cookieVal))
            {
                return defaultValue;
            }
            return cookieVal == "1";
        }

        public static IResponseCookies Set(IResponseCookies cookies, string key, string value)
        {
            return Set(cookies, key, value, TimeSpan.FromDays(365));
        }

        public static IResponseCookies Set(IResponseCookies cookies, string key, string value, TimeSpan expiration)
        {
            cookies.Append(key, value, new CookieOptions { Expires = DateTime.UtcNow.Add(expiration) });
            return cookies;
        }

        public static IResponseCookies Set(IResponseCookies cookies, string key, bool value)
        {
            return Set(cookies, key, value, TimeSpan.FromDays(365));
        }

        public static IResponseCookies Set(IResponseCookies cookies, string key, bool value, TimeSpan expiration)
        {
            return Set(cookies, key, value ? "1" : "0", expiration);
        }

        public static IResponseCookies Delete(IResponseCookies cookies, string key)
        {
            cookies.Delete(key);
            return cookies;
        }
    }
}
