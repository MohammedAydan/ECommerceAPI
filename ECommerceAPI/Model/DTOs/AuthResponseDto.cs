using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Model.DTOs
{
    public class AuthResponseDto
    {
        public int Code { get; set; } = 200;
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { set; get; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public UserDto? User { get; set; }
    }
}
