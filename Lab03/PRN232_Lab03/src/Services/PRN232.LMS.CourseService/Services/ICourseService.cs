using PRN232.LMS.CourseService.Models.Courses;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Services
{
    public class CourseValidationException(string message) : Exception(message);

    public interface ICourseService
    {
        Task<PaginatedResponse<CourseResponse>> GetCoursesAsync(QueryParameters query);
        Task<CourseResponse?> GetCourseByIdAsync(int id, string? expand = null);
        Task<CourseResponse> CreateCourseAsync(CourseRequest request);
        Task<CourseResponse?> UpdateCourseAsync(int id, CourseRequest request);
        Task<bool> DeleteCourseAsync(int id);
        Task<int> GetDefaultSubjectIdAsync();
    }
}
