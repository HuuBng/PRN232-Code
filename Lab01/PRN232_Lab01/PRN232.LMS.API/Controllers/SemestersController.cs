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
    [Route("api/semesters")]
    public class SemestersController(ISemesterService semesterService) : ControllerBase
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetSemesters.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSemesterById(int id, [FromQuery] string? fields)
        {
            var semester = await semesterService.GetSemesterByIdAsync(id);
            return semester == null
                ? NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"))
                : Ok(ApiResponse<object>.Ok(FieldSelector.HasFields(fields) ? FieldSelector.SelectFields(semester, fields) : semester));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateSemester.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterRequest request)
        {
            var semester = await semesterService.CreateSemesterAsync(request);
            return CreatedAtAction(nameof(GetSemesterById), new { id = semester.SemesterId }, ApiResponse<SemesterResponse>.Ok(semester, "Semester created successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateSemester.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
