using System;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Adliance.AspNetCore.Buddy.Sms.Twilio.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IBuddyServiceCollection"/>.
    /// </summary>
    public static class BuddyServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Twilio to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="buddyServices">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
        /// <param name="smsConfiguration">The <see cref="ITwilioSmsConfiguration"/> instance.</param>
        /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
        public static IBuddyServiceCollection AddTwilio(this IBuddyServiceCollection buddyServices,
            ITwilioSmsConfiguration smsConfiguration)
        {
            buddyServices.Services.AddSingleton(smsConfiguration);
            buddyServices.Services.AddTransient<ISmser, TwilioSmser>();
            return buddyServices;
        }

        /// <summary>
        /// Adds Twilio to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="buddyServices">The <see cref="IBuddyServiceCollection" /> to add services to.</param>
        /// <param name="smsConfigurationSection">The <see cref="IConfigurationSection"/> configuration section.</param>
        /// <returns>The <see cref="IBuddyServiceCollection" /> so that additional calls can be chained.</returns>
        public static IBuddyServiceCollection AddTwilio(this IBuddyServiceCollection buddyServices,
            IConfigurationSection smsConfigurationSection)
        {
            var smsOptions = smsConfigurationSection.Get<TwilioSmsConfiguration>();
            buddyServices.Services.Configure<ITwilioSmsConfiguration>(smsConfigurationSection);
            ArgumentNullException.ThrowIfNull(smsOptions, "SMS Configuration");
            return AddTwilio(buddyServices, smsOptions);
        }
    }
}
