using Asp.Versioning;
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
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/courses")]
    [Route("api/v{version:apiVersion}/courses")]
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
                : Ok(FieldSelector.HasFields(fields)
                    ? ApiResponse<object>.Ok(FieldSelector.SelectFields(course, fields))
                    : ApiResponse<CourseResponse>.Ok(course));
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
            try
            {
                var course = await courseService.CreateCourseAsync(request);
                return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseId }, ApiResponse<CourseResponse>.Ok(course, "Course created successfully"));
            }
            catch (CourseValidationException ex)
            {
                return BadRequest(ApiResponse<CourseResponse>.Fail(ex.Message));
            }
        }

        /// <summary>
        ///     Creates a course attached to a specific semester route.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("/api/semesters/{semesterId:int}/courses")]
        [HttpPost("/api/v{version:apiVersion}/semesters/{semesterId:int}/courses")]
        public async Task<IActionResult> CreateCourseForSemester(int semesterId, [FromBody] CourseForSemesterRequest request)
        {
            try
            {
                var subjectId = request.SubjectId ?? await courseService.GetDefaultSubjectIdAsync();
                var courseRequest = new CourseRequest
                {
                    CourseName = request.CourseName,
                    SemesterId = semesterId,
                    SubjectId = subjectId
                };

                var course = await courseService.CreateCourseAsync(courseRequest);
                return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseId }, ApiResponse<CourseResponse>.Ok(course, "Course created successfully"));
            }
            catch (CourseValidationException ex)
            {
                return BadRequest(ApiResponse<CourseResponse>.Fail(ex.Message));
            }
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
            try
            {
                var course = await courseService.UpdateCourseAsync(id, request);
                return course == null
                    ? NotFound(ApiResponse<CourseResponse>.Fail("Course not found"))
                    : Ok(ApiResponse<CourseResponse>.Ok(course, "Course updated successfully"));
            }
            catch (CourseValidationException ex)
            {
                return BadRequest(ApiResponse<CourseResponse>.Fail(ex.Message));
            }
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
