using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Courses;
namespace PRN232.LMS.API.Controllers
{
    /// <summary>
    ///     Mô hình/lớp xử lý cho CoursesController.
    /// </summary>
    [ApiController]
    [Route("api/courses")]
    public class CoursesController(ICourseService courseService) : ControllerBase
    {
        /// <summary>
        ///     Xử lý request/nghiệp vụ GetCourses.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] QueryParameters query)
        {
            var result = await courseService.GetCoursesAsync(query);

            if (FieldSelector.HasFields(query.Fields))
            {
                var selected = new PaginatedResponse<object>
                {
                    Items = FieldSelector.SelectFields(result.Items, query.Fields),
                    Pagination = result.Pagination
                };

                return Ok(ApiResponse<PaginatedResponse<object>>.Ok(selected));
            }

            return Ok(ApiResponse<PaginatedResponse<CourseResponse>>.Ok(result));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ GetCourseById.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourseById(int id, [FromQuery] string? expand, [FromQuery] string? fields)
        {
            var course = await courseService.GetCourseByIdAsync(id, expand);
            return course == null
                ? NotFound(ApiResponse<CourseResponse>.Fail("Course not found"))
                : Ok(ApiResponse<object>.Ok(FieldSelector.HasFields(fields) ? FieldSelector.SelectFields(course, fields) : course));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ CreateCourse.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequest request)
        {
            var course = await courseService.CreateCourseAsync(request);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseId }, ApiResponse<CourseResponse>.Ok(course, "Course created successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ UpdateCourse.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseRequest request)
        {
            var course = await courseService.UpdateCourseAsync(id, request);
            return course == null
                ? NotFound(ApiResponse<CourseResponse>.Fail("Course not found"))
                : Ok(ApiResponse<CourseResponse>.Ok(course, "Course updated successfully"));
        }

        /// <summary>
        ///     Xử lý request/nghiệp vụ DeleteCourse.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var deleted = await courseService.DeleteCourseAsync(id);
            return !deleted
                ? NotFound(ApiResponse<object>.Fail("Course not found"))
                : Ok(ApiResponse<object>.Ok(null, "Course deleted successfully"));
        }
    }
}
