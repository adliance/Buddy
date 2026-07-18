using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Adliance.AspNetCore.Buddy.GuiKit.Test.Pages;

public class PanelsModel : PageModel
{
    public ContentResult OnGetSnippet()
    {
        Thread.Sleep(6000);
        return Content("<p>This content was loaded via <code>hx-get</code>, using the htmx bundled with GuiKit.</p>", "text/html");
    }
}
