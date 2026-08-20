namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

/// <summary>
/// View model for the .cshtml fixtures in RazorFixtures/Views, used to render actual Razor templates in
/// RazorTemplaterTest. Must be public: the runtime-compiled views need to bind to it across assemblies.
/// </summary>
public class RazorFixtureModel
{
    public required string Name { get; init; }
    public List<string> Items { get; init; } = [];
}
