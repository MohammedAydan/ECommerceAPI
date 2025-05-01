using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(EmailMessage emailMessage);
    }
}
