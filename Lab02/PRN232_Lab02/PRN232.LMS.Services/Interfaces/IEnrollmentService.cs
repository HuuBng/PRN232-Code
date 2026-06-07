using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Enrollments;
namespace PRN232.LMS.Services.Interfaces
{
    public class EnrollmentValidationException(string message) : Exception(message);

    /// <summary>
    ///     Khai báo các nghiệp vụ cần có cho IEnrollmentService.
    /// </summary>
    public interface IEnrollmentService
    {
        Task<PaginatedResponse<EnrollmentResponse>> GetEnrollmentsAsync(QueryParameters query);
        Task<EnrollmentResponse?> GetEnrollmentByIdAsync(int id, string? expand = null);
        Task<EnrollmentResponse> CreateEnrollmentAsync(EnrollmentRequest request);
        Task<EnrollmentResponse?> UpdateEnrollmentAsync(int id, EnrollmentRequest request);
        Task<bool> DeleteEnrollmentAsync(int id);
    }
}
