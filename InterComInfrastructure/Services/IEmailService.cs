using System.Threading.Tasks;

namespace InterComInfrastructure.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Відправляє e-mail із вложенням PDF.
        /// </summary>
        /// <param name="toEmail">Адреса отримувача</param>
        /// <param name="subject">Тема листа</param>
        /// <param name="bodyHtml">HTML-тіло</param>
        /// <param name="attachment">Байти PDF</param>
        /// <param name="attachmentName">Назва файла</param>
        Task SendAsync(
            string toEmail,
            string subject,
            string bodyHtml,
            byte[] attachment,
            string attachmentName);
    }
}
