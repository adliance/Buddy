using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Testing.Shared.InProcess;

public class InProcessOptions<TEntryPoint> where TEntryPoint : class
{
    public string ContentRoot { get; set; } = "./";
    public string DbConnectionStringConfigurationKey { get; set; } = "";
    public Dictionary<string, string?> Configuration { get; set; } = new();
    public Action<IServiceCollection>? ConfigureWebAppServices { get; set; }
    public Action<IServiceCollection>? ConfigureWebAppTestServices { get; set; }
}
