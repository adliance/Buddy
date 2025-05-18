namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices;

/// <summary>
/// Specifies the configuration format.
/// </summary>
public interface IAzureCommunicationConfiguration
{
    /// <summary>
    /// The API endpoint to use.
    /// </summary>
    /// <remarks>
    /// This is unique for every tenant / app service instance.
    /// </remarks>
    string Endpoint { get; }

    /// <summary>
    /// The access key for authentication.
    /// </summary>
    string AccessKey { get; }

    /// <summary>
    /// Disable open/click tracking for end user mails.
    /// </summary>
    /// <remarks>
    /// Also needs to be enabled in for the send domain resource in the Azure portal.
    /// </remarks>
    bool UserEngagementTrackingDisabled { get; }
}

// ReSharper disable once UnusedType.Global
/// <summary>
/// Default configuration for Azure Communication emailer.
/// </summary>
public class DefaultAzureCommunicationConfiguration : IAzureCommunicationConfiguration
{
    /// <inheritdoc cref="IAzureCommunicationConfiguration.Endpoint"/>
    public string Endpoint { get; init; } = string.Empty;

    /// <inheritdoc cref="IAzureCommunicationConfiguration.AccessKey"/>
    public string AccessKey { get; init; } = string.Empty;

    /// <inheritdoc cref="IAzureCommunicationConfiguration.UserEngagementTrackingDisabled"/>
    public bool UserEngagementTrackingDisabled { get; init; } = true;
}
