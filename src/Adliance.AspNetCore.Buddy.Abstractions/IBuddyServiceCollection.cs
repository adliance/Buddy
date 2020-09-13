using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Abstractions
{
    public interface IBuddyServiceCollection
    {
        public IServiceCollection Services { get; }
    }

    public class BuddyServiceCollection : IBuddyServiceCollection
    {
        public BuddyServiceCollection(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}