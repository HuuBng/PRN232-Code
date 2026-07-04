using PRN232.LMS.Shared.Models;
using PRN232.LMS.StudentService.Models;

namespace PRN232.LMS.StudentService.Services
{
    public interface IStudentService
    {
        Task<PaginatedResponse<StudentResponse>> GetStudentsAsync(QueryParameters query);
        Task<PaginatedResponse<StudentV2Response>> GetStudentsV2Async(QueryParameters query);
        Task<StudentResponse?> GetStudentByIdAsync(int id);
        Task<StudentV2Response?> GetStudentByIdV2Async(int id);
        Task<StudentResponse> CreateStudentAsync(StudentRequest request);
        Task<StudentResponse?> UpdateStudentAsync(int id, StudentRequest request);
        Task<bool> DeleteStudentAsync(int id);
    }
}
