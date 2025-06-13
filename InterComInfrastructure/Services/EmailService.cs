using InterComInfrastructure.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace InterComInfrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _cfg;

        public EmailService(IConfiguration cfg)
        {
            _cfg = cfg;
        }

        public async Task SendAsync(
            string toEmail,
            string subject,
            string bodyHtml,
            byte[] attachmentBytes,
            string attachmentName)
        {
            using var message = new MimeKit.MimeMessage();
            message.From.Add(MimeKit.MailboxAddress.Parse(_cfg["Smtp:From"]!));
            message.To.Add(MimeKit.MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var builder = new MimeKit.BodyBuilder
            {
                HtmlBody = bodyHtml
            };

            builder.Attachments.Add(attachmentName, attachmentBytes);

            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();

            // 1) Витягаємо налаштування
            var host = _cfg["Smtp:Host"]!;
            var port = int.Parse(_cfg["Smtp:Port"]!);
            var user = _cfg["Smtp:Username"]!;
            var pass = _cfg["Smtp:Password"]!;

            // 2) Підключаємося з STARTTLS на порті 587
            //    Якщо ви хочете implicit SSL, використовуйте порт 465 і SecureSocketOptions.SslOnConnect
            await client.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls
            );

            // 3) Аутентифікуємося
            await client.AuthenticateAsync(user, pass);

            // 4) Відправляємо лист
            await client.SendAsync(message);

            await client.DisconnectAsync(true);
        }
    }
}
