using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDto> SignInAsync(SignInDto signIn);
        Task<AuthResponseDto> SignUpAsync(SignUpDto signUp);
        Task<UserDto?> GetUserByIdAsync(string id);
        Task<IEnumerable<string>> GetUserRolesAsync(UserModel user);
        Task<AuthResponseDto> RefreshToken(string refreshToken);
        Task<UserModel?> UpdateUserAsync(UserModel userModel);
    }
}
