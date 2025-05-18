using System;
using System.Threading.Tasks;
using Adliance.AspNetCore.Buddy.Abstractions;
using static System.String;

namespace Adliance.AspNetCore.Buddy.Template.Razor;

/// <summary>
/// Renders a razor template and sends the result as SMS.
/// </summary>
public class SmsRenderer : ISmsRenderer
{
    #region Constructor

    private readonly ISmser _smsMessagingGateway;
    private readonly ITemplater _templater;

    /// <summary>
    /// Creates an instance of the SMS renderer class.
    /// </summary>
    /// <param name="smsMessagingGateway">An instance of a SMSer.</param>
    /// <param name="templater">An instance of a templater.</param>
    public SmsRenderer(ISmser smsMessagingGateway, ITemplater templater)
    {
        _smsMessagingGateway = smsMessagingGateway;
        _templater = templater;
    }

    #endregion Constructor

    /// <summary>
    /// Renders a message, based on the template <paramref name="templateBaseName"/> provided in the "SmsTemplates" directory.
    /// </summary>
    /// <inheritdoc cref="ISmsRenderer.RenderAndSendAsync(string,string,object)"/>
    /// <exception cref="ArgumentException">The <paramref name="recipientAddress"/> is null or whitespace.</exception>
    public virtual async Task RenderAndSendAsync(string recipientAddress, string templateBaseName, object viewModel)
    {
        await RenderAndSendAsync(
            recipientAddress,
            "SmsTemplates",
            templateBaseName,
            viewModel
        );
    }

    /// <inheritdoc cref="ISmsRenderer.RenderAndSendAsync(string,string,string,object)"/>
    /// <exception cref="ArgumentException">The <paramref name="recipientAddress"/> is null or whitespace.</exception>
    public virtual async Task RenderAndSendAsync(string recipientAddress, string templateDirectoryName,
        string textTemplateName, object viewModel)
    {
        if (IsNullOrWhiteSpace(recipientAddress))
        {
            throw new ArgumentException(@"The recipient must not be null.", nameof(recipientAddress));
        }

        string text;
        try
        {
            text = (await _templater.Render(templateDirectoryName, $"{textTemplateName}", viewModel)).Trim();
        }
        catch
        {
            text = Empty;
        }

        await _smsMessagingGateway.SendAsync(recipientAddress, text);
    }
}
