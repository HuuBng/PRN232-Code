using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Students;
namespace PRN232.LMS.API.Controllers
{
    /// <summary>
    ///     v2 of the Students endpoint. Surfaces <c>PhoneNumber</c> and
    ///     <c>StudentCode</c> in the response alongside the v1 fields.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/v{version:apiVersion}/students")]
    public class StudentsV2Controller(IStudentService studentService) : ControllerBase
    {
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            [FromRoute] int id,
            [FromQuery] string? expand,
            [FromQuery] string? fields,
            [FromHeader(Name = "X-Request-Id")] string? requestId)
        {
            var student = await studentService.GetStudentByIdV2Async(id, expand);
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
