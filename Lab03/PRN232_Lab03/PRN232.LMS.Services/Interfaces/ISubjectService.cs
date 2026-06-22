using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Subjects;
namespace PRN232.LMS.Services.Interfaces
{
    /// <summary>
    ///     Khai báo các nghiệp vụ cần có cho ISubjectService.
    /// </summary>
    public interface ISubjectService
    {
        Task<PaginatedResponse<SubjectResponse>> GetSubjectsAsync(QueryParameters query);
        Task<SubjectResponse?> GetSubjectByIdAsync(int id);
        Task<SubjectResponse> CreateSubjectAsync(SubjectRequest request);
        Task<SubjectResponse?> UpdateSubjectAsync(int id, SubjectRequest request);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
