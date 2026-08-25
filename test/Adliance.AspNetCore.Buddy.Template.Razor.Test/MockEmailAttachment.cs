using Adliance.AspNetCore.Buddy.Abstractions;

namespace Adliance.AspNetCore.Buddy.Template.Razor.Test;

public class MockEmailAttachment(string filename, byte[] bytes) : IEmailAttachment
{
    public string Filename { get; } = filename;
    public byte[] Bytes { get; } = bytes;
}
