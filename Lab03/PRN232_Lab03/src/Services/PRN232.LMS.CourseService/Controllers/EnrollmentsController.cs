using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.CourseService.Models.Enrollments;
using PRN232.LMS.CourseService.Services;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/enrollments")]
    [Route("api/v{version:apiVersion}/enrollments")]
    [Authorize]
    public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
    {
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<EnrollmentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetEnrollments([FromQuery] QueryParameters query)
        {
            var result = await enrollmentService.GetEnrollmentsAsync(query);

            if (FieldSelector.HasFields(query.Fields))
            {
                var selected = new PaginatedResponse<object>
                {
                    Items = FieldSelector.SelectFields(result.Items, query.Fields),
                    Pagination = result.Pagination
                };

                return Ok(ApiResponse<PaginatedResponse<object>>.Ok(selected));
            }

            return Ok(ApiResponse<PaginatedResponse<EnrollmentResponse>>.Ok(result));
        }

        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEnrollmentById(
            int id,
            [FromQuery] string? expand,
            [FromQuery] string? fields)
        {
            var enrollment = await enrollmentService.GetEnrollmentByIdAsync(id, expand);
            return enrollment == null
                ? NotFound(ApiResponse<EnrollmentResponse>.Fail("Enrollment not found"))
                : Ok(FieldSelector.HasFields(fields)
                    ? ApiResponse<object>.Ok(FieldSelector.SelectFields(enrollment, fields))
                    : ApiResponse<EnrollmentResponse>.Ok(enrollment));
        }

        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentRequest request)
        {
            try
            {
                var enrollment = await enrollmentService.CreateEnrollmentAsync(request);
                return CreatedAtAction(nameof(GetEnrollmentById), new { id = enrollment.EnrollmentId }, ApiResponse<EnrollmentResponse>.Ok(enrollment, "Enrollment created successfully"));
            }
            catch (EnrollmentValidationException ex)
            {
                return BadRequest(ApiResponse<EnrollmentResponse>.Fail(ex.Message));
            }
        }

        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentRequest request)
        {
            try
            {
                var enrollment = await enrollmentService.UpdateEnrollmentAsync(id, request);
                return enrollment == null
                    ? NotFound(ApiResponse<EnrollmentResponse>.Fail("Enrollment not found"))
                    : Ok(ApiResponse<EnrollmentResponse>.Ok(enrollment, "Enrollment updated successfully"));
            }
            catch (EnrollmentValidationException ex)
            {
                return BadRequest(ApiResponse<EnrollmentResponse>.Fail(ex.Message));
            }
        }

        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var deleted = await enrollmentService.DeleteEnrollmentAsync(id);
            return !deleted
                ? NotFound(ApiResponse<object>.Fail("Enrollment not found"))
                : Ok(ApiResponse<object>.Ok(null, "Enrollment deleted successfully"));
        }
    }
}
