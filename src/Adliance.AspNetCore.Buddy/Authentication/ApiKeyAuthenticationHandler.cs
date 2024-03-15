using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Adliance.AspNetCore.Buddy.Authentication;

/// <inheritdoc />
// ReSharper disable once ClassNeverInstantiated.Global
public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IApiKeyAuthenticationService authenticationService,
    IApiKeyAuthenticationConfiguration conf)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (string.IsNullOrWhiteSpace(conf.HttpHeaderKey) && string.IsNullOrWhiteSpace(conf.QueryStringKey))
            throw new Exception("No HTTP header and no query string key configured.");

        string? apiKey = null;
        if (!string.IsNullOrWhiteSpace(conf.QueryStringKey) && Request.Query.ContainsKey(conf.QueryStringKey) &&
            !string.IsNullOrWhiteSpace(Request.Query[conf.QueryStringKey]))
            apiKey = Request.Query[conf.QueryStringKey];

        // HTTP header takes precedence over querystring
        if (!string.IsNullOrWhiteSpace(conf.HttpHeaderKey) && Request.Headers.ContainsKey(conf.HttpHeaderKey) &&
            !string.IsNullOrWhiteSpace(Request.Headers[conf.HttpHeaderKey]))
            apiKey = Request.Headers[conf.HttpHeaderKey];

        if (!conf.AllowEmptyApiKey && string.IsNullOrWhiteSpace(apiKey))
            return AuthenticateResult.Fail("No API key specified.");

        var validationResult = await authenticationService.ValidateKey(apiKey);
        if (!validationResult.IsValid) return AuthenticateResult.Fail("API key invalid");

        var identity = new ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name,
            ClaimTypes.Role);
        if (!string.IsNullOrWhiteSpace(validationResult.UserName))
            identity.AddClaim(new Claim(ClaimTypes.Name, validationResult.UserName));

        if (validationResult.Roles != null)
            foreach (var r in validationResult.Roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, r));

        if (validationResult.Claims != null)
            foreach (var c in validationResult.Claims)
                identity.AddClaim(c);

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), null,
            ApiKeyAuthenticationDefaults.AuthenticationScheme);
        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = "API key, charset=\"UTF-8\"";
        await base.HandleChallengeAsync(properties);
    }
}