using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.CourseService.Models.Courses;
using PRN232.LMS.CourseService.Services;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    [Route("api/courses")]
    [Route("api/v{version:apiVersion}/courses")]
    [Authorize]
    public class CoursesController(ICourseService courseService) : ControllerBase
    {
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<CourseResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<object>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] QueryParameters query)
        {
            var result = await courseService.GetCoursesAsync(query);

            if (FieldSelector.HasFields(query.Fields))
            {
                var selected = new PaginatedResponse<object>
                {
                    Items = FieldSelector.SelectFields<CourseResponse>(result.Items, query.Fields),
                    Pagination = result.Pagination
                };

                return Ok(ApiResponse<PaginatedResponse<object>>.Ok(selected));
            }

            return Ok(ApiResponse<PaginatedResponse<CourseResponse>>.Ok(result));
        }

        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "ReadOrAdmin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCourseById(
            int id,
            [FromQuery] string? expand,
            [FromQuery] string? fields)
        {
            var course = await courseService.GetCourseByIdAsync(id, expand);
            return course == null
                ? NotFound(ApiResponse<CourseResponse>.Fail("Course not found"))
                : Ok(FieldSelector.HasFields(fields)
                    ? ApiResponse<object>.Ok(FieldSelector.SelectFields(course, fields))
                    : ApiResponse<CourseResponse>.Ok(course));
        }

        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
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

        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
        [HttpPost("/api/semesters/{semesterId:int}/courses")]
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

        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
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

        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Authorize(Policy = "AdminOnly")]
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
