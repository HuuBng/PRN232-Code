using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Semesters;
namespace PRN232.LMS.API.Controllers
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho SemestersController.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/semesters")]
    [Route("api/v{version:apiVersion}/semesters")]
    [Authorize]
    public class SemestersController(ISemesterService semesterService) : ControllerBase
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetSemesters.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SemesterResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetSemesters([FromQuery] QueryParameters query)
        {
            var result = await semesterService.GetSemestersAsync(query);

            if (FieldSelector.HasFields(query.Fields))
            {
                var selected = new PaginatedResponse<object>
                {
                    Items = FieldSelector.SelectFields(result.Items, query.Fields),
                    Pagination = result.Pagination
                };

                return Ok(ApiResponse<PaginatedResponse<object>>.Ok(selected));
            }

            return Ok(ApiResponse<PaginatedResponse<SemesterResponse>>.Ok(result));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ GetSemesterById.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSemesterById(int id, [FromQuery] string? fields)
        {
            var semester = await semesterService.GetSemesterByIdAsync(id);
            return semester == null
                ? NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"))
                : Ok(FieldSelector.HasFields(fields)
                    ? ApiResponse<object>.Ok(FieldSelector.SelectFields(semester, fields))
                    : ApiResponse<SemesterResponse>.Ok(semester));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateSemester.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterRequest request)
        {
            var semester = await semesterService.CreateSemesterAsync(request);
            return CreatedAtAction(nameof(GetSemesterById), new { id = semester.SemesterId }, ApiResponse<SemesterResponse>.Ok(semester, "Semester created successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateSemester.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterRequest request)
        {
            var semester = await semesterService.UpdateSemesterAsync(id, request);
            return semester == null
                ? NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"))
                : Ok(ApiResponse<SemesterResponse>.Ok(semester, "Semester updated successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ DeleteSemester.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            var deleted = await semesterService.DeleteSemesterAsync(id);
            return !deleted
                ? NotFound(ApiResponse<object>.Fail("Semester not found"))
                : Ok(ApiResponse<object>.Ok(null, "Semester deleted successfully"));
        }
    }
}
