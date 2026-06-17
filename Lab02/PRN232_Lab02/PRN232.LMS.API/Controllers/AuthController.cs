using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Auth;
using PRN232.LMS.Services.Models.Common;

namespace PRN232.LMS.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/auth")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await authService.LoginAsync(request);
            if (result == null)
            {
                return Unauthorized(ApiResponse<AuthTokenResponse>.Fail("Invalid username or password"));
            }

            return Ok(ApiResponse<AuthTokenResponse>.Ok(result, "Login successful"));
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result == null)
            {
                return Unauthorized(ApiResponse<AuthTokenResponse>.Fail("Invalid refresh token"));
            }

            return Ok(ApiResponse<AuthTokenResponse>.Ok(result, "Token refreshed successfully"));
        }
    }
}
