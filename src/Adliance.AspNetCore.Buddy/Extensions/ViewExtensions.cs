using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Adliance.AspNetCore.Buddy.Extensions
{
    public static class ViewExtensions
    {
        public static void SetCssClass(this ViewContext view, string cssClass)
        {
            view.ViewBag.pageCssClass = cssClass;
        }

        public static string GetCssClass(this ViewContext view)
        {
            return view.ViewBag.pageCssClass;
        }

        public static void SetTitle(this ViewContext view, string title)
        {
            view.ViewBag.pageTitle = title;
        }

        public static void SetTitle(this ViewContext view, LocalizedHtmlString title)
        {
            view.ViewBag.pageTitle = title.Value;
        }

        public static string GetTitle(this ViewContext view)
        {
            return view.ViewBag.pageTitle;
        }
    }
}
