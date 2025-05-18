using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet;

public class MailjetHealthCheck : IHealthCheck
{
    private readonly IMailjetConfiguration _mailjetConfiguration;
    private readonly IEmailConfiguration _emailConfiguration;
    private readonly ILogger<MailjetHealthCheck> _logger;

    public MailjetHealthCheck(IMailjetConfiguration mailjetConfiguration, IEmailConfiguration emailConfiguration, ILogger<MailjetHealthCheck> logger)
    {
        _mailjetConfiguration = mailjetConfiguration;
        _emailConfiguration = emailConfiguration;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new MailjetClient(_mailjetConfiguration.PublicApiKey, _mailjetConfiguration.PrivateApiKey);

            var email = new TransactionalEmail
            {
                From = new SendContact(_emailConfiguration.SenderAddress, _emailConfiguration.SenderName),
                Subject = "Health Check",
                ReplyTo = new SendContact(_emailConfiguration.ReplyToAddress, _emailConfiguration.SenderName),
                CustomCampaign = _mailjetConfiguration.Campaign,
                To = new List<SendContact>
                {
                    new SendContact("hannes@sachsenhofer.com")
                },
                TextPart = "This e-mail is sent from a health check and should never reach anybody."
            };

            await client.SendTransactionalEmailAsync(email, true);
            return await Task.FromResult(HealthCheckResult.Healthy("Mailjet is healthy."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Mailjet Healthcheck failed: {ex.Message}");
            return await Task.FromResult(HealthCheckResult.Unhealthy("Mailjet is not healthy."));
        }
    }
}
