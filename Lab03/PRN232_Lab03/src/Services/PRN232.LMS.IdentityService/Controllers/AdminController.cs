using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.IdentityService.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public IActionResult Dashboard()
        {
            return Ok(ApiResponse<object>.Ok(new { message = "Admin dashboard data" }, "Welcome to the admin dashboard"));
        }
    }
}
