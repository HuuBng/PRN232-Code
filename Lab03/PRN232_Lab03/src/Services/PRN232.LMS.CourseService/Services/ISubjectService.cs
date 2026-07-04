using PRN232.LMS.CourseService.Models.Subjects;
using PRN232.LMS.Shared.Models;

namespace PRN232.LMS.CourseService.Services
{
    public interface ISubjectService
    {
        Task<PaginatedResponse<SubjectResponse>> GetSubjectsAsync(QueryParameters query);
        Task<SubjectResponse?> GetSubjectByIdAsync(int id);
        Task<SubjectResponse> CreateSubjectAsync(SubjectRequest request);
        Task<SubjectResponse?> UpdateSubjectAsync(int id, SubjectRequest request);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
