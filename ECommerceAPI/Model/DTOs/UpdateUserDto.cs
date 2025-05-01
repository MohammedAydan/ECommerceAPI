using ECommerceAPI.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Model.DTOs
{
    public class UpdateUserDto
    {
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public IFormFile? Image { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        
        // Check if any field is empty
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(UserName) ||
                   string.IsNullOrWhiteSpace(Email) ||
                   string.IsNullOrWhiteSpace(Country) ||
                   string.IsNullOrWhiteSpace(City) ||
                   string.IsNullOrWhiteSpace(Address);
        }

        public UserModel ConvertToUserModel()
        {
            return new UserModel
            {
                //Id = Id
                UserName = this.UserName,
                Email = this.Email,
                Country = this.Country,
                City = this.City,
                Address = this.Address,
                PhoneNumber = this.PhoneNumber,
            };
        }
    }
}
