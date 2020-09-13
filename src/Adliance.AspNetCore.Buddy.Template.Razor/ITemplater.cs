using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Abstractions
{
    public interface ITemplater
    {
        Task<string> Render(string directoryName, string templateName, object viewModel);
    }
}
