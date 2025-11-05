namespace Adliance.AspNetCore.Buddy.Testing.Shared.Playwright;

public class PlaywrightOptions
{
    public PlaywrightType Playwright { get; set; } = PlaywrightType.HeadlessOnlyWhenRunningInAgent;

    public bool Headless => Playwright == PlaywrightType.HeadlessAlways ||
                            (Playwright == PlaywrightType.HeadlessOnlyWhenRunningInAgent &&
                             BuddyEnvironment.IsRunningInAgent);

    public int Timeout { get; set; } = 30000;
}

public enum PlaywrightType
{
    HeadlessAlways,
    HeadedAlways,
    HeadlessOnlyWhenRunningInAgent
}
