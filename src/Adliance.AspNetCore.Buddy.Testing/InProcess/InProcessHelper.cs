using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace Adliance.AspNetCore.Buddy.Testing.InProcess;

public static class InProcessHelper
{
    public static async Task<InProcessResult<TEntryPoint>> Setup<TEntryPoint>(InProcessOptions<TEntryPoint> options) where TEntryPoint : class
    {
        var factory = new WebApplicationFactory<TEntryPoint>();
        factory = factory.WithWebHostBuilder(config =>
        {
            config.UseContentRoot(options.ContentRoot);

            config.ConfigureAppConfiguration((_, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(options.Configuration);
                configBuilder.AddEnvironmentVariables();
            });

            if (options.ConfigureWebAppServices != null) config.ConfigureServices(options.ConfigureWebAppServices);
            if (options.ConfigureWebAppTestServices != null) config.ConfigureTestServices(options.ConfigureWebAppTestServices);
        });

        await Task.CompletedTask.ConfigureAwait(false);

        var result = new InProcessResult<TEntryPoint>
        {
            Factory = factory,
            Client = factory.CreateClient()
        };
        return result;
    }
}
