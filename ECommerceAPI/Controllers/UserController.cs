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

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromForm] UserModel userModel)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(userModel);
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
    }
}
