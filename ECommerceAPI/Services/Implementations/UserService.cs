using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using ECommerceAPI.Services.Interfaces;

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
            return user == null ? null : UserDto.convertToUserDto(user);
        }

        public async Task<AuthResponseDto> SignInAsync(SignInDto signIn)
        {
            return await _userRepository.SignInAsync(signIn);
        }

        public async Task<AuthResponseDto> SignUpAsync(SignUpDto signUp)
        {
            return await _userRepository.SignUpAsync(signUp);
        }
    }
}
