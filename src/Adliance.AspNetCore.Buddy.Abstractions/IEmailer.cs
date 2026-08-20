using System;
using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions;

/// <summary>
/// Specifies the contract for an email provider.
/// </summary>
public partial interface IEmailer
{
    /// <summary>
    /// Asynchronously send the specified message.
    /// </summary>
    /// <param name="email">The email to be sent.</param>
    /// <returns>A task.</returns>
    Task Send(SendableEmail email);

    /// <summary>
    /// Sends the specified message without blocking. The <paramref name="onCompleted"/> callback is invoked when sending finishes;
    /// a <see langword="null"/> exception value indicates success.
    /// </summary>
    void SendNonBlocking(Action<Exception?> onCompleted, SendableEmail email);
}
