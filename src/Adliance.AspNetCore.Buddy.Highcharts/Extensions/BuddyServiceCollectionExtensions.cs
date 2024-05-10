using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Adliance.AspNetCore.Buddy.Highcharts.Extensions;

public static class BuddyServiceCollectionExtensions
{
    public static IServiceCollection AddHighchartsServer(
        this IServiceCollection services,
        IHighchartsServerSettings configuration)
    {
        services.AddSingleton(configuration);
        return AddHighchartsServer(services);
    }

    public static IServiceCollection AddHighchartsServer(
        this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        var options = configurationSection.Get<HighchartsServerDefaultSettings>();
        services.Configure<HighchartsServerDefaultSettings>(configurationSection);
        ArgumentNullException.ThrowIfNull(options, "Highchart Configuration");
        return AddHighchartsServer(services, options);
    }

    public static IServiceCollection AddHighchartsServer(
        this IServiceCollection services)
    {
        services.AddTransient<HighchartsServer>();
        return services;
    }
}
