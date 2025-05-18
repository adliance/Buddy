using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Storage.Extensions;

public static class HealthChecksBuilderExtensions
{
    public static IHealthChecksBuilder AddStorageCheck(this IHealthChecksBuilder builder)
    {
        return builder.AddCheck<StorageHealthCheck>("Storage");
    }
}
