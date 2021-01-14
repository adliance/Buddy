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

            var validationResult = await _authenticationService.ValidateKey(apiKey);
            if (!validationResult.IsValid)
            {
                return AuthenticateResult.Fail("API key invalid");
            }

            var identity = new ClaimsIdentity(ApiKeyAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
            if (!string.IsNullOrWhiteSpace(validationResult.UserName))
            {
                identity.AddClaim(new Claim(ClaimTypes.Name, validationResult.UserName));
            }

            if (validationResult.Roles != null)
            {
                foreach (var r in validationResult.Roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, r));
                }
            }

            if (validationResult.Claims != null)
            {
                foreach (var c in validationResult.Claims)
                {
                    identity.AddClaim(c);
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