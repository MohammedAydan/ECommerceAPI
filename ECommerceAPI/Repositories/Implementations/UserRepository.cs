using ECommerceAPI.Data;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System.Linq;
using ECommerceAPI.Helpers;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAPI.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly TokenHelper _tokenHelper;
        private readonly IConfiguration _configuration;

        public UserRepository(AppDbContext context, UserManager<UserModel> userManager, TokenHelper tokenHelper, IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
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
            var userDto = UserDto.convertToUserDto(user, roles);

            string accessToken = _tokenHelper.CreateAccessToken(userDto);
            string refreshToken = await createRefreshToken(user.Id);

            return CreateAuthResponse(200, "Sign-in successful.", userDto,AccessToken: accessToken,RefreshToken: refreshToken);
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

            UserDto userDto = UserDto.convertToUserDto(userModel, ["User"]);

            string accessToken = _tokenHelper.CreateAccessToken(userDto);
            string refreshToken = await createRefreshToken(userDto.Id);

            return CreateAuthResponse(200, "Sign-up successful.",userDto , AccessToken: accessToken, RefreshToken: refreshToken);
        }

        public async Task<AuthResponseDto> RefreshToken(string refreshToken) 
        {
            var refreshTokenRes = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if(refreshTokenRes == null || refreshTokenRes.Expires < DateTime.UtcNow || refreshTokenRes.IsUsed || refreshTokenRes.IsRevoked)
            {
                return CreateAuthResponse(401, "Invalid Refresh Token",errors: ["Invalid Refresh Token"]);
            }

            //refreshTokenRes.IsUsed = true;
            //_context.RefreshTokens.Update(refreshTokenRes);

            _context.RefreshTokens.Remove(refreshTokenRes);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(refreshTokenRes.UserId);

            if (user == null) 
            {
                return CreateAuthResponse(401, "User not found", errors: ["Invalid Refresh Token Or User not found"]);
            }

            UserDto userDto = UserDto.convertToUserDto(user);

            string accessToken = _tokenHelper.CreateAccessToken(userDto);
            string newRefreshToken = await createRefreshToken(userDto.Id);

            return CreateAuthResponse(200, "Refresh successful.", userDto, AccessToken: accessToken, RefreshToken: newRefreshToken);
        }

        private AuthResponseDto CreateAuthResponse(int code, string message, UserDto? user = null, List<string>? errors = null,string? AccessToken = null,string? RefreshToken = null)
        {
            return new AuthResponseDto
            {
                Code = code,
                Message = message,
                User = user,
                Errors = errors,
                AccessToken = AccessToken,
                RefreshToken = RefreshToken
            };
        }

        private async Task<string> createRefreshToken(string userId) 
        {
            string refreshToken = _tokenHelper.CreateRefreshToken();
            await _context.RefreshTokens.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                Expires = DateTime.UtcNow.AddMonths(int.Parse(_configuration["JWT:RefreshTokenExpir"]??"3")),
                IsRevoked = false,
                IsUsed = false
            });
            await _context.SaveChangesAsync();

            return refreshToken;
        }
    }
}
