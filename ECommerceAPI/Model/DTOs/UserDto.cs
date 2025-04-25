using ECommerceAPI.Helpers;
using ECommerceAPI.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Model.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string? PhoneNumber { get; set; }
        public List<string>? Roles { get; set; } = null;

        public static UserDto convertToUserDto(UserModel user, List<string>? roles = null)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                ImageUrl = user.ImageUrl,
                Country = user.Country,
                City = user.City,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Roles = roles,
            };
        }
    }
}
