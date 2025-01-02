using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;

namespace ECommerceAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDto> SignInAsync(SignInDto signIn);
        Task<AuthResponseDto> SignUpAsync(SignUpDto signUp);
        Task<UserDto?> GetUserByIdAsync(string id);
    }
}
