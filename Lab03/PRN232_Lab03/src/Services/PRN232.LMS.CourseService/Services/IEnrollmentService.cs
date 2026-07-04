using PRN232.LMS.CourseService.Models.Enrollments;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Services
{
    public class EnrollmentValidationException(string message) : Exception(message);

    public interface IEnrollmentService
    {
        Task<PaginatedResponse<EnrollmentResponse>> GetEnrollmentsAsync(QueryParameters query);
        Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int id, string? expand = null);
        Task<EnrollmentResponse> CreateEnrollmentAsync(EnrollmentRequest request);
        Task<EnrollmentResponse?> UpdateEnrollmentAsync(int id, EnrollmentRequest request);
        Task<bool> DeleteEnrollmentAsync(int id);
    }
}
