using System;
using System.Globalization;
using System.Text;
using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Email.Mailjet.Test;

public class MockedMailjetConfiguration : IMailjetConfiguration
{
    public string PublicApiKey => GetEnvironmentVariable("Adliance_Buddy_Tests__Mailjet_PublicApiKey");
    public string PrivateApiKey => GetEnvironmentVariable("Adliance_Buddy_Tests__Mailjet_PrivateApiKey");
    public string Campaign => "Unit Tests";

    public string GetEnvironmentVariable(string name)
    {
        return Environment.GetEnvironmentVariable(name)
               ?? Environment.GetEnvironmentVariable(name.ToUpper())
               ?? Environment.GetEnvironmentVariable(name.ToLower())
               ?? throw BuildEnvironmentVariableException(name);
    }

    private static Exception BuildEnvironmentVariableException(string name)
    {
        var sb = new StringBuilder();
        sb.AppendLine(CultureInfo.InvariantCulture, $"Environment variable \"{name}\" missing. Available environment variables are:");
        foreach (var o in Environment.GetEnvironmentVariables()) sb.AppendLine(o.ToString());
        throw new Exception(sb.ToString());
    }
}

public class MockedEmailConfiguration : IEmailConfiguration
{
    public string SenderName => "Hannes Sachsenhofer";
    public string SenderAddress => "test@adliance.net";
    public string ReplyToAddress => "hannes.sachsenhofer@adliance.net";
    public string RedirectAllEmailsTo => "";
    public bool Disable => false;
}

public class MockedEmailAttachment : IEmailAttachment
{
    public MockedEmailAttachment(string fileName, byte[] bytes)
    {
        Filename = fileName;
        Bytes = bytes;
    }

    public string Filename { get; set; }
    public byte[] Bytes { get; set; }
}
