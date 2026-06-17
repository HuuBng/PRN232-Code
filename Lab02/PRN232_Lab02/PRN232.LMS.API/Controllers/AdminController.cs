using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Models.Common;
namespace PRN232.LMS.API.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/admin")]
    [Route("api/v{version:apiVersion}/admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("dashboard")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public IActionResult GetDashboard([FromHeader(Name = "X-Request-Id")] string? requestId)
        {
            return Ok(ApiResponse<object>.Ok(new
            {
                message = "Admin endpoint is protected by JWT role-based authorization.",
                requestId
            }));
        }
    }
}
