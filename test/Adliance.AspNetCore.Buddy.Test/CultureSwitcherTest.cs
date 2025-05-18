using System.Globalization;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Test;

public class CultureSwitcherTest
{
    [Fact]
    public void Can_Switch_Culture()
    {
        Assert.NotEqual("el", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
        using (new CultureSwitcher("EL"))
        {
            Assert.Equal("el", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            Assert.Equal("el", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            Assert.Equal("el", Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName);
            Assert.Equal("el", Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
        }

        Assert.NotEqual("el", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
    }
}
