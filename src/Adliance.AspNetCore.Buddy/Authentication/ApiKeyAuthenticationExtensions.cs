using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Authentication;

public static class ApiKeyAuthenticationExtensions
{
    public static AuthenticationBuilder AddApiKey<TAuthConfiguration, TAuthService>(this AuthenticationBuilder builder)
        where TAuthConfiguration : class, IApiKeyAuthenticationConfiguration
        where TAuthService : class, IApiKeyAuthenticationService
    {
        return AddApiKey<TAuthConfiguration, TAuthService>(builder, ApiKeyAuthenticationDefaults.AuthenticationScheme, _ => { });
    }

    public static AuthenticationBuilder AddApiKey<TAuthConfiguration, TAuthService>(this AuthenticationBuilder builder, string authenticationScheme)
        where TAuthConfiguration : class, IApiKeyAuthenticationConfiguration
        where TAuthService : class, IApiKeyAuthenticationService
    {
        return AddApiKey<TAuthConfiguration, TAuthService>(builder, authenticationScheme, _ => { });
    }

    public static AuthenticationBuilder AddApiKey<TAuthConfiguration, TAuthService>(this AuthenticationBuilder builder, Action<ApiKeyAuthenticationOptions> configureOptions)
        where TAuthConfiguration : class, IApiKeyAuthenticationConfiguration
        where TAuthService : class, IApiKeyAuthenticationService
    {
        return AddApiKey<TAuthConfiguration, TAuthService>(builder, ApiKeyAuthenticationDefaults.AuthenticationScheme, configureOptions);
    }

    public static AuthenticationBuilder AddApiKey<TAuthConfiguration, TAuthService>(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiKeyAuthenticationOptions> configureOptions)
        where TAuthConfiguration : class, IApiKeyAuthenticationConfiguration
        where TAuthService : class, IApiKeyAuthenticationService
    {
        builder.Services.AddTransient<IApiKeyAuthenticationConfiguration, TAuthConfiguration>();
        builder.Services.AddTransient<IApiKeyAuthenticationService, TAuthService>();

        return builder.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(authenticationScheme, configureOptions);
    }
}
