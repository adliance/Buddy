namespace Adliance.AspNetCore.Buddy.Abstractions
{
    public interface IEmailAttachment
    {
        string Filename { get; }
        byte[] Bytes { get; }
    }
}
