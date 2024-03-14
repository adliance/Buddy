using System.Security.Claims;
using System.Text.Encodings.Web;
using Adliance.AspNetCore.Buddy.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Time.Testing;
using Microsoft.Net.Http.Headers;
using Moq;
using Xunit;

namespace Adliance.AspNetCore.Buddy.Test;

public class ApiKeyAuthenticationHandlerTest
{
    private readonly ApiKeyAuthenticationHandler _handler;

    public ApiKeyAuthenticationHandlerTest()
    {
        Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>> options = new();
        var timeProvider = new FakeTimeProvider();
        timeProvider.SetLocalTimeZone(TimeZoneInfo.Utc);
        timeProvider.SetUtcNow(new DateTimeOffset(2024, 3, 14, 12, 0, 0, TimeSpan.Zero));

        options
            .Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new ApiKeyAuthenticationOptions
            {
                // Time provider is superseding the ISystemClock
                TimeProvider = timeProvider
            });

        var logger = new Mock<ILogger<ApiKeyAuthenticationHandler>>();
        Mock<ILoggerFactory> loggerFactory = new();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

        Mock<UrlEncoder> encoder = new();
        Mock<IApiKeyAuthenticationService> apiKeyAuthenticationService = new();
        apiKeyAuthenticationService
            .Setup(x => x.ValidateKey("test-key"))
            .ReturnsAsync(
                new ApiKeyAuthenticationServiceResult
                {
                    IsValid = true,
                    UserName = "Super Tester",
                    Claims = [new Claim(ClaimTypes.Name, "Super Tester")],
                    Roles = ["ADMIN"]
                });

        var config = new MockApiKeyAuthenticationConfiguration();

        _handler = new ApiKeyAuthenticationHandler(options.Object, loggerFactory.Object, encoder.Object,
            apiKeyAuthenticationService.Object, config);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_NoAuthorizationHeader_ReturnsAuthenticateResultFail()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.Authorization] = new StringValues(string.Empty);

        await _handler.InitializeAsync(
            new AuthenticationScheme(ApiKeyAuthenticationDefaults.AuthenticationScheme, null,
                typeof(ApiKeyAuthenticationHandler)), context);
        var result = await _handler.AuthenticateAsync();

        Assert.False(result.Succeeded);
        Assert.Equal("No API key specified.", result.Failure?.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ValidAuthorizationHeader_ReturnsAuthenticationTicket()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Test-Key"] = new StringValues("test-key");

        await _handler.InitializeAsync(
            new AuthenticationScheme(ApiKeyAuthenticationDefaults.AuthenticationScheme, null,
                typeof(ApiKeyAuthenticationHandler)), context);
        var result = await _handler.AuthenticateAsync();

        Assert.True(result.Succeeded);
        Assert.Null(result.Failure);
        Assert.Equal("Super Tester", result.Principal?.Identity?.Name);
        // Assert.Equal(new DateTime(new DateOnly(2024, 1, 2), new TimeOnly()), result.Ticket.Properties.ExpiresUtc);
        Assert.Null(result.Ticket.Properties.ExpiresUtc);
        Assert.Null(result.Ticket.Properties.IssuedUtc);
        Assert.False(result.Ticket.Properties.IsPersistent);
        Assert.Null(result.Ticket.Properties.AllowRefresh);
        Assert.Equal("ApiKey", result.Ticket.AuthenticationScheme);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ValidQuery_ReturnsAuthenticationTicket()
    {
        var context = new DefaultHttpContext
        {
            Request =
            {
                QueryString = new QueryString("?Test-Key=test-key")
            }
        };

        await _handler.InitializeAsync(
            new AuthenticationScheme(ApiKeyAuthenticationDefaults.AuthenticationScheme, null,
                typeof(ApiKeyAuthenticationHandler)), context);
        var result = await _handler.AuthenticateAsync();

        Assert.True(result.Succeeded);
        Assert.Null(result.Failure);
        Assert.Equal("Super Tester", result.Principal?.Identity?.Name);
        // Assert.Equal(new DateTime(new DateOnly(2024, 1, 2), new TimeOnly()), result.Ticket.Properties.ExpiresUtc);
        Assert.Null(result.Ticket.Properties.ExpiresUtc);
        Assert.Null(result.Ticket.Properties.IssuedUtc);
        Assert.False(result.Ticket.Properties.IsPersistent);
        Assert.Null(result.Ticket.Properties.AllowRefresh);
        Assert.Equal("ApiKey", result.Ticket.AuthenticationScheme);
    }
}