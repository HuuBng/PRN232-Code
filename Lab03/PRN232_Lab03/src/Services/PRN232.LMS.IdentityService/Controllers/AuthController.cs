using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.IdentityService.Models;
using PRN232.LMS.IdentityService.Services;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.IdentityService.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Produces("application/json")]
    [Route("api/auth")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status401Unauthorized)]
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
