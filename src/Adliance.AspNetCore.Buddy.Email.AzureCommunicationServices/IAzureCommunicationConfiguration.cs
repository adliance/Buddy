namespace Adliance.AspNetCore.Buddy.Email.AzureCommunicationServices;

public interface IAzureCommunicationConfiguration
{
    string Endpoint { get; }
    string AccessKey { get; }
    bool UserEngagementTrackingDisabled { get; }
}

// ReSharper disable once UnusedType.Global
public class DefaultAzureCommunicationConfiguration : IAzureCommunicationConfiguration
{
    public string Endpoint { get; init; } = "";
    public string AccessKey { get; init; } = "";
    public bool UserEngagementTrackingDisabled { get; init; } = true;
}