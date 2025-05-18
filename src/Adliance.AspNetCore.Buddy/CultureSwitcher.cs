using System;
using System.Globalization;

namespace Adliance.AspNetCore.Buddy;

public class CultureSwitcher : IDisposable
{
    private readonly CultureInfo _previousCulture;

    public CultureSwitcher(string culture) : this(new CultureInfo(culture))
    {
    }

    public CultureSwitcher(CultureInfo culture)
    {
        _previousCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = culture;
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = _previousCulture;
    }
}
