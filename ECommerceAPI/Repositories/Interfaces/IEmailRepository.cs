using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IEmailRepository
    {
        Task SendEmail(EmailMessage emailMessage);
    }
}
