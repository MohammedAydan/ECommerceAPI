using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace ECommerceAPI.Repositories.Implementations
{
    public class EmailRepository : IEmailRepository
    {
        private readonly EmailConfig _emailConfig;

        public EmailRepository(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendEmail(EmailMessage emailMessage)
        {
            var message = CreateEmailMessage(emailMessage);
            await Send(message);
        }

        private MimeMessage CreateEmailMessage(EmailMessage emailMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ECommerce App", _emailConfig.From));
            message.To.AddRange(emailMessage.To);
            message.Subject = emailMessage.Subject;
            message.Body = new TextPart(TextFormat.Html) { Text = emailMessage.Content };

            return message;
        }

        private async Task Send(MimeMessage message)
        {
            using var smtpClient = new SmtpClient();
            try
            {
                smtpClient.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                smtpClient.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                await smtpClient.SendAsync(message);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                smtpClient.Disconnect(true);
                smtpClient.Dispose();
            }
        }
    }
}
