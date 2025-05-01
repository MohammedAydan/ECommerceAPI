using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Interfaces;

namespace ECommerceAPI.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IEmailRepository _emailSenderRepository;

        public EmailService(IEmailRepository emailSenderRepository)
        {
            _emailSenderRepository = emailSenderRepository;
        }

        public async Task SendEmail(EmailMessage emailMessage)
        {
            await _emailSenderRepository.SendEmail(emailMessage);
        }
    }
}
