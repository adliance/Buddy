using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid;

public class SendgridHealthCheck : IHealthCheck
{
    private readonly ISendgridConfiguration _sendGridConfiguration;
    private readonly ILogger<SendgridHealthCheck> _logger;

    public SendgridHealthCheck(ISendgridConfiguration sendGridConfiguration, ILogger<SendgridHealthCheck> logger)
    {
        _sendGridConfiguration = sendGridConfiguration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
    {
        var healthy = false;
        try
        {
            var sendgrid = new SendGridClient(_sendGridConfiguration.SendgridSecret);
            var message = new SendGridMessage
            {
                Subject = "Health Check",
                From = new EmailAddress("name@server.com"),
                PlainTextContent = "This is just a health check",
            };
            message.AddTo(new EmailAddress("name@server.com"));
            message.MailSettings = new MailSettings
            {
                SandboxMode = new SandboxMode
                {
                    Enable = true
                }
            };

            var result = await sendgrid.SendEmailAsync(message, cancellationToken);
            if (result.StatusCode == HttpStatusCode.OK)
            {
                healthy = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sendgrid health check failed.");
        }

        if (healthy)
        {
            return await Task.FromResult(HealthCheckResult.Healthy("Sendgrid is healthy."));
        }

        return await Task.FromResult(HealthCheckResult.Unhealthy("Sendgrid is not healthy."));
    }
}
