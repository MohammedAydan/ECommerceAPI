using ECommerceAPI.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Model.DTOs
{
    public class SignUpDto
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public IFormFile? Image { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 50 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Confirm Password does not match the Password.")]
        public string ConfirmPassword { get; set; }


        // Check if any field is empty
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(UserName) ||
                   string.IsNullOrWhiteSpace(Email) ||
                   string.IsNullOrWhiteSpace(Country) ||
                   string.IsNullOrWhiteSpace(City) ||
                   string.IsNullOrWhiteSpace(Address) ||
                   string.IsNullOrWhiteSpace(Password) ||
                   string.IsNullOrWhiteSpace(ConfirmPassword);
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
