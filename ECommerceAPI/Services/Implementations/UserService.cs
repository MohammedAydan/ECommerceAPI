using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace ECommerceAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if(user == null)
            {
                return null;
            }

            var roles = await this.GetUserRolesAsync(user);
            if (roles.IsNullOrEmpty())
            {
                roles = ["User"];
            }

            return user == null ? null : UserDto.convertToUserDto(user, roles.ToList());
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(UserModel user)
        {
            var roles = await _userRepository.GetUserRolesAsync(user);
            return roles;
        }

        public async Task<AuthResponseDto> SignInAsync(SignInDto signIn)
        {
            return await _userRepository.SignInAsync(signIn);
        }

        public async Task<AuthResponseDto> SignUpAsync(SignUpDto signUp)
        {
            return await _userRepository.SignUpAsync(signUp);
        }

        public async Task<AuthResponseDto> RefreshToken(string refreshToken) 
        {
            return await _userRepository.RefreshToken(refreshToken);
        }
    }
}
