// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace Adliance.AspNetCore.Buddy.Abstractions;

/// <summary>
/// Specifies the contract of a configuration for an email provider.
/// </summary>
public interface IEmailConfiguration
{
    /// <summary>
    /// The name of the sender.
    /// </summary>
    string SenderName { get; }

    /// <summary>
    /// The email address of the sender.
    /// </summary>
    string SenderAddress { get; }

    /// <summary>
    /// The "reply to" address (can be different from the sender address).
    /// </summary>
    string ReplyToAddress { get; }

    /// <summary>
    /// The email address all emails are optionally redirected to.
    /// </summary>
    string RedirectAllEmailsTo { get; }

    /// <summary>
    /// If set, this text will be prepended to all subjects that are being sent.
    /// </summary>
    string? SubjectPrefix { get; }

    /// <summary>
    /// If set, this text will be appended to all subjects that are being sent.
    /// </summary>
    string? SubjectPostfix { get; }

    /// <summary>
    /// If true all emails are discarded.
    /// </summary>
    bool Disable { get; }
}

// ReSharper disable once UnusedType.Global
/// <summary>
/// A default email configuration class.
/// </summary>
public class DefaultEmailConfiguration : IEmailConfiguration
{
    /// <inheritdoc cref="IEmailConfiguration.SenderName"/>
    public string SenderName { get; set; } = string.Empty;

    /// <inheritdoc cref="IEmailConfiguration.SenderAddress"/>
    public string SenderAddress { get; set; } = string.Empty;

    /// <inheritdoc cref="IEmailConfiguration.ReplyToAddress"/>
    public string ReplyToAddress { get; set; } = string.Empty;

    /// <inheritdoc cref="IEmailConfiguration.RedirectAllEmailsTo"/>
    public string RedirectAllEmailsTo { get; set; } = string.Empty;

    /// <inheritdoc cref="IEmailConfiguration.SubjectPrefix"/>
    public string? SubjectPrefix { get; set; } = string.Empty;

    /// <inheritdoc cref="IEmailConfiguration.SubjectPostfix"/>
    public string? SubjectPostfix { get; set; } = string.Empty;

    /// <inheritdoc cref="IEmailConfiguration.Disable"/>
    public bool Disable { get; set; }
}
