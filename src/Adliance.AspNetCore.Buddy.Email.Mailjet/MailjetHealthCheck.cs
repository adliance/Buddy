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

public class MailjetHealthCheck(IMailjetConfiguration mailjetConfiguration, IEmailConfiguration emailConfiguration, ILogger<MailjetHealthCheck> logger)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = new MailjetClient(mailjetConfiguration.PublicApiKey, mailjetConfiguration.PrivateApiKey);

            var email = new TransactionalEmail
            {
                From = new SendContact(emailConfiguration.SenderAddress, emailConfiguration.SenderName),
                Subject = "Health Check",
                ReplyTo = new SendContact(emailConfiguration.ReplyToAddress, emailConfiguration.SenderName),
                CustomCampaign = mailjetConfiguration.Campaign,
                To = new List<SendContact>
                {
                    new("hannes@sachsenhofer.com")
                },
                TextPart = "This e-mail is sent from a health check and should never reach anybody."
            };

            await client.SendTransactionalEmailAsync(email, true);
            return await Task.FromResult(HealthCheckResult.Healthy("Mailjet is healthy."));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Mailjet Healthcheck failed: {ex.Message}");
            return await Task.FromResult(HealthCheckResult.Unhealthy("Mailjet is not healthy."));
        }
    }
}
