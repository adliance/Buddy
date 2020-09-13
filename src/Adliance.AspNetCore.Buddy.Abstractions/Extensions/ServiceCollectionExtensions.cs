using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IBuddyServiceCollection AddBuddy(this IServiceCollection services)
        {
            return new BuddyServiceCollection(services);
        }
    }
}