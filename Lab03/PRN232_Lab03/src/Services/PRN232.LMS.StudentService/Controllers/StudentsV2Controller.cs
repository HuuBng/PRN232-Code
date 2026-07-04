using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Shared.Models;
using PRN232.LMS.StudentService.Models;
using PRN232.LMS.StudentService.Services;

namespace PRN232.LMS.StudentService.Controllers
{
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/v{version:apiVersion}/students")]
    [Authorize]
    public class StudentsV2Controller(IStudentService studentService) : ControllerBase
    {
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<StudentV2Response>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] QueryParameters query)
        {
            var result = await studentService.GetStudentsV2Async(query);

            if (FieldSelector.HasFields(query.Fields))
            {
                var selected = new PaginatedResponse<object>
                {
                    Items = FieldSelector.SelectFields(result.Items, query.Fields),
                    Pagination = result.Pagination
                };

                return Ok(ApiResponse<PaginatedResponse<object>>.Ok(selected));
            }

            return Ok(ApiResponse<PaginatedResponse<StudentV2Response>>.Ok(result));
        }

        [ProducesResponseType(typeof(ApiResponse<StudentV2Response>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<StudentV2Response>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int id,
            [FromQuery] string? fields,
            [FromHeader(Name = "X-Request-Id")] string? requestId)
        {
            var student = await studentService.GetStudentByIdV2Async(id);
            if (student == null)
            {
                return NotFound(ApiResponse<StudentV2Response>.Fail("Student not found"));
            }

            return Ok(FieldSelector.HasFields(fields)
                ? ApiResponse<object>.Ok(FieldSelector.SelectFields(student, fields))
                : ApiResponse<StudentV2Response>.Ok(student));
        }
    }
}
