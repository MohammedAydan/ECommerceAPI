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
        Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto userModel);

        Task<IEnumerable<UserDto>> GetAllUsersAsync(int? page = 1, int? limit = 10, string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null);
    }
}
