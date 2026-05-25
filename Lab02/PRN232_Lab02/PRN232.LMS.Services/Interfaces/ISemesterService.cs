using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Semesters;
namespace PRN232.LMS.Services.Interfaces
{
    /// <summary>
    ///     Khai báo các nghiệp vụ cần có cho ISemesterService.
    /// </summary>
    public interface ISemesterService
    {
        Task<PaginatedResponse<SemesterResponse>> GetSemestersAsync(QueryParameters query);
        Task<SemesterResponse?> GetSemesterByIdAsync(int id);
        Task<SemesterResponse> CreateSemesterAsync(SemesterRequest request);
        Task<SemesterResponse?> UpdateSemesterAsync(int id, SemesterRequest request);
        Task<bool> DeleteSemesterAsync(int id);
    }
}
