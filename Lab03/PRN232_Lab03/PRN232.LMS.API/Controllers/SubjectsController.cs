using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Subjects;
namespace PRN232.LMS.API.Controllers
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho SubjectsController.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/subjects")]
    [Route("api/v{version:apiVersion}/subjects")]
    [Authorize]
    public class SubjectsController(ISubjectService subjectService) : ControllerBase
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetSubjects.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SubjectResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetSubjects([FromQuery] QueryParameters query)
        {
            var result = await subjectService.GetSubjectsAsync(query);

            if (FieldSelector.HasFields(query.Fields))
            {
                var selected = new PaginatedResponse<object>
                {
                    Items = FieldSelector.SelectFields(result.Items, query.Fields),
                    Pagination = result.Pagination
                };

                return Ok(ApiResponse<PaginatedResponse<object>>.Ok(selected));
            }

            return Ok(ApiResponse<PaginatedResponse<SubjectResponse>>.Ok(result));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ GetSubjectById.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSubjectById(int id, [FromQuery] string? fields)
        {
            var subject = await subjectService.GetSubjectByIdAsync(id);
            return subject == null
                ? NotFound(ApiResponse<SubjectResponse>.Fail("Subject not found"))
                : Ok(FieldSelector.HasFields(fields)
                    ? ApiResponse<object>.Ok(FieldSelector.SelectFields(subject, fields))
                    : ApiResponse<SubjectResponse>.Ok(subject));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateSubject.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectRequest request)
        {
            var subject = await subjectService.CreateSubjectAsync(request);
            return CreatedAtAction(nameof(GetSubjectById), new { id = subject.SubjectId }, ApiResponse<SubjectResponse>.Ok(subject, "Subject created successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateSubject.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectRequest request)
        {
            var subject = await subjectService.UpdateSubjectAsync(id, request);
            return subject == null
                ? NotFound(ApiResponse<SubjectResponse>.Fail("Subject not found"))
                : Ok(ApiResponse<SubjectResponse>.Ok(subject, "Subject updated successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ DeleteSubject.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var deleted = await subjectService.DeleteSubjectAsync(id);
            return !deleted
                ? NotFound(ApiResponse<object>.Fail("Subject not found"))
                : Ok(ApiResponse<object>.Ok(null, "Subject deleted successfully"));
        }
    }
}
