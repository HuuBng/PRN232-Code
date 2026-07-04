using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Students;
namespace PRN232.LMS.Services.Interfaces
{
    /// <summary>
    ///     Khai báo các nghiệp vụ cần có cho IStudentService.
    /// </summary>
    public interface IStudentService
    {
        Task<PaginatedResponse<StudentResponse>> GetStudentsAsync(QueryParameters query);
        Task<PaginatedResponse<StudentV2Response>> GetStudentsV2Async(QueryParameters query);
        Task<StudentResponse?> GetStudentByIdAsync(int id, string? expand = null);
        Task<StudentResponse> CreateStudentAsync(StudentRequest request);
        Task<StudentResponse?> UpdateStudentAsync(int id, StudentRequest request);
        Task<bool> DeleteStudentAsync(int id);
        Task<StudentV2Response?> GetStudentByIdV2Async(int id, string? expand = null);
    }
}
