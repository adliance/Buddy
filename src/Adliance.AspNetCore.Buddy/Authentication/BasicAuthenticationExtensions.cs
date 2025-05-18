using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Adliance.AspNetCore.Buddy.Authentication;

public static class BasicAuthenticationExtensions
{
    public static AuthenticationBuilder AddBasicAuthentication<TAuthService>(this AuthenticationBuilder builder)
        where TAuthService : class, IBasicAuthenticationService
    {
        return AddBasicAuthentication<TAuthService>(builder, BasicAuthenticationDefaults.AuthenticationScheme, _ => { });
    }

    public static AuthenticationBuilder AddBasicAuthentication<TAuthService>(this AuthenticationBuilder builder, string authenticationScheme)
        where TAuthService : class, IBasicAuthenticationService
    {
        return AddBasicAuthentication<TAuthService>(builder, authenticationScheme, _ => { });
    }

    public static AuthenticationBuilder AddBasicAuthentication<TAuthService>(this AuthenticationBuilder builder, Action<BasicAuthenticationOptions> configureOptions)
        where TAuthService : class, IBasicAuthenticationService
    {
        return AddBasicAuthentication<TAuthService>(builder, BasicAuthenticationDefaults.AuthenticationScheme, configureOptions);
    }

    public static AuthenticationBuilder AddBasicAuthentication<TAuthService>(this AuthenticationBuilder builder, string authenticationScheme, Action<BasicAuthenticationOptions> configureOptions)
        where TAuthService : class, IBasicAuthenticationService
    {
        builder.Services.AddTransient<IBasicAuthenticationService, TAuthService>();

        return builder.AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(authenticationScheme, configureOptions);
    }
}
