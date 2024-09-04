using System;
using Adliance.AspNetCore.Buddy.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace Adliance.AspNetCore.Buddy.Pdf.V1.Extensions;

public static class BuddyServiceCollectionExtensions
{
    public static IBuddyServiceCollection AddPdf(
        this IBuddyServiceCollection buddyServices,
        IPdferConfiguration pdferConfiguration)
    {
        buddyServices.Services.AddSingleton(pdferConfiguration);
        return AddPdf(buddyServices);
    }

    public static IBuddyServiceCollection AddPdf(
        this IBuddyServiceCollection buddyServices,
        IConfigurationSection pdferConfigurationSection)
    {
        var pdferOptions = pdferConfigurationSection.Get<DefaultPdferConfiguration>();
        buddyServices.Services.Configure<DefaultPdferConfiguration>(pdferConfigurationSection);
        ArgumentNullException.ThrowIfNull(pdferOptions, "PDFer Configuration");
        return AddPdf(buddyServices, pdferOptions);
    }

    public static IBuddyServiceCollection AddPdf(
        this IBuddyServiceCollection buddyServices)
    {
        buddyServices.Services.AddTransient<IPdfer, AdliancePdfer>();
        return buddyServices;
    }
}
