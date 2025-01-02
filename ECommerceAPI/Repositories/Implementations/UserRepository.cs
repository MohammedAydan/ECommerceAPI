using ECommerceAPI.Data;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;

namespace ECommerceAPI.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserModel> _userManager;

        public UserRepository(AppDbContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<UserModel?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return await _context.Users.FindAsync(id);
        }

        public async Task<AuthResponseDto> SignInAsync(SignInDto signIn)
        {
            if (signIn == null || signIn.IsEmpty())
            {
                return CreateAuthResponse(401, "Invalid email or password.");
            }

            var user = await _userManager.FindByEmailAsync(signIn.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, signIn.Password))
            {
                return CreateAuthResponse(401, "Invalid email or password.");
            }

            List<string> roles = (await _userManager.GetRolesAsync(user)).ToList();

            return CreateAuthResponse(200, "Sign-in successful.", UserDto.convertToUserDto(user, roles));
        }

        public async Task<AuthResponseDto> SignUpAsync(SignUpDto signUp)
        {
            if (signUp == null || signUp.IsEmpty())
            {
                return CreateAuthResponse(401, "Invalid data provided.");
            }

            if (await _userManager.FindByEmailAsync(signUp.Email) != null)
            {
                return CreateAuthResponse(401, "Email already exists.");
            }

            if (await _userManager.FindByNameAsync(signUp.UserName) != null)
            {
                return CreateAuthResponse(401, "Username already exists.");
            }

            var userModel = signUp.ConvertToUserModel();
            var result = await _userManager.CreateAsync(userModel, signUp.Password);

            if (result == null || !result.Succeeded)
            {
                return CreateAuthResponse(401, "Error signing up user.", errors: result?.Errors.Select(e => e.Description).ToList());
            }

            // assign role
            await _userManager.AddToRoleAsync(userModel,"User");

            return CreateAuthResponse(200, "Sign-up successful.", UserDto.convertToUserDto(userModel, ["User"]));
        }

        private AuthResponseDto CreateAuthResponse(int code, string message, UserDto user = null, List<string> errors = null)
        {
            return new AuthResponseDto
            {
                Code = code,
                Message = message,
                User = user,
                Errors = errors
            };
        }
    }
}
