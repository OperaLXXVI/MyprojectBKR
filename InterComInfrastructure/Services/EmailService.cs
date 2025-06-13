using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace InterComInfrastructure.Services
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string bodyHtml, byte[]? attachment = null, string? attachmentName = null);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _cfg;
        public EmailService(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task SendAsync(string to, string subject, string bodyHtml, byte[]? attachment = null, string? attachmentName = null)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_cfg["Smtp:From"]));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = bodyHtml };

            if (attachment != null && attachmentName != null)
                builder.Attachments.Add(attachmentName, attachment);

            msg.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_cfg["Smtp:Host"], int.Parse(_cfg["Smtp:Port"]), bool.Parse(_cfg["Smtp:EnableSsl"]));
            await client.AuthenticateAsync(_cfg["Smtp:Username"], _cfg["Smtp:Password"]);
            await client.SendAsync(msg);
            await client.DisconnectAsync(true);
        }
    }
}
