using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Students;
namespace PRN232.LMS.API.Controllers
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho StudentsController.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/students")]
    [Route("api/v{version:apiVersion}/students")]
    [Authorize]
    public class StudentsController(IStudentService studentService) : ControllerBase
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetStudents.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StudentResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] QueryParameters query)
        {
            var result = await studentService.GetStudentsAsync(query);

            if (FieldSelector.HasFields(query.Fields))
            {
                var selected = new PaginatedResponse<object>
                {
                    Items = FieldSelector.SelectFields(result.Items, query.Fields),
                    Pagination = result.Pagination
                };

                return Ok(ApiResponse<PaginatedResponse<object>>.Ok(selected));
            }

            return Ok(ApiResponse<PaginatedResponse<StudentResponse>>.Ok(result));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ GetStudentById.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStudentById(
            [FromRoute] int id,
            [FromQuery] string? expand,
            [FromQuery] string? fields,
            [FromHeader(Name = "X-Request-Id")] string? requestId)
        {
            var student = await studentService.GetStudentByIdAsync(id, expand);
            if (student == null)
            {
                return NotFound(ApiResponse<StudentResponse>.Fail("Student not found"));
            }

            return Ok(FieldSelector.HasFields(fields)
                ? ApiResponse<object>.Ok(FieldSelector.SelectFields(student, fields))
                : ApiResponse<StudentResponse>.Ok(student));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateStudent.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentRequest request)
        {
            var student = await studentService.CreateStudentAsync(request);
            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = student.StudentId },
                ApiResponse<StudentResponse>.Ok(student, "Student created successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateStudent.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentRequest request)
        {
            var student = await studentService.UpdateStudentAsync(id, request);
            if (student == null)
            {
                return NotFound(ApiResponse<StudentResponse>.Fail("Student not found"));
            }

            return Ok(ApiResponse<StudentResponse>.Ok(student, "Student updated successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ DeleteStudent.
        /// </summary>
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var deleted = await studentService.DeleteStudentAsync(id);
            if (!deleted)
            {
                return NotFound(ApiResponse<object>.Fail("Student not found"));
            }

            return Ok(ApiResponse<object>.Ok(null, "Student deleted successfully"));
        }
    }
}
