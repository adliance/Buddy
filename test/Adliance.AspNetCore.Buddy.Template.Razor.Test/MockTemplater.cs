namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

public class MockTemplater : ITemplater
{
    public List<(string DirectoryName, string TemplateName, object ViewModel)> Calls { get; } = [];

    /// <summary>
    /// Template names for which Render() should throw, to simulate a missing template.
    /// </summary>
    public HashSet<string> TemplateNamesToThrowFor { get; } = [];

    /// <summary>
    /// Optional override to control what a given template renders to. Falls back to a default string based on the arguments.
    /// </summary>
    public Func<string, string, object, string>? RenderOverride { get; set; }

    public Task<string> Render(string directoryName, string templateName, object viewModel)
    {
        Calls.Add((directoryName, templateName, viewModel));

        if (TemplateNamesToThrowFor.Contains(templateName))
        {
            throw new InvalidOperationException($"No template named '{templateName}' found in directory '{directoryName}'.");
        }

        var result = RenderOverride != null
            ? RenderOverride(directoryName, templateName, viewModel)
            : $"{directoryName}/{templateName}: {viewModel}";
        return Task.FromResult(result);
    }
}
