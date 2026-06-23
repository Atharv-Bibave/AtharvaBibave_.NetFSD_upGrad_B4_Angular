using Asp.Versioning;
using EMS.Application.DTOs;
using EMS.Application.Services.Interfaces;
using EMS.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace EMS.WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            if (!result)
                return Conflict(ApiResponse.Fail("Email already registered."));

            return Ok(ApiResponse.Ok("Registration successful."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto);
            if (token == null)
                return Unauthorized(ApiResponse.Fail("Invalid email or password."));

            return Ok(ApiResponse.Ok(new { Token = token }, "Login successful."));
        }
    }
}
