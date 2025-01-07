using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<AuthResponseDto> SignInAsync(SignInDto signIn);
        Task<AuthResponseDto> SignUpAsync(SignUpDto signUp);
        Task<UserModel?> GetByIdAsync(string id);
        Task<IEnumerable<string>> GetUserRolesAsync(UserModel user);
        Task<AuthResponseDto> RefreshToken(string refreshToken);
    }
}
