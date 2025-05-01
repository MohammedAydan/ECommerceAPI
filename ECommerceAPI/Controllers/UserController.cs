using ECommerceAPI.Model.DTOs;
using ECommerceAPI.Model.Entities;
using ECommerceAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerceAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (id == null)
                {
                    return NotFound("User not found");
                }

                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // SignIn user
        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto signIn)
        {
            try
            {
                var authRes = await _userService.SignInAsync(signIn);
                if (authRes == null)
                {
                    return Unauthorized("Unauthorized");
                }

                return Ok(authRes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("signUp")]
        public async Task<IActionResult> SignUp([FromForm] SignUpDto signUp)
        {
            try
            {
                var authRes = await _userService.SignUpAsync(signUp);
                if (authRes == null)
                {
                    return Unauthorized("Unauthorized");
                }

                return Ok(authRes);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //[Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshToken)
        {
            try
            {
                var res = await _userService.RefreshToken(refreshToken.RefreshToken);
                if (res == null)
                {
                    return Unauthorized("Unauthorized");
                }

                return Ok(res);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto updateUserDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return NotFound("User not found");
                }

                var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
                if (updatedUser == null)
                {
                    return NotFound("User not found");
                }
                return Ok(updatedUser);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int limit = 10, string? search = null, string? sortBy = "Id", bool ascending = true, Dictionary<string, string>? filters = null)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(page, limit, search, sortBy, ascending, filters);
                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
