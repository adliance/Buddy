using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// The contract for a razor templater.
/// </summary>
public interface ITemplater
{
    /// <summary>
    /// Renders a specified razor template provided in a directory.
    /// </summary>
    /// <param name="directoryName">The directory containing the template.</param>
    /// <param name="templateName">The template's name.</param>
    /// <param name="viewModel">The view model data for the razor template.</param>
    /// <returns>The rendered template as string.</returns>
    Task<string> Render(string directoryName, string templateName, object viewModel);
}
