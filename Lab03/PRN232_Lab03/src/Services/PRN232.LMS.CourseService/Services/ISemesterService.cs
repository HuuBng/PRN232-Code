using PRN232.LMS.CourseService.Models.Semesters;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Services
{
    public interface ISemesterService
    {
        Task<PaginatedResponse<SemesterResponse>> GetSemestersAsync(QueryParameters query);
        Task<SemesterResponse?> GetSemesterByIdAsync(int id);
        Task<SemesterResponse> CreateSemesterAsync(SemesterRequest request);
        Task<SemesterResponse?> UpdateSemesterAsync(int id, SemesterRequest request);
        Task<bool> DeleteSemesterAsync(int id);
    }
}
