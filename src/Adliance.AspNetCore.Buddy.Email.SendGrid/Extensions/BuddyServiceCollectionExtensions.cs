using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Adliance.AspNetCore.Buddy.Email.SendGrid.Extensions;

public static class BuddyServiceCollectionExtensions
{
    public static IBuddyServiceCollection AddSendGrid(
        this IBuddyServiceCollection services,
        IEmailConfiguration emailConfiguration,
        ISendgridConfiguration sendgridConfiguration)
    {
        services.Services.AddSingleton(emailConfiguration);
        services.Services.AddSingleton(sendgridConfiguration);
        services.Services.AddTransient<IEmailer, SendgridEmailer>();
        return services;
    }
}
