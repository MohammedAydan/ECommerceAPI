using ECommerceAPI.Model.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.Model.DTOs
{
    public class SignInDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 50 characters.")]
        public string Password { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password);
        }
    }
}
