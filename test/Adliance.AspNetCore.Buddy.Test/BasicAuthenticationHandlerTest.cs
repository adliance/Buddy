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

public class BasicAuthenticationHandlerTest
{
    private readonly BasicAuthenticationHandler _handler;

    public BasicAuthenticationHandlerTest()
    {
        Mock<IOptionsMonitor<BasicAuthenticationOptions>> options = new();
        var timeProvider = new FakeTimeProvider();
        timeProvider.SetLocalTimeZone(TimeZoneInfo.Utc);
        timeProvider.SetUtcNow(new DateTimeOffset(2024, 3, 14, 12, 0, 0, TimeSpan.Zero));

        options
            .Setup(x => x.Get(It.IsAny<string>()))
            .Returns(new BasicAuthenticationOptions
            {
                // Time provider is superseding the ISystemClock
                TimeProvider = timeProvider
            });

        var logger = new Mock<ILogger<BasicAuthenticationHandler>>();
        Mock<ILoggerFactory> loggerFactory = new();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

        Mock<UrlEncoder> encoder = new();
        Mock<IBasicAuthenticationService> basicAuthenticationService = new();
        basicAuthenticationService
            .Setup(x => x.AuthenticateAsync("username", "password"))
            .ReturnsAsync(
                new BasicAuthenticationUser
                {
                    Id = "test",
                    Name = "Super Tester"
                });

        _handler = new BasicAuthenticationHandler(options.Object, loggerFactory.Object, encoder.Object,
            basicAuthenticationService.Object);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_NoAuthorizationHeader_ReturnsAuthenticateResultFail()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[HeaderNames.Authorization] = new StringValues(string.Empty);

        await _handler.InitializeAsync(
            new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, null,
                typeof(BasicAuthenticationHandler)), context);
        var result = await _handler.AuthenticateAsync();

        Assert.False(result.Succeeded);
        Assert.Equal("Invalid Authorization Header", result.Failure?.Message);
    }

    [Fact]
    public async Task HandleAuthenticateAsync_ValidAuthorizationHeader_ReturnsAuthenticationTicket()
    {
        var context = new DefaultHttpContext();
        var credentials = Convert.ToBase64String("username:password"u8.ToArray());
        context.Request.Headers[HeaderNames.Authorization] = new StringValues($"Basic {credentials}");

        await _handler.InitializeAsync(
            new AuthenticationScheme(BasicAuthenticationDefaults.AuthenticationScheme, null,
                typeof(BasicAuthenticationHandler)), context);
        var result = await _handler.AuthenticateAsync();

        Assert.True(result.Succeeded);
        Assert.Null(result.Failure);
        Assert.Equal("Super Tester", result.Principal?.Identity?.Name);
        // Assert.Equal(new DateTime(new DateOnly(2024, 1, 2), new TimeOnly()), result.Ticket.Properties.ExpiresUtc);
        Assert.Null(result.Ticket.Properties.ExpiresUtc);
        Assert.Null(result.Ticket.Properties.IssuedUtc);
        Assert.False(result.Ticket.Properties.IsPersistent);
        Assert.Null(result.Ticket.Properties.AllowRefresh);
        Assert.Equal("BasicAuthentication", result.Ticket.AuthenticationScheme);
    }
}