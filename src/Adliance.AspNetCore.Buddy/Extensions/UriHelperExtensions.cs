using System;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Adliance.AspNetCore.Buddy.Extensions
{
    public static class UriHelperExtensions
    {
        public static Uri GetAbsoluteUri(this IUrlHelper urlHelper, string action, string controller, object routeValues)
        {
            if (urlHelper?.ActionContext?.HttpContext?.Request == null)
            {
                throw new Exception("No current request available. Are you sure you're inside an HTTP request?");
            }

            return GetAbsoluteUri(urlHelper, new Uri(urlHelper.ActionContext.HttpContext.Request.GetDisplayUrl()), action, controller, routeValues);
        }

        public static Uri GetAbsoluteUri(this IUrlHelper urlHelper, Uri baseUri, string action, string controller, object routeValues)
        {
            return new Uri(baseUri.GetLeftPart(UriPartial.Authority) + urlHelper.Action(action, controller, routeValues));
        }
    }
}
