using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Courses;
namespace PRN232.LMS.Services.Interfaces
{
    public class CourseValidationException(string message) : Exception(message);

    /// <summary>
    ///     Khai báo các nghiệp vụ cần có cho ICourseService.
    /// </summary>
    public interface ICourseService
    {
        Task<PaginatedResponse<CourseResponse>> GetCoursesAsync(QueryParameters query);
        Task<CourseResponse?> GetCourseByIdAsync(int id, string? expand = null);
        Task<CourseResponse> CreateCourseAsync(CourseRequest request);
        Task<CourseResponse?> UpdateCourseAsync(int id, CourseRequest request);
        Task<bool> DeleteCourseAsync(int id);
    }
}
