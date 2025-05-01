using ECommerceAPI.Data;
using ECommerceAPI.Helpers;
using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ECommerceAPI.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        private readonly TokenHelper _tokenHelper;
        private readonly IConfiguration _configuration;
        private readonly UploadImagesHelper _imagesHelper;

        public UserRepository(AppDbContext context, UserManager<UserModel> userManager, TokenHelper tokenHelper, IConfiguration configuration, UploadImagesHelper imagesHelper)
        {
            _context = context;
            _userManager = userManager;
            _tokenHelper = tokenHelper;
            _configuration = configuration;
            _imagesHelper = imagesHelper;
        }

        public async Task<UserModel?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            return await _context.Users.FindAsync(id);
        }

        public async Task<IEnumerable<string>> GetUserRolesAsync(UserModel user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<AuthResponseDto> SignInAsync(SignInDto signIn)
        {
            if (signIn == null || signIn.IsEmpty())
                return CreateAuthResponse(400, "Invalid email or password.");

            var user = await _userManager.FindByEmailAsync(signIn.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, signIn.Password))
                return CreateAuthResponse(400, "Invalid email or password.");

            user.LastSignIn = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var userDto = UserDto.convertToUserDto(user, roles);

            if (userDto.ImageUrl != null)
                userDto.ImageUrl = _imagesHelper.GetImagePath(userDto.ImageUrl, "users");

            var accessToken = _tokenHelper.CreateAccessToken(userDto);
            var refreshToken = await CreateRefreshTokenAsync(user.Id);

            return CreateAuthResponse(200, "Sign-in successful.", userDto, accessToken, refreshToken);
        }

        public async Task<AuthResponseDto> SignUpAsync(SignUpDto signUp)
        {
            if (signUp == null || signUp.IsEmpty())
                return CreateAuthResponse(400, "Invalid data provided.");

            if (await _userManager.FindByEmailAsync(signUp.Email) != null)
                return CreateAuthResponse(400, "Email already exists.");

            if (await _userManager.FindByNameAsync(signUp.UserName) != null)
                return CreateAuthResponse(400, "Username already exists.");

            var userModel = signUp.ConvertToUserModel();
            var result = await _userManager.CreateAsync(userModel, signUp.Password);

            if (result == null || !result.Succeeded)
                return CreateAuthResponse(400, "Failed to create user.", errors: result?.Errors.Select(e => e.Description).ToList());

            // Upload image if provided
            if (signUp.Image != null)
            {
                userModel.ImageUrl = await _imagesHelper.UploadImageAsync(signUp.Image, "users");
            }

            userModel.CreatedAt = DateTime.UtcNow;
            userModel.LastSignIn = DateTime.UtcNow;
            await _userManager.UpdateAsync(userModel);

            await _userManager.AddToRoleAsync(userModel, "User");

            var userDto = UserDto.convertToUserDto(userModel, ["User"]);
            if (userDto.ImageUrl != null)
                userDto.ImageUrl = _imagesHelper.GetImagePath(userDto.ImageUrl, "users");

            var accessToken = _tokenHelper.CreateAccessToken(userDto);
            var refreshToken = await CreateRefreshTokenAsync(userDto.Id);

            return CreateAuthResponse(201, "Sign-up successful.", userDto, accessToken, refreshToken);
        }

        public async Task<AuthResponseDto> RefreshToken(string refreshToken)
        {
            var refreshTokenEntity = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);

            if (refreshTokenEntity == null || refreshTokenEntity.Expires < DateTime.UtcNow || refreshTokenEntity.IsUsed || refreshTokenEntity.IsRevoked)
                return CreateAuthResponse(400, "Invalid refresh token.",errors: ["Invalid Refresh Token"]);

            // Remove old refresh token (optional: mark as used)
            _context.RefreshTokens.Remove(refreshTokenEntity);
            await _context.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(refreshTokenEntity.UserId);
            if (user == null)
                return CreateAuthResponse(404, "User not found.", errors: ["User not found."]);

            var userDto = UserDto.convertToUserDto(user);

            var newAccessToken = _tokenHelper.CreateAccessToken(userDto);
            var newRefreshToken = await CreateRefreshTokenAsync(userDto.Id);

            return CreateAuthResponse(200, "Token refreshed successfully.", userDto, newAccessToken, newRefreshToken);
        }

        public async Task<UserModel?> UpdateUserAsync(UserModel userModel)
        {
            if (userModel == null)
                throw new ArgumentNullException(nameof(userModel));

            var existingUser = await _context.Users.FindAsync(userModel.Id);
            if (existingUser == null)
                throw new InvalidOperationException("User not found.");

            if (userModel.Image != null)
            {
                if (!string.IsNullOrEmpty(existingUser.ImageUrl))
                    _imagesHelper.DeleteImage(existingUser.ImageUrl);

                existingUser.ImageUrl = await _imagesHelper.UploadImageAsync(userModel.Image, "users");
            }

            existingUser.UserName = userModel.UserName ?? existingUser.UserName;
            existingUser.Email = userModel.Email ?? existingUser.Email;
            existingUser.PhoneNumber = userModel.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.Address = userModel.Address ?? existingUser.Address;
            existingUser.City = userModel.City ?? existingUser.City;
            existingUser.Country = userModel.Country ?? existingUser.Country;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            return existingUser;
        }

        // PRIVATE HELPERS

        private AuthResponseDto CreateAuthResponse(int code, string message, UserDto? user = null, string? accessToken = null, string? refreshToken = null, List<string>? errors = null)
        {
            return new AuthResponseDto
            {
                Code = code,
                Message = message,
                User = user,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Errors = errors
            };
        }

        private async Task<string> CreateRefreshTokenAsync(string userId)
        {
            var refreshToken = _tokenHelper.CreateRefreshToken();
            var expirationMonths = int.Parse(_configuration["JWT:RefreshTokenExpir"] ?? "3");

            var tokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                Expires = DateTime.UtcNow.AddMonths(expirationMonths),
                IsRevoked = false,
                IsUsed = false
            };

            await _context.RefreshTokens.AddAsync(tokenEntity);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync(
            int? page = 1,
            int? limit = 10,
            string? search = null,
            string? sortBy = "Id",
            bool ascending = true,
            Dictionary<string, string>? filters = null)
        {
            page = page.HasValue && page.Value > 0 ? page.Value : 1;
            limit = limit.HasValue && limit.Value > 0 ? limit.Value : 10;

            IQueryable<UserModel> query = _context.Users.AsQueryable();

            // Search (applies to string fields — you can customize which fields to search)
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.UserName.Contains(search)); // Example: Adjust "UserName" to match your schema
            }

            // Apply filters
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    var propertyInfo = typeof(UserModel).GetProperty(filter.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null) continue;

                    var parameter = Expression.Parameter(typeof(UserModel), "x");
                    var property = Expression.Property(parameter, propertyInfo);

                    object? typedValue;
                    try
                    {
                        typedValue = Convert.ChangeType(filter.Value, propertyInfo.PropertyType);
                    }
                    catch
                    {
                        continue; // Skip this filter if conversion fails
                    }

                    var constant = Expression.Constant(typedValue);
                    Expression condition;

                    if (propertyInfo.PropertyType == typeof(string))
                    {
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        condition = Expression.Call(property, containsMethod!, constant);
                    }
                    else
                    {
                        condition = Expression.Equal(property, constant);
                    }

                    var lambda = Expression.Lambda<Func<UserModel, bool>>(condition, parameter);
                    query = query.Where(lambda);
                }
            }

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                var propertyInfo = typeof(UserModel).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo != null)
                {
                    var parameter = Expression.Parameter(typeof(UserModel), "x");
                    var property = Expression.Property(parameter, propertyInfo);
                    var sortLambda = Expression.Lambda(property, parameter);

                    string methodName = ascending ? "OrderBy" : "OrderByDescending";
                    var method = typeof(Queryable).GetMethods()
                        .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .MakeGenericMethod(typeof(UserModel), propertyInfo.PropertyType);

                    query = (IQueryable<UserModel>)method.Invoke(null, new object[] { query, sortLambda })!;
                }
            }

            // Apply pagination
            query = query.Skip((page.Value - 1) * limit.Value).Take(limit.Value);

            return await query.ToListAsync();
        }
    }
}
