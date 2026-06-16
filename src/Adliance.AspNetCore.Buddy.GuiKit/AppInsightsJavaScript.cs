using System;
using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.GuiKit;

public class AppInsightsJavaScript(IServiceProvider services)
{
    public HtmlString Script
    {
        get
        {
            var snippet = services.GetService<JavaScriptSnippet>();
            if (snippet == null) return new HtmlString("");
            return new HtmlString(snippet.FullScript);
        }
    }
}
