using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Enrollments;
namespace PRN232.LMS.API.Controllers
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho EnrollmentsController.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/enrollments")]
    [Route("api/v{version:apiVersion}/enrollments")]
    public class EnrollmentsController(IEnrollmentService enrollmentService) : ControllerBase
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetEnrollments.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        ///     Xử lý request/nghiệp vụ GetEnrollmentById.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEnrollmentById(int id, [FromQuery] string? expand, [FromQuery] string? fields)
        {
            var enrollment = await enrollmentService.GetEnrollmentByIdAsync(id, expand);
            return enrollment == null
                ? NotFound(ApiResponse<EnrollmentResponse>.Fail("Enrollment not found"))
                : Ok(ApiResponse<object>.Ok(FieldSelector.HasFields(fields) ? FieldSelector.SelectFields(enrollment, fields) : enrollment));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateEnrollment.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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


        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateEnrollment.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        ///     Xử lý request/nghiệp vụ DeleteEnrollment.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
