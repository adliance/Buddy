using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Authentication
{
    /// <inheritdoc/>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        #region Constructor

        private readonly IApiKeyAuthenticationService _authenticationService;
        private readonly IApiKeyAuthenticationConfiguration _conf;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IApiKeyAuthenticationService authenticationService,
            IApiKeyAuthenticationConfiguration conf)
            : base(options, logger, encoder, clock)
        {
            _authenticationService = authenticationService;
            _conf = conf;
        }

        #endregion Constructor

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (string.IsNullOrWhiteSpace(_conf.HttpHeaderKey) && string.IsNullOrWhiteSpace(_conf.QueryStringKey))
            {
                throw new Exception("No HTTP header and no query string key configured.");
            }

            string? apiKey = null;
            if (!string.IsNullOrWhiteSpace(_conf.QueryStringKey) && Request.Query.ContainsKey(_conf.QueryStringKey) && !string.IsNullOrWhiteSpace(Request.Query[_conf.QueryStringKey]))
            {
                apiKey = Request.Query[_conf.QueryStringKey];
            }

            // HTTP header takes precedence over querystring
            if (!string.IsNullOrWhiteSpace(_conf.HttpHeaderKey) && Request.Headers.ContainsKey(_conf.HttpHeaderKey) && !string.IsNullOrWhiteSpace(Request.Headers[_conf.HttpHeaderKey]))
            {
                apiKey = Request.Headers[_conf.HttpHeaderKey];
            }

            if (!_conf.AllowEmptyApiKey && string.IsNullOrWhiteSpace(apiKey))
            {
                return AuthenticateResult.Fail("No API key specified.");
            }

            var isValidKey = await _authenticationService.IsValidApiKey(apiKey);
            if (!isValidKey)
            {
                return AuthenticateResult.Fail("API key invalid");
            }

            var identity = new ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            var username = await _authenticationService.GetUserName(apiKey);
            if (!string.IsNullOrWhiteSpace(username))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, username));
            }

            var roles = await _authenticationService.GetRoles(apiKey);
            if (roles != null)
            {
                foreach (var r in roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, r));
                }
            }

            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), null, ApiKeyAuthenticationDefaults.AuthenticationScheme);
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "API key, charset=\"UTF-8\"";
            await base.HandleChallengeAsync(properties);
        }
    }
}