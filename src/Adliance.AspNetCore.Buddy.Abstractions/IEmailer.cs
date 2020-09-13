using System.Threading.Tasks;

namespace Adliance.AspNetCore.Buddy.Abstractions
{
    public interface IEmailer
    {
        Task Send(string recipientAddress, string subject, string htmlBody, string textBody, params IEmailAttachment[] attachments);
    }
}
